﻿using System;

namespace MemcardRex
{
	public class HardwareInterface
	{
		//private int _port;
		private int _mode;
		private int _commMode;
		//private int _speed;

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

        public HardwareInterface(int mode, int commMode)
		{
			_mode = mode;
			_commMode = commMode;
		}

		/// <summary>
		/// Init hardware interface
		/// </summary>
		/// <param name="port">Serial or TCP port</param>
		/// <param name="speed">Link speed</param>
		/// <returns>Returns error message or null on success</returns>
		public virtual string Start(string port, int speed)
		{
			return null;
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
			return "";
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
			return true;
		}
    }
}

