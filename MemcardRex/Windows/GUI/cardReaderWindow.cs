//Hardware card reading device information window
//Shendo 2012 - 2023

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using static MemcardRex.mainWindow;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

using MemcardRex.Core;

namespace MemcardRex
{
    public partial class cardReaderWindow : Form
    {
        private HardwareInterface _hardInterface;
        private bool _quickFormat;
        private string _comPort;
        private string _remoteAddress;
        private int _remoteComPort;
        private string _errorMessage;
        private UInt32 _pocketSerial = 0;
        private byte[] _pocketBIOS = new byte[16384];

        //Reading status flag
        private bool operationCompleted = false;

        BackgroundWorker backgroundWorker = new BackgroundWorker();

        //Complete Memory Card data
        byte[] completeMemoryCard = new byte[131072];

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public byte[] MemoryCard
        {
            get { return completeMemoryCard; }
            set { Array.Copy(value, completeMemoryCard, value.Length); }
        }

        public bool QuickFormat
        {
            set { _quickFormat = value; }
        }

        public string ComPort
        {
            set { _comPort = value; }
        }

        public string RemoteCommAddress
        {
            set { _remoteAddress = value; }
        }
         
        public int RemoteCommPort
        {
            set { _remoteComPort = value; }
        }

        public UInt32 PocketSerial
        {
            get { return _pocketSerial; }
        }

        public byte[] PocketBIOS
        {
            get { return _pocketBIOS; }
        }

        public bool OperationCompleted
        {
            get { return operationCompleted; }
        }

        public cardReaderWindow(HardwareInterface hardInterface)
        {
            InitializeComponent();

            _hardInterface = hardInterface;

            //On HiDPI screens window is not properly sized if control box is missing.
            //This dialog doesn't need control box so we need to disable it after creation
            this.ControlBox = false;

            //Set up events for background worker
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;

            this.Text = hardInterface.Name() + " communication";
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (operationCompleted)
            {
                if(_hardInterface.CommMode == HardwareInterface.CommModes.read || 
                    _hardInterface.CommMode == HardwareInterface.CommModes.psbios)
                    RaiseReadingComplete();
            }

            //Close dialog
            this.Close();
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (_hardInterface.Type == HardwareInterface.Types.unirom && _hardInterface.CommMode == HardwareInterface.CommModes.read)
            {
                if (progressBar.Visible == false && _hardInterface.StoredInRam)
                {
                    progressBar.Visible = true;
                    abortButton.Enabled = true;
                    deviceLabel.Text = "Reading data from Unirom...";
                }
            }
            //First run
            else if (!abortButton.Enabled)
            {
                string interfaceDescription = _hardInterface.Name();

                if (_hardInterface.Firmware() != "") interfaceDescription += " (ver. " + _hardInterface.Firmware() + ")...";
                else interfaceDescription += "...";

                abortButton.Enabled = true;

                switch (_hardInterface.CommMode)
                {
                    case HardwareInterface.CommModes.read:
                        if (_hardInterface.Type == HardwareInterface.Types.unirom)
                        {
                            //Unirom reading mode is special, we have to wait for it to store contents to RAM
                            deviceLabel.Text = "Waiting for Unirom to store contents in RAM.\nTransfer will start after all the sectors have been read.";
                            progressBar.Visible = false;
                            abortButton.Enabled = false;
                        }
                        else
                        {
                            deviceLabel.Text = "Reading data from " + interfaceDescription;
                        }
                        break;

                    case HardwareInterface.CommModes.format:
                        deviceLabel.Text = "Formatting card on " + interfaceDescription;
                        if (_quickFormat) _hardInterface.FrameCount = 64;
                        break;

                    case HardwareInterface.CommModes.write:
                        deviceLabel.Text = "Writing data to " + interfaceDescription;
                        break;

                    case HardwareInterface.CommModes.psbios:
                        deviceLabel.Text = "Dumping BIOS using " + interfaceDescription;
                        break;
                }
                progressBar.Style = ProgressBarStyle.Continuous;
            }
            progressBar.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            byte[] tempDataBuffer = new byte[128];
            ushort i = 0;
            int frameSize = 128;

            //Init interface, either serial or tcp
            if (_hardInterface.Mode == HardwareInterface.Modes.serial)
                _errorMessage = _hardInterface.Start(_comPort, 0);
            else
                _errorMessage = _hardInterface.Start(_remoteAddress, _remoteComPort);

            //Display error message
            if (_errorMessage != null)
            {
                _hardInterface.Stop();

                if (_hardInterface.Type != HardwareInterface.Types.ps3mca)
                    _errorMessage += "\n\nMake sure to select proper communication port and speed in preferences dialog";

                backgroundWorker.CancelAsync();
                return;
            }

            //Check if this is a realtime link
            if(_hardInterface.CommMode == HardwareInterface.CommModes.realtime)
            {
                _hardInterface.Stop();
                _errorMessage = "Realtime not implemented yet";
                return;
            }

            //Check if this is PocketStation serial read
            if(_hardInterface.CommMode == HardwareInterface.CommModes.psinfo)
            {
                _pocketSerial = _hardInterface.ReadPocketStationSerial(out _errorMessage);

                _hardInterface.Stop();
                backgroundWorker.CancelAsync();
                return;
            }

            //Check if this is PocketStation time set
            if (_hardInterface.CommMode == HardwareInterface.CommModes.pstime)
            {
                _hardInterface.SetPocketStationTime(out _errorMessage);

                _hardInterface.Stop();
                backgroundWorker.CancelAsync();
                return;
            }

            //Read serial also before BIOS dumping
            if (_hardInterface.CommMode == HardwareInterface.CommModes.psbios)
            {
                _pocketSerial = _hardInterface.ReadPocketStationSerial(out _errorMessage);
                Thread.Sleep(10);

                //Break out in case of an error
                if(_errorMessage != null)
                {
                    _hardInterface.Stop();
                    backgroundWorker.CancelAsync();
                    return;
                }
            }

            //Process all frames of the Memory Card
            while (i < _hardInterface.FrameCount)
            {
                //Check if the "Abort" button has been pressed
                if (backgroundWorker.CancellationPending == true)
                {
                    //Cleanly close the interface
                    if (_hardInterface != null) _hardInterface.Stop();
                    return;
                }

                //Are we reading or writing data
                if (_hardInterface.CommMode == HardwareInterface.CommModes.read)
                {
                    //Get 128 byte frame data from hardware device
                    tempDataBuffer = _hardInterface.ReadMemoryCardFrame(i);

                    //Check if there was a checksum mismatch
                    if (tempDataBuffer != null)
                    {
                        Array.Copy(tempDataBuffer, 0, completeMemoryCard, i * 128, 128);
                        backgroundWorker.ReportProgress(i);
                        i++;
                    }
                }
                else if (_hardInterface.CommMode == HardwareInterface.CommModes.psbios)
                {
                    //Get 128 byte chunk of BIOS
                    tempDataBuffer = _hardInterface.DumpPocketStationBIOS(i);

                    //Check if chunk was read properly
                    if (tempDataBuffer != null)
                    { 
                        Array.Copy(tempDataBuffer, 0, _pocketBIOS, i * 128, 128);
                        backgroundWorker.ReportProgress(i);
                        i++;
                    }
                }
                else
                {
                    //Unirom works with 2048 byte chunks
                    if (_hardInterface.Type == HardwareInterface.Types.unirom)
                    {
                        frameSize = 2048;
                        tempDataBuffer = new byte[2048];
                    }

                    Array.Copy(completeMemoryCard, i * frameSize, tempDataBuffer, 0, frameSize);

                    //Check if write was succesful
                    if (_hardInterface.WriteMemoryCardFrame(i, tempDataBuffer))
                    {
                        backgroundWorker.ReportProgress(i);
                        i++;
                    }
                }
            }

            _hardInterface.Stop();

            //Everything completed without interruption
            operationCompleted = true;
        }

        public EventHandler ReadingComplete;

        internal void RaiseReadingComplete()
        {
            if (this.ReadingComplete != null)
                this.ReadingComplete(this, EventArgs.Empty);
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            //Close dialog either by canceling background worker or directly
            if (backgroundWorker.IsBusy) backgroundWorker.CancelAsync();
            else this.Close();
        }

        private void cardReaderWindow_Load(object sender, EventArgs e)
        {
            //Set proper number of frames to read/write
            if(_hardInterface.CommMode == HardwareInterface.CommModes.format && _quickFormat)
                _hardInterface.FrameCount = 64;
            else if(_hardInterface.CommMode == HardwareInterface.CommModes.psbios)
                _hardInterface.FrameCount = 128;
            else _hardInterface.FrameCount = 1024;

            abortButton.Enabled = false;

            deviceLabel.Text = "Detecting " + _hardInterface.Name() + " on " + _comPort;

            //Unirom requires card checksum and has less data frames because of bigger frame size
            if (_hardInterface.Type == HardwareInterface.Types.unirom)
            {
                _hardInterface.LastChecksum = _hardInterface.CalculateChecksum(completeMemoryCard);
                if (_hardInterface.CommMode != (int)HardwareInterface.CommModes.read)
                {
                    _hardInterface.FrameCount /= 16;
                }
            }

            //Set up progress bar
            progressBar.Minimum = 0;
            progressBar.Maximum = _hardInterface.FrameCount;

            progressBar.Value = 0;

            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 30;

            //Start background reader/writer service
            backgroundWorker.RunWorkerAsync();
        }
    }
}
