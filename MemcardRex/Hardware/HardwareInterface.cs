using System;

namespace MemcardRex
{
	public class HardwareInterface
	{
		private int _type;
		private int _mode;
		private int _commMode;
		private int _cardSlot;
		private int _frameCount = 1024;		//Default number of frames on a standard Memory Card
        private UInt32 _lastChecksum;
		private bool _storedInRam;

        /// <summary>
        /// All supported interface types
        /// </summary>
        public enum Types : int
		{
			dexdrive,
			memcarduino,
			ps1cardlink,
			unirom,
			ps3mca
		};

		/// <summary>
		/// Supported operation modes
		/// </summary>
		public enum Modes : int
		{
			serial,
			tcp
		};

		/// <summary>
		/// Supported communication modes
		/// </summary>
		public enum CommModes : int
		{
			read,
			write,
			format
		}

        public int Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public int CommMode
        {
            get { return _commMode; }
            set { _commMode = value; }
        }

		public int CardSlot
		{
			get { return _cardSlot; }
			set { _cardSlot = value; }
		}

		public int FrameCount
		{
			get { return _frameCount; }
			set { _frameCount = value; }
		}

        public UInt32 LastChecksum
        {
            get { return _lastChecksum; }
            set { _lastChecksum = value; }
        }

		public int Type
		{
			get { return _type; }
			set { _type = value; }
		}

        public bool StoredInRam
        {
            get { return _storedInRam; }
			set { _storedInRam = value; }
        }

        public HardwareInterface(int mode, int commMode)
		{
			_mode = mode;
			_commMode = commMode;
			_type = -1;
		}

        public UInt32 CalculateChecksum(byte[] inBytes)
        {
            UInt32 returnVal = 0;
            for (int i = 0; i < inBytes.Length; i++)
            {
                returnVal += (UInt32)inBytes[i];
            }
            return returnVal;
        }

        /// <summary>
        /// Init hardware interface
        /// </summary>
        /// <param name="port">Serial or TCP port</param>
        /// <param name="speed">Link speed</param>
        /// <returns>Returns error message or null on success</returns>
        public virtual string Start(string port, int speed)
		{
			return "This interface is yet not supported";
		}

		/// <summary>
		/// Cleanly stop the interface
		/// </summary>
		public virtual void Stop()
		{

		}

		/// <summary>
		/// Return name of a device
		/// </summary>
		/// <returns></returns>
		public virtual string Name()
		{
			return "Dummy interface";
		}

		/// <summary>
		/// Return firmware version of a device
		/// </summary>
		/// <returns></returns>
		public virtual string Firmware()
		{
			return "";
		}

		/// <summary>
		/// Read a single 128 byte frame from a device
		/// </summary>
		/// <param name="FrameNumber">Frame to read</param>
		/// <returns></returns>
        public virtual byte[] ReadMemoryCardFrame(ushort FrameNumber)
		{
			return null;
		}

		/// <summary>
		/// Write a single 128 byte frame to a device
		/// </summary>
		/// <param name="FrameNumber">Frame to write</param>
		/// <param name="FrameData">Data to write</param>
		/// <returns>Returns success of the operation</returns>
        public virtual bool WriteMemoryCardFrame(ushort FrameNumber, byte[] FrameData)
		{
			return false;
		}
    }
}

