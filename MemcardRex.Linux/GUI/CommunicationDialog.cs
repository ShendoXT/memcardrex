using Gtk;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MemcardRex.Core;

namespace MemcardRex.Linux;

public class CommunicationDialog
{
    private Builder _builder;
    private Dialog _dialog;
    private ProgressBar _progressBar;
    private Label _label;
    private Button _btnAbort;
    private bool _isPulsing = false;
    private bool firstRun = true;
    private bool abortWaiting = false;

    private HardwareInterface _hardInterface;
    private bool _quickFormat;
    private string _comPort = "";
    private string _remoteAddress = "";
    private int _remoteComPort;
    private string? _errorMessage = null;
    private UInt32 _pocketSerial = 0;
    private byte[] _pocketBIOS = new byte[16384];

    //Reading status flag
    private bool operationCompleted = false;

    //Complete Memory Card data
    byte[] completeMemoryCard = new byte[131072];

    public string? ErrorMessage
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

    public event EventHandler? OnComplete;

    public CommunicationDialog(Window parent, HardwareInterface hardInterface)
    {
        _builder = new Builder("MemcardRex.Linux.GUI.CommunicationDialog.ui")!;

        _dialog = (Dialog)_builder.GetObject("main_dialog")!;
        _progressBar = (ProgressBar)_builder.GetObject("progress_bar")!;
        _label = (Label)_builder.GetObject("lbl_status")!;
        _btnAbort = (Button)_builder.GetObject("btn_abort")!;

        _dialog.SetTransientFor(parent);

        _hardInterface = hardInterface!;

        //Set title based on the hardware interface name
        _dialog.SetTitle(hardInterface.Name() + " communication");

        _dialog.OnRealize += async (s, e) => 
        {
            //Set proper number of frames to read/write
            if(_hardInterface.CommMode == HardwareInterface.CommModes.format && _quickFormat)
                _hardInterface.FrameCount = 64;
            else if(_hardInterface.CommMode == HardwareInterface.CommModes.psbios)
                _hardInterface.FrameCount = 128;
            else _hardInterface.FrameCount = 1024;

            //Disable abort button before starting a device
            _btnAbort.SetSensitive(false);
            
            //Show detecting message
            string deviceLabel = "Detecting " + _hardInterface.Name();
            if(_hardInterface.Type != HardwareInterface.Types.ps3mca)
            {
                if(_hardInterface.Mode == HardwareInterface.Modes.tcp)
                    deviceLabel += " on " + _remoteAddress + ":" + _remoteComPort.ToString();
                else
                    deviceLabel += " on " + _comPort;
            }

            //Set status message
            _label.SetText(deviceLabel);

            //Unirom requires card checksum and has less data frames because of bigger frame size
            if (_hardInterface.Type == HardwareInterface.Types.unirom)
            {
                _hardInterface.LastChecksum = _hardInterface.CalculateChecksum(completeMemoryCard);
                if (_hardInterface.CommMode != HardwareInterface.CommModes.read)
                {
                    _hardInterface.FrameCount /= 16;
                }
            }

            //Pulse status bar
            StartPulsing();

            await Task.Run(async () => {
                    //Start the communication
                    await DoWork();
                });

        };

        //Abort
        _btnAbort.OnClicked += (s, e) => {
            _isPulsing = false;
            abortWaiting = true;
        };
    }

    public void Show() => _dialog.Show();
    public void Close() => _dialog.Destroy();

    //Operations complete, invoke on complete
    private void OpComplete(){
        GLib.Functions.IdleAdd(0, () => {
            if (_dialog.Handle != IntPtr.Zero) {
                OnComplete?.Invoke(this, EventArgs.Empty);
            }
            return false;
        });
    }

    private void ReportProgress(int value)
    {
        if (_hardInterface.Type == HardwareInterface.Types.unirom && _hardInterface.CommMode == HardwareInterface.CommModes.read)
        {
            /*if (progressBar.Visible == false && _hardInterface.StoredInRam)
            {
                progressBar.Visible = true;
                abortButton.Enabled = true;
                deviceLabel.Text = "Reading data from Unirom...";
            }*/
        }
        //First run
        else if (firstRun)
        {
            string interfaceDescription = _hardInterface.Name();

            if (_hardInterface.Firmware() != "") interfaceDescription += " (ver. " + _hardInterface.Firmware() + ")...";
            else interfaceDescription += "...";

            _btnAbort.SetSensitive(true);

            switch (_hardInterface.CommMode)
            {
                case HardwareInterface.CommModes.read:
                    if (_hardInterface.Type == HardwareInterface.Types.unirom)
                    {
                        //Unirom reading mode is special, we have to wait for it to store contents to RAM
                        _label.SetText("Waiting for Unirom to store contents in RAM.\nTransfer will start after all the sectors have been read.");
                        //progressBar.Visible = false;
                        _btnAbort.SetSensitive(false);
                    }
                    else
                    {
                        _label.SetText("Reading data from " + interfaceDescription);
                    }
                    break;

                case HardwareInterface.CommModes.format:
                    _label.SetText("Formatting card on " + interfaceDescription);
                    if (_quickFormat) _hardInterface.FrameCount = 64;
                    break;

                case HardwareInterface.CommModes.write:
                    _label.SetText("Writing data to " + interfaceDescription);
                    break;

                case HardwareInterface.CommModes.psbios:
                    _label.SetText("Dumping BIOS using " + interfaceDescription);
                    break;
            }
            firstRun = false;
            _isPulsing = false;
        }

        //Report progress to UI
        GLib.Functions.IdleAdd(0, () => {
            if (_dialog.Handle != IntPtr.Zero) {
                _progressBar.SetFraction(value / (_hardInterface.FrameCount + 0.0f));
            }
            return false;
        });
    }

    private async Task DoWork(){
        await Task.Yield();

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
                _errorMessage += "\n\nMake sure to select proper communication port in preferences dialog";

            OpComplete();
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
            OpComplete();
            return;
        }

        //Check if this is PocketStation time set
        if (_hardInterface.CommMode == HardwareInterface.CommModes.pstime)
        {
            _hardInterface.SetPocketStationTime(out _errorMessage);

            _hardInterface.Stop();
            OpComplete();
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
                OpComplete();
                return;
            }
        }

        //Process all frames of the Memory Card
        while (i < _hardInterface.FrameCount)
        {
            //Check if the "Abort" button has been pressed
            if (abortWaiting)
            {
                //Cleanly close the interface
                if (_hardInterface != null) _hardInterface.Stop();
                OpComplete();
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
                    ReportProgress(i);
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
                    ReportProgress(i);
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

                Array.Copy(completeMemoryCard, i * frameSize, tempDataBuffer!, 0, frameSize);

                //Check if write was succesful
                if (_hardInterface.WriteMemoryCardFrame(i, tempDataBuffer))
                {
                    ReportProgress(i);
                    i++;
                }
            }
        }

        _hardInterface.Stop();

        //Everything completed without interruption
        operationCompleted = true;

        OpComplete();
    }

    //Pulsing progress bar
    private async void StartPulsing()
    {
        if (_isPulsing) return;
        _isPulsing = true;

        while (_isPulsing)
        {
            if (_dialog.Handle == IntPtr.Zero) break;

            _progressBar.Pulse();
            await Task.Delay(100);
        }
    }
}