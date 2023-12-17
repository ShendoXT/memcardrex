//MemCARDuino communication class
//Shendo 2013 - 2023

using System;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace MemcardRex
{
	public class MemCARDuino : HardwareInterface
	{
        enum MCinoCommands { GETID = 0xA0, GETVER = 0xA1, MCR = 0xA2, MCW = 0xA3 };
        enum MCinoResponses { ERROR = 0xE0, GOOD = 0x47, BADCHECKSUM = 0x4E, BADSECTOR = 0xFF };

        //MCino communication port
        SerialPort OpenedPort = null;

        //Name of the interface
        string InterfaceName = "MemCARDuino";

        //Contains a firmware version of a detected device
        string FirmwareVersion = "0.0";

        public override string Name()
        {
            return InterfaceName;
        }

        public override string Firmware()
        {
            return FirmwareVersion;
        }

        public override string Start(string ComPortName, int ComPortSpeed)
        {
            int PortBaudrate = 115200;
            if (ComPortSpeed == 1) PortBaudrate = 38400;

            //Define a port to open
            OpenedPort = new SerialPort(ComPortName, PortBaudrate, Parity.None, 8, StopBits.One);
            OpenedPort.ReadBufferSize = 256;

            //Buffer for storing read data from the MCino
            byte[] ReadData = null;

            //Try to open a selected port (in case of an error return a descriptive string)
            try
            {
                OpenedPort.Open();
            }
            catch (Exception e) { return e.Message; }

            //DTR needs to be toggled to reset Arduino and set it to serial mode
            OpenedPort.DtrEnable = true;
            OpenedPort.DtrEnable = false;
            Thread.Sleep(2000);

            //Check if this is MCino
            SendDataToPort((byte)MCinoCommands.GETID, 100);
            ReadData = ReadDataFromPort();

            if (!"MCDINO".Equals(Encoding.UTF8.GetString(ReadData, 0, 6)))
            {
                //Maybe this is Arduino Leonardo or Micro
                OpenedPort.DtrEnable = true;

                //Repeat GETID command
                SendDataToPort((byte)MCinoCommands.GETID, 100);
                ReadData = ReadDataFromPort();

                //If there is still no response this is definitely not MemCARDuino
                if (!"MCDINO".Equals(Encoding.UTF8.GetString(ReadData, 0, 6)))
                    return "MemCARDuino was not detected on '" + ComPortName + "' port.";
            }

            //Get the firmware version
            SendDataToPort((byte)MCinoCommands.GETVER, 30);
            ReadData = ReadDataFromPort();

            FirmwareVersion = (ReadData[0] >> 4).ToString() + "." + (ReadData[0] & 0xF).ToString();

            //Everything went well, MCino is ready to be used
            return null;
        }

        //Cleanly stop working with MemCARDuino
        public override void Stop()
        {
            if (OpenedPort == null) return;
            if (OpenedPort.IsOpen == true) OpenedPort.Close();
        }

        //Send MCino command on the opened COM port with a delay
        private void SendDataToPort(byte Command, int Delay)
        {
            //Clear everything in the input buffer
            OpenedPort.DiscardInBuffer();

            //Send Command Byte
            OpenedPort.Write(new byte[] { Command }, 0, 1);

            //Wait for a required timeframe (for the MCino response)
            if (Delay > 0) Thread.Sleep(Delay);
        }

        //Catch the response from a MCino
        private byte[] ReadDataFromPort()
        {
            //Buffer for reading data
            byte[] InputStream = new byte[256];

            //Read data from MCino
            if (OpenedPort.BytesToRead != 0) OpenedPort.Read(InputStream, 0, 256);

            return InputStream;
        }

        //Read a specified frame of a Memory Card
        public override byte[] ReadMemoryCardFrame(ushort FrameNumber)
        {
            int DelayCounter = 0;

            //Buffer for storing read data from MCino
            byte[] ReadData = null;

            //128 byte frame data from a Memory Card
            byte[] ReturnDataBuffer = new byte[128];

            byte FrameLsb = (byte)(FrameNumber & 0xFF);     //Least significant byte
            byte FrameMsb = (byte)(FrameNumber >> 8);       //Most significant byte
            byte XorData = (byte)(FrameMsb ^ FrameLsb);     //XOR variable for consistency checking

            //Read a frame from the Memory Card
            SendDataToPort((byte)MCinoCommands.MCR, 0);
            SendDataToPort(FrameMsb, 0);
            SendDataToPort(FrameLsb, 0);

            //Wait for the buffer to fill
            while (OpenedPort.BytesToRead < 130 && DelayCounter < 18)
            {
                Thread.Sleep(5);
                DelayCounter++;
            }

            ReadData = ReadDataFromPort();

            //Copy recieved data
            Array.Copy(ReadData, 0, ReturnDataBuffer, 0, 128);

            //Calculate XOR checksum
            for (int i = 0; i < 128; i++)
            {
                XorData ^= ReturnDataBuffer[i];
            }

            //Return null if there is a checksum missmatch
            if (XorData != ReadData[128] || ReadData[129] != (byte)MCinoResponses.GOOD) return null;

            //Return read data
            return ReturnDataBuffer;
        }

        //Write a specified frame to a Memory Card
        public override bool WriteMemoryCardFrame(ushort FrameNumber, byte[] FrameData)
        {
            int DelayCounter = 0;

            //Buffer for storing read data from MCino
            byte[] ReadData = null;

            byte FrameLsb = (byte)(FrameNumber & 0xFF);     //Least significant byte
            byte FrameMsb = (byte)(FrameNumber >> 8);       //Most significant byte
            byte XorData = (byte)(FrameMsb ^ FrameLsb);     //XOR variable for consistency checking

            //Calculate XOR checksum
            for (int i = 0; i < 128; i++)
            {
                XorData ^= FrameData[i];
            }

            OpenedPort.DiscardInBuffer();

            //Write a frame to the Memory Card
            SendDataToPort((byte)MCinoCommands.MCW, 0);
            SendDataToPort(FrameMsb, 0);
            SendDataToPort(FrameLsb, 0);
            OpenedPort.Write(FrameData, 0, 128);
            SendDataToPort(XorData, 0);                      //XOR Checksum

            //Wait for the buffer to fill
            while (OpenedPort.BytesToRead < 1 && DelayCounter < 18)
            {
                Thread.Sleep(5);
                DelayCounter++;
            }

            //Fetch MCino's response to the last command
            ReadData = ReadDataFromPort();

            if (ReadData[0x0] == (byte)MCinoResponses.GOOD) return true;

            //Data was not written sucessfully
            return false;
        }

        public MemCARDuino(int mode, int commMode) : base(mode, commMode)
		{
            //_mode = mode;
            //_commMode = commMode;
        }
	}
}

