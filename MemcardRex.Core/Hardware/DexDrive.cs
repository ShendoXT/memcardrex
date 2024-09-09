//DexDrive communication class
//Based on the various sources around the internet
//Shendo 2012 - 2023

using System;
using System.IO.Ports;
using System.Threading;

namespace MemcardRex.Core
{
    public class DexDrive : HardwareInterface
    {
        enum DexCommands { INIT = 0x00, STATUS = 0x01, READ = 0x02, WRITE = 0x04, LIGHT = 0x07, MAGIC_HANDSHAKE = 0x27 };
        enum DexResponses { POUT = 0x20, ERROR = 0x21, NOCARD = 0x22, CARD = 0x23, WRITE_OK = 0x28, WRITE_SAME = 0x29, WAIT = 0x2A, ID = 0x40, DATA = 0x41 };

        //DexDrive communication port
        SerialPort OpenedPort = null;

        //Name of the interface
        string InterfaceName = "DexDrive";

        //Contains a firmware version of a detected device
        string FirmwareVersion = "0.0";

        //Init DexDrive (string returned if an error happened)
        public override string Start(string ComPortName, int dummy)
        {
            //Define a port to open
            OpenedPort = new SerialPort(ComPortName, 38400, Parity.None, 8, StopBits.One);
            OpenedPort.ReadBufferSize = 256;

            //Buffer for storing read data from the DexDrive
            byte[] ReadData = null;

            //Try to open a selected port (in case of an error return a descriptive string)
            try { OpenedPort.Open(); }
            catch (Exception e) { return e.Message; }

            //Dexdrive won't respond if RTS is not toggled on/off
            OpenedPort.RtsEnable = false;
            Thread.Sleep(300);
            OpenedPort.RtsEnable = true;
            Thread.Sleep(300);

            //DTR line is used for additional power
            OpenedPort.DtrEnable = true;

            //Check if DexDrive is attached to the port
            //Detection may fail 1st or 2nd time, so the command is sent 5 times
            for (int i = 0; i < 5; i++)
            {
                OpenedPort.DiscardInBuffer();
                OpenedPort.Write("XXXXX");
                Thread.Sleep(20);
            }

            //Check for "IAI" string
            ReadData = ReadDataFromPort();
            if (ReadData[0] != 0x49 || ReadData[1] != 0x41 || ReadData[2] != 0x49) return "DexDrive was not detected on '" + ComPortName + "' port.";

            //Wake DexDrive up (kick it from POUT mode)
            SendDataToPort((byte)DexCommands.INIT, new byte[] { 0x10, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0xAA, 0xBB, 0xCC, 0xDD }, 50);
            //SendDataToPort((byte)DexCommands.INIT, new byte[] { 0x10, 0x29, 0x23, 0xbe, 0x84, 0xe1, 0x6c, 0xd6, 0xae, 0x52, 0x90, 0x49, 0xf1, 0xf1, 0xbb, 0xe9, 0xeb }, 50);

            //Check for "PSX" string
            ReadData = ReadDataFromPort();
            if (ReadData[5] != 0x50 || ReadData[6] != 0x53 || ReadData[7] != 0x58) return "Detected device is not a PS1 DexDrive.";

            //Fetch the firmware version
            FirmwareVersion = (ReadData[8] >> 6).ToString() + "." + ((ReadData[8] >> 2) & 0xF).ToString() + (ReadData[8] & 0x3).ToString();

            //Send magic handshake signal 10 times
            for (int i = 0; i < 10; i++) SendDataToPort((byte)DexCommands.MAGIC_HANDSHAKE, null, 0);
            Thread.Sleep(50);

            //Turn on the status light
            SendDataToPort((byte)DexCommands.LIGHT, new byte[] { 1 }, 50);

            //Everything went well, DexDrive is ready to recieve commands
            return null;
        }

        //Cleanly stop working with DexDrive
        public override void Stop()
        {
            if (OpenedPort.IsOpen == true) OpenedPort.Close();
        }

        public override string Name()
        {
            return InterfaceName;
        }

        public override string Firmware()
        {
            return FirmwareVersion;
        }

        public override SupportedFeatures Features()
        {
            return SupportedFeatures.RealtimeMode;
        }

        //Send DexDrive command on the opened COM port with a delay
        private void SendDataToPort(byte Command, byte[] Data, int Delay)
        {
            //Clear everything in the input buffer
            OpenedPort.DiscardInBuffer();

            //Every command must begin with "IAI" string
            OpenedPort.Write("IAI" + (char)Command);
            if (Data != null) OpenedPort.Write(Data, 0, Data.Length);

            //Wait for a required timeframe (for the DexDrive response)
            if (Delay > 0) Thread.Sleep(Delay);
        }

        //Catch the response from a DexDrive
        private byte[] ReadDataFromPort()
        {
            //Buffer for reading data
            byte[] InputStream = new byte[256];

            //Read data from DexDrive
            if (OpenedPort.BytesToRead != 0) OpenedPort.Read(InputStream, 0, 256);

            return InputStream;
        }

        //Read a specified frame of a Memory Card
        public override byte[] ReadMemoryCardFrame(ushort FrameNumber)
        {
            //Buffer for storing read data from the DexDrive
            byte[] ReadData = null;

            //128 byte frame data from a Memory Card
            byte[] ReturnDataBuffer = new byte[128];

            int DelayCounter = 0;

            byte FrameLsb = (byte)(FrameNumber & 0xFF);     //Least significant byte
            byte FrameMsb = (byte)(FrameNumber >> 8);       //Most significant byte
            byte XorData = (byte)(FrameLsb ^ FrameMsb);     //XOR variable for consistency checking

            //Read a frame from the Memory Card
            SendDataToPort((byte)DexCommands.READ, new byte[] { FrameLsb, FrameMsb }, 0);

            //Wait for the buffer to fill
            while (OpenedPort.BytesToRead < 133 && DelayCounter < 16)
            {
                Thread.Sleep(5);
                DelayCounter++;
            }

            //Read Memory Card data
            ReadData = ReadDataFromPort();

            //Copy received data (filter IAI prefix)
            Array.Copy(ReadData, 4, ReturnDataBuffer, 0, 128);

            //Calculate XOR checksum
            for (int i = 0; i < 128; i++)
            {
                XorData ^= ReturnDataBuffer[i];
            }

            //Return null if there is a checksum missmatch
            if (XorData != ReadData[132]) return null;

            //Return read data
            return ReturnDataBuffer;
        }

        //Write a specified frame to a Memory Card
        public override bool WriteMemoryCardFrame(ushort FrameNumber, byte[] FrameData)
        {
            //Buffer for storing read data from the DexDrive
            byte[] ReadData = null;

            byte FrameLsb = (byte)(FrameNumber & 0xFF);                                 //Least significant byte
            byte FrameMsb = (byte)(FrameNumber >> 8);                                   //Most significant byte
            byte RevFrameLsb = ReverseByte(FrameLsb);                                   //Reversed least significant byte
            byte RevFrameMsb = ReverseByte(FrameMsb);                                   //Reversed most significant byte
            byte XorData = (byte)(FrameMsb ^ FrameLsb ^ RevFrameMsb ^ RevFrameLsb);     //XOR variable for consistency checking

            int DelayCounter = 0;

            //Calculate XOR checksum
            for (int i = 0; i < 128; i++)
            {
                XorData ^= FrameData[i];
            }

            //Write a frame to a Memory Card
            SendDataToPort((byte)DexCommands.WRITE, new byte[] { FrameMsb, FrameLsb, RevFrameMsb, RevFrameLsb }, 0);        //Frame number
            OpenedPort.Write(FrameData, 0, FrameData.Length);                                                               //Save data
            OpenedPort.Write(new byte[] { XorData }, 0, 1);                                                                 //XOR Checksum

            //Wait for the buffer to fill
            while (OpenedPort.BytesToRead < 4 && DelayCounter < 20)
            {
                Thread.Sleep(5);
                DelayCounter++;
            }

            //Fetch DexDrive's response to the last command
            ReadData = ReadDataFromPort();

            //Check the return status (return true if all went OK)
            if (ReadData[0x3] == (byte)DexResponses.WRITE_OK || ReadData[0x3] == (byte)DexResponses.WRITE_SAME) return true;

            //Data was not written sucessfully
            return false;
        }

        //Reverse order of bits in a byte
        byte ReverseByte(byte InputByte)
        {
            byte ReturnByte = new byte();

            int i = 0;
            int j = 7;

            while (i < 8)
            {
                if ((InputByte & (1 << i)) > 0) ReturnByte |= (byte)(1 << j);

                i++;
                j--;
            }

            //Return reversed byte
            return ReturnByte;
        }

        public DexDrive() : base()
        {
            Type = Types.dexdrive;
        }
    }
}