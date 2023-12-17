// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using AppKit;
using DeviceCheck;

namespace MemcardRex.macOS
{
	public partial class HardwareDialogController : NSViewController
	{
        #region Private Variables
        private NSViewController _presentor;
        private HardwareInterface _hardInterface;
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

        #region Override Methods
        public override void ViewWillAppear()
        {
            base.ViewWillAppear();

            this.View.Window.Title = _hardInterface.Name() + " communication";


            //Write description based on the current mode
            switch (_hardInterface.CommMode)
            {
                case (int)HardwareInterface.CommModes.read:
                    deviceLabel.StringValue = "Reading data from " + _hardInterface.Name() + " (ver. " + _hardInterface.Firmware() + ")...";
                    break;

                case (int)HardwareInterface.CommModes.format:
                case (int)HardwareInterface.CommModes.write:
                    deviceLabel.StringValue = "Writing data to " + _hardInterface.Name() + " (ver. " + _hardInterface.Firmware() + ")...";
                    break;
            }

            //Disable resizing of modal dialog
            this.View.Window.StyleMask &= ~NSWindowStyle.Resizable;
        }
        #endregion

        public HardwareDialogController (IntPtr handle) : base (handle)
		{
		}

        [Export("abortDialog:")]
        void AbortDialoga(NSObject sender)
        {
            Presentor.DismissViewController(this);
        }
    }
}
