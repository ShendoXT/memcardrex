//Hardware card reading device information window
//Shendo 2012 - 2023

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MemcardRex
{
    public partial class cardReaderWindow : Form
    {
        private HardwareInterface _hardInterface;
        private bool _quickFormat;

        BackgroundWorker backgroundWorker = new BackgroundWorker();

        //Complete Memory Card data
        byte[] completeMemoryCard = new byte[131072];

        public byte[] MemoryCard
        {
            get { return completeMemoryCard; }
            set { Array.Copy(value, completeMemoryCard, value.Length); }
        }

        public bool QuickFormat
        {
            set { _quickFormat = value; }
        }

        //Reading status flag
        bool operationCompleted = false;

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
            if (_hardInterface.CommMode == (int)HardwareInterface.CommModes.read && operationCompleted)
                RaiseReadingComplete();

            //Close dialog
            this.Close();
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (_hardInterface.Type == (int)HardwareInterface.Types.unirom && _hardInterface.CommMode == (int)HardwareInterface.CommModes.read)
            {
                if (progressBar.Visible == false && _hardInterface.StoredInRam)
                {
                    progressBar.Visible = true;
                    abortButton.Enabled = true;
                    deviceLabel.Text = "Reading data from Unirom...";
                }
            }

            progressBar.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            byte[] tempDataBuffer = new byte[128];
            ushort i = 0;
            int frameSize = 128;

            //Process all frames of the Memory Card
            while (i < _hardInterface.FrameCount)
            {
                //Check if the "Abort" button has been pressed
                if (backgroundWorker.CancellationPending == true) return;

                //Are we reading or writing data
                if (_hardInterface.CommMode == (int)HardwareInterface.CommModes.read)
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
                else
                {
                    //Unirom works with 2048 byte chunks
                    if (_hardInterface.Type == (int)HardwareInterface.Types.unirom)
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
            string interfaceDescription = _hardInterface.Name();

            if (_hardInterface.Firmware() != "") interfaceDescription += " (ver. " + _hardInterface.Firmware() + ")...";
            else interfaceDescription += "...";

            //Write description based on the current mode
            switch (_hardInterface.CommMode)
            {
                case (int)HardwareInterface.CommModes.read:
                    if (_hardInterface.Type == (int)HardwareInterface.Types.unirom)
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

                case (int)HardwareInterface.CommModes.format:
                    deviceLabel.Text = "Formatting card on " + interfaceDescription;
                    if (_quickFormat) _hardInterface.FrameCount = 64;
                    break;

                case (int)HardwareInterface.CommModes.write:
                    deviceLabel.Text = "Writing data to " + interfaceDescription;
                    break;
            }

            //Unirom requires card checksum and has less data frames because of bigger frame size
            if (_hardInterface.Type == (int)HardwareInterface.Types.unirom)
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

            //Start background reader/writer service
            backgroundWorker.RunWorkerAsync();
        }
    }
}
