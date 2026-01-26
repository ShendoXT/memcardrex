using System;
using System.Collections.Generic;

namespace MemcardRex.Core
{
    public struct HardInterfaces
    {
        public HardwareInterface hardwareInterface;
        public HardwareInterface.Modes mode;
        public string displayName;
    }

	public class HardwareSetup{
        //Registered hardware interfaces
    	public List<HardInterfaces> registeredInterfaces = new List<HardInterfaces>();
	
	    //Register hardware interfaces
        public void RegisterInterface(HardwareInterface hardInterface, HardwareInterface.Modes mode)
        {
            HardInterfaces regInterface = new HardInterfaces();
            regInterface.hardwareInterface = hardInterface;
            regInterface.mode = mode;
            regInterface.displayName = hardInterface.Name();

            //Append via TCP if interface mode is tcp
            if(mode == HardwareInterface.Modes.tcp) regInterface.displayName += " via TCP";

            registeredInterfaces.Add(regInterface);
        }
        
        public void AttachInterface(HardwareInterface hardInterface)
        {
            //Serial always available
            RegisterInterface(hardInterface, HardwareInterface.Modes.serial);

            //Check if interface supports TCP mode
            if((hardInterface.Features() & HardwareInterface.SupportedFeatures.TcpMode) > 0)
                RegisterInterface(hardInterface, HardwareInterface.Modes.tcp);
        }

		//List names of all available hardware interfaces
		public string[] GetAllInterfaceNames(){
			string[] names = new string[registeredInterfaces.Count];

			for (int i = 0; i < registeredInterfaces.Count; i++)
			{
				names[i] = registeredInterfaces[i].displayName;
			}

			return names;
		}
	}

	public class HardwareInterface
	{
		private Types _type;
		private Modes _mode;
		private CommModes _commMode;
		private int _cardSlot;
		private int _frameCount = 1024;		//Default number of frames on a standard Memory Card
        private UInt32 _lastChecksum;
		private bool _storedInRam;
		private const string pocketstationError = "PocketStation commands are not supported by this interface";

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
			format,
			realtime,
			psinfo,
			psbios,
			pstime
		}

		/// <summary>
		/// Features supported by the 
		/// </summary>
		public enum SupportedFeatures : int
		{
			TcpMode = 1,				//Interface supports TCP protocol
			RealtimeMode = 1 << 1,		//Fast reading of directories, updating in realtime
			PocketStation = 1 << 2		//Interface supports custom pocketstation commands
		}

        public Modes Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public CommModes CommMode
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

		public Types Type
		{
			get { return _type; }
			set { _type = value; }
		}

        public bool StoredInRam
        {
            get { return _storedInRam; }
			set { _storedInRam = value; }
        }

        public HardwareInterface()
		{
			//Set default values
			_mode = Modes.serial;
			_commMode = CommModes.read;
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
		/// Return all supported features of this interface
		/// </summary>
		/// <returns></returns>
		public virtual SupportedFeatures Features()
		{
			return 0;
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

        /// <summary>
        /// Read serial from PocketStation
        /// </summary>
        /// <param name="errorMsg">Descriptive error message</param>
        /// <returns>Serial as BCD</returns>
        public virtual UInt32 ReadPocketStationSerial(out string errorMsg)
		{
			errorMsg = pocketstationError;
			return 0;
		}

        /// <summary>
        /// Dump 16KB BIOS from PocketStation
        /// </summary>
        /// <param name="part">Part of the BIOS to dump</param>
        /// <returns>16KB BIOS data</returns>
        public virtual byte[] DumpPocketStationBIOS(int part)
		{
			return null;
		}

		/// <summary>
		/// Push current date and time from a PC to PocketStation
		/// </summary>
		/// <param name="errorMsg">Descriptive error message</param>
		/// <returns>Operation success</returns>
		public virtual bool SetPocketStationTime(out string errorMsg)
		{
			errorMsg = pocketstationError;
			return false;
		}
    }
}
