using System;
using Foundation;
using AppKit;
using System.ComponentModel;

namespace MemcardRex.macOS
{
	public partial class HardwareDialogController : NSViewController
	{
        #region Private Variables
        private NSViewController _presentor;
        private HardwareInterface _hardInterface;
        private bool _quickFormat;
        #endregion

        public NSViewController Presentor
        {
            get { return _presentor; }
            set { _presentor = value; }
        }

        public HardwareInterface HardInterface
        {
            get { return _hardInterface; }
            set { _hardInterface = value; }
        }

        public bool QuickFormat
        {
            set { _quickFormat = value; }
        }

        BackgroundWorker backgroundWorker = new BackgroundWorker();

        //Complete Memory Card data
        byte[] completeMemoryCard = new byte[131072];

        public byte[] MemoryCard
        {
            get { return completeMemoryCard; }
            set { Array.Copy(value, completeMemoryCard, value.Length); }
        }

        //Reading status flag
        bool operationCompleted = false;

        #region Override Methods
        public override void ViewWillAppear()
        {
            base.ViewWillAppear();

            //Set up events for background worker
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;

            this.View.Window.Title = _hardInterface.Name() + " communication";

            string interfaceDescription = _hardInterface.Name();

            if (_hardInterface.Firmware() != "") interfaceDescription += " (ver. " + _hardInterface.Firmware() + ")...";
            else interfaceDescription += "...";

            //Write description based on the current mode
            switch (_hardInterface.CommMode)
            {
                case (int)HardwareInterface.CommModes.read:
                    if(_hardInterface.Type == (int)HardwareInterface.Types.unirom)
                    {
                        //Unirom reading mode is special, we have to wait for it to store contents to RAM
                        deviceLabel.StringValue = "Waiting for Unirom to store contents in RAM.\nTransfer will start after all the sectors have been read.";
                        progressBar.AlphaValue = 0.0f;
                        abortButton.Enabled = false;
                    }
                    else
                    {
                        deviceLabel.StringValue = "Reading data from " + interfaceDescription;
                    }
                    break;

                case (int)HardwareInterface.CommModes.format:
                    deviceLabel.StringValue = "Formatting card on " + interfaceDescription;
                    if (_quickFormat) _hardInterface.FrameCount = 64;
                    break;

                case (int)HardwareInterface.CommModes.write:
                    deviceLabel.StringValue = "Writing data to " + interfaceDescription;
                    break;
            }

            //Unirom requires card checksum and has less data frames because of bigger frame size
            if (_hardInterface.Type == (int) HardwareInterface.Types.unirom)
            {
                _hardInterface.LastChecksum = _hardInterface.CalculateChecksum(completeMemoryCard);
                if(_hardInterface.CommMode != (int)HardwareInterface.CommModes.read)
                {
                    _hardInterface.FrameCount /= 16;
                }
            }

            //Disable resizing of modal dialog
            this.View.Window.StyleMask &= ~NSWindowStyle.Resizable;

            //Set up progress bar
            progressBar.MinValue = 0;
            progressBar.MaxValue = _hardInterface.FrameCount;

            progressBar.DoubleValue = 0;

            //Start background reader/writer service
            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_hardInterface.CommMode == (int)HardwareInterface.CommModes.read && operationCompleted)
                RaiseReadingComplete();

            //Close dialog
            Presentor.DismissViewController(this);
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (_hardInterface.Type == (int)HardwareInterface.Types.unirom && _hardInterface.CommMode == (int)HardwareInterface.CommModes.read)
            {
                if (progressBar.AlphaValue == 0.0f && _hardInterface.StoredInRam)
                {
                    progressBar.AlphaValue = 1.0f;
                    abortButton.Enabled = true;
                    deviceLabel.StringValue = "Reading data from Unirom...";
                }
            }

            progressBar.DoubleValue = e.ProgressPercentage;
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
                if(_hardInterface.CommMode == (int) HardwareInterface.CommModes.read)
                {
                    //Get 128 byte frame data from hardware device
                    tempDataBuffer = HardInterface.ReadMemoryCardFrame(i);

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
                    if (HardInterface.Type == (int) HardwareInterface.Types.unirom)
                    {
                        frameSize = 2048;
                        tempDataBuffer = new byte[2048];
                    }

                    Array.Copy(completeMemoryCard, i * frameSize, tempDataBuffer, 0, frameSize);

                    //Check if write was succesful
                    if (HardInterface.WriteMemoryCardFrame(i, tempDataBuffer))
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
        #endregion

        public EventHandler ReadingComplete;

        internal void RaiseReadingComplete()
        {
            if (this.ReadingComplete != null)
                this.ReadingComplete(this, EventArgs.Empty);
        }

        public HardwareDialogController (IntPtr handle) : base (handle)
		{
		}

        [Export("abortDialog:")]
        void AbortDialog(NSObject sender)
        {
            //Close dialog either by canceling background worker or directly
            if (backgroundWorker.IsBusy) backgroundWorker.CancelAsync();
            else Presentor.DismissViewController(this);
        }
    }
}
