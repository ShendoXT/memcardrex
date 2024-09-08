//MemCARDuino communication class
//Shendo 2013 - 2024

using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Linq;

namespace MemcardRex
{
	public class MemCARDuino : HardwareInterface
	{
        enum MCinoCommands { GETID = 0xA0, GETVER = 0xA1, MCR = 0xA2, MCW = 0xA3, PSINFO = 0xB0, PSBIOS = 0xB1, PSTIME = 0xB2};
        enum MCinoResponses { ERROR = 0xE0, GOOD = 0x47, BADCHECKSUM = 0x4E, BADSECTOR = 0xFF };

        //MCino communication port
        SerialPort OpenedPort = null;

        //Name of the interface
        string InterfaceName = "MemCARDuino";

        //Contains a firmware version of a detected device
        byte FirmwareVersion = 0;

        //Features
        private const byte PocketCommandsMin = 0x08;        //Minimum version that supports PocketStation commands

        //Error messages
        private const string PocketUnsupported = "Please update MemCARDuino to use PocketStation commands";
        private const string PocketNotFound = "PocketStation not detected on MemCARDuino";

        public override string Name()
        {
            return InterfaceName;
        }

        public override string Firmware()
        {
            return (FirmwareVersion >> 4).ToString() + "." + (FirmwareVersion & 0xF).ToString();
        }

        public override SupportedFeatures Features()
        {
            return SupportedFeatures.RealtimeMode | SupportedFeatures.PocketStation;
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
            ReadData = ReadDataFromPort(256);

            if (!"MCDINO".Equals(Encoding.UTF8.GetString(ReadData, 0, 6)))
            {
                //Maybe this is Arduino Leonardo or Micro
                OpenedPort.DtrEnable = true;

                //Repeat GETID command
                SendDataToPort((byte)MCinoCommands.GETID, 100);
                ReadData = ReadDataFromPort(256);

                //If there is still no response this is definitely not MemCARDuino
                if (!"MCDINO".Equals(Encoding.UTF8.GetString(ReadData, 0, 6)))
                    return "MemCARDuino was not detected on '" + ComPortName + "' port.";
            }

            //Get the firmware version
            SendDataToPort((byte)MCinoCommands.GETVER, 30);
            ReadData = ReadDataFromPort(256);

            FirmwareVersion = ReadData[0];

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
        private byte[] ReadDataFromPort(int count)
        {
            //Buffer for reading data
            byte[] InputStream = new byte[count];

            //Read data from MCino
            if (OpenedPort.BytesToRead != 0) OpenedPort.Read(InputStream, 0, count);

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

            ReadData = ReadDataFromPort(256);

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
            ReadData = ReadDataFromPort(256);

            if (ReadData[0x0] == (byte)MCinoResponses.GOOD) return true;

            //Data was not written sucessfully
            return false;
        }

        //Read info from PocketStation and report serial number
        public override UInt32 ReadPocketStationSerial(out string errorMsg)
        {
            int DelayCounter = 0;

            //Buffer for storing read data from MCino
            byte[] ReadData = null;

            //Check if PocketStation commands are supported by the interface
            if (FirmwareVersion < PocketCommandsMin) {
                errorMsg = PocketUnsupported;
                return 0;
            };

            OpenedPort.DiscardInBuffer();

            SendDataToPort((byte)MCinoCommands.PSINFO, 0);

            //Wait for the buffer to fill
            while (OpenedPort.BytesToRead < 0x12 && DelayCounter < 18)
            {
                Thread.Sleep(5);
                DelayCounter++;
            }

            ReadData = ReadDataFromPort(256);

            //Check if PocketStation is connected to interface
            if (ReadData[0] != 0x12)
            {
                errorMsg = PocketNotFound;
                return 0;
            }

            //All good, return data
            errorMsg = null;
            return (UInt32) (ReadData[7] | ReadData[8] << 8 | ReadData[9] << 16 | ReadData[10] << 24);
        }

        //Dump 128 byte chunks of BIOS from PocketStation
        public override byte[] DumpPocketStationBIOS(int part)
        {
            int DelayCounter = 0;
            byte[] ReadData;
            byte[] StatusResp;

            //Check if PocketStation commands are supported by the interface
            if (FirmwareVersion < PocketCommandsMin)
            {
                return null;
            };

            Thread.Sleep(10);

            OpenedPort.DiscardInBuffer();

            SendDataToPort((byte)MCinoCommands.PSBIOS, 0);
            SendDataToPort((byte)part, 0);       //Part of the bios 1-128

            //Wait for response
            while (OpenedPort.BytesToRead < 2 && DelayCounter < 18)
            {
                Thread.Sleep(5);
                DelayCounter++;
            }

            ReadData = ReadDataFromPort(2);

            //Check if proper responses were returned
            if(ReadData[0] != 0x5 && ReadData[1] != 0x80)
            {
                return null;
            }

            //Wait for buffer to fill
            while (OpenedPort.BytesToRead < 129 && DelayCounter < 18)
            {
                Thread.Sleep(5);
                DelayCounter++;
            }

            ReadData = ReadDataFromPort(128);
            StatusResp = ReadDataFromPort(128);

            if (StatusResp[0] != (byte)MCinoResponses.GOOD) return null;

            return ReadData;
        }

        //Get a 2 digit BCD value from an int
        private byte getBCD(int value)
        {
            int tens = value / 10;
            int single = value - (tens * 10);

            return (byte)((tens << 4) | single);
        }


        public override bool SetPocketStationTime(out string errorMsg)
        {
            int DelayCounter = 0;
            byte[] ReadData;

            //Check if PocketStation commands are supported by the interface
            if (FirmwareVersion < PocketCommandsMin)
            {
                errorMsg = PocketUnsupported;
                return false;
            };

            OpenedPort.DiscardInBuffer();

            SendDataToPort((byte)MCinoCommands.PSTIME, 0);

            //Wait for response
            while (OpenedPort.BytesToRead < 2 && DelayCounter < 18)
            {
                Thread.Sleep(5);
                DelayCounter++;
            }

            ReadData = ReadDataFromPort(2);

            //Check if PocketStation is connected to interface
            if (ReadData[0] != 0x0 && ReadData[01] != 0x08)
            {
                errorMsg = PocketNotFound;
                return false;
            }

            //Send time data
            SendDataToPort(getBCD(DateTime.Now.Day), 0);        //Day
            SendDataToPort(getBCD(DateTime.Now.Month), 0);      //Month
            SendDataToPort(getBCD(DateTime.Now.Year % 100), 0); //Year
            SendDataToPort(getBCD(DateTime.Now.Year / 100), 0); //Century

            SendDataToPort(getBCD(DateTime.Now.Second), 0);     //Second
            SendDataToPort(getBCD(DateTime.Now.Minute), 0);     //Minute
            SendDataToPort(getBCD(DateTime.Now.Hour), 0);       //Hour
            SendDataToPort(getBCD(((int)DateTime.Now.DayOfWeek) + 1), 0);     //Day of week

            errorMsg = null;
            return true;
        }

        public MemCARDuino() : base()
		{
            Type = Types.memcarduino;
        }
	}
}

