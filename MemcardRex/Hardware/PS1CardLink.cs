//PS1CardLink communication class
//Shendo 2013 - 2023
//lmiori92 2021: added support for TCP communication (e.g. Serial Wifi Bridge)

using System;
using System.IO.Ports;
using System.Threading;
using System.Net.Sockets;

namespace MemcardRex
{
	public class PS1CardLink : HardwareInterface
	{
        enum PS1CLnkCommands { GETID = 0xA0, GETVER = 0xA1, MCR = 0xA2, MCW = 0xA3 };
        enum PS1CLnkResponses { ERROR = 0xE0, GOOD = 0x47, BADCHECKSUM = 0x4E, BADSECTOR = 0xFF };

        //PS1CLnk communication port
        SerialPort OpenedPort = null;
        TcpClient OpenTcpClient = null;
        NetworkStream TcpStream = null;

        //Protocol setup
        int MaxTimeout5msTickCount = 18;

        //Name of the interface
        string InterfaceName = "PS1CardLink";

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
            //Check if PS1CardLink needs to start in TCP mode
            if(base.Mode == (int)HardwareInterface.Modes.tcp)
            {
                //Comport name used as address
                //Com port speed used as TCP port
                return StartPS1CardLinkTCP(ComPortName, ComPortSpeed);
            }

            int PortBaudrate = 115200;
            if (ComPortSpeed == 1) PortBaudrate = 38400;

            //Setup the protocol parameters
            MaxTimeout5msTickCount = 18;  /* 5ms * 18 = 90ms */

            //Define a port to open
            OpenedPort = new SerialPort(ComPortName, PortBaudrate, Parity.None, 8, StopBits.One);
            OpenedPort.ReadBufferSize = 256;

            //Try to open a selected port (in case of an error return a descriptive string)
            try { OpenedPort.Open(); }
            catch (Exception e) { return e.Message; }

            // Initial handshake
            return InitializePS1CardLink(ComPortName);
        }

        // This constructor is used to setup a Serial Port over TCP/IP with PS1Link
        public string StartPS1CardLinkTCP(string Address, int Port)
        {
            //Setup the protocol parameters
            MaxTimeout5msTickCount = 400;  /* 5ms * 400 = 2000ms */

            // Try to setup the communication link
            try
            {
                OpenTcpClient = new TcpClient(Address, Port);
                TcpStream = OpenTcpClient.GetStream();
            }
            catch (Exception e)
            {
                return e.Message;
            }

            // Initial handshake
            return InitializePS1CardLink(Address + ":" + Port);
        }

        private string InitializePS1CardLink(string ComName)
        {
            //Buffer for storing read data from the PS1CLnk
            byte[] ReadData = null;
            int ReceivedBytes = 0;
            //Error string
            string ErrorString = "PS1CardLink was not detected on '" + ComName + "' port.";

            //Check if this is PS1CLnk
            SendDataToPort((byte)PS1CLnkCommands.GETID, 100);

            ReadData = ReadDataFromPort(ref ReceivedBytes);

            if (ReceivedBytes != 7 || ReadData[0] != 'P' || ReadData[1] != 'S' || ReadData[2] != '1' || ReadData[3] != 'C' || ReadData[4] != 'L' || ReadData[5] != 'N' || ReadData[6] != 'K')
            {
                return ErrorString;
            }

            //Get the software version
            SendDataToPort((byte)PS1CLnkCommands.GETVER, 30);
            ReadData = ReadDataFromPort(ref ReceivedBytes);

            if (ReceivedBytes != 1)
            {
                return ErrorString;
            }

            FirmwareVersion = (ReadData[0] >> 4).ToString() + "." + (ReadData[0] & 0xF).ToString();

            //Everything went well, PS1CLnk is ready to be used
            return null;
        }

        //Cleanly stop working with PS1CLnk
        public override void Stop()
        {
            if (OpenedPort != null)
            {
                if (OpenedPort.IsOpen == true) OpenedPort.Close();
            }
            else if (TcpStream != null)
            {
                TcpStream.Close();
                OpenTcpClient.Close();
            }
        }

        //Send PS1CLnk command on the opened COM port with a delay
        private void SendDataToPort(byte Command, int Delay)
        {
            //Clear everything in the input buffer
            FlushPort();

            //Send Command Byte
            SendDataToPort(new byte[] { Command }, 0, 1);

            //Wait for a required timeframe (for the PS1CLnk response)
            if (Delay > 0) Thread.Sleep(Delay);
        }

        // Send data to the communication port
        private void SendDataToPort(byte[] Data, int offset, int count)
        {
            if (OpenedPort != null)
            {
                OpenedPort.Write(Data, offset, count);
            }
            else if (TcpStream != null)
            {
                TcpStream.Write(Data, offset, count);
            }
            else
            {
                // Not initialized properly
            }
        }

        //Catch the response from a PS1CLnk
        private byte[] ReadDataFromPort(ref int BytesRead)
        {
            //Buffer for reading data
            byte[] InputStream = new byte[256];

            BytesRead = 0;

            //Read data from PS1CLnk
            if (OpenedPort != null)
            {
                if (OpenedPort.BytesToRead != 0) BytesRead = OpenedPort.Read(InputStream, 0, 256);
            }
            else if (OpenTcpClient != null)
            {
                if (OpenTcpClient.Available != 0)
                {
                    BytesRead = TcpStream.Read(InputStream, 0, 256);
                }
            }
            else
            {
                // Not initialized properly
            }

            return InputStream;
        }

        private int GetAvailableBytesFromPort()
        {
            if (OpenedPort != null)
            {
                return OpenedPort.BytesToRead;
            }
            else if (OpenTcpClient != null)
            {
                return OpenTcpClient.Available;
            }
            else
            {
                return 0;
            }
        }

        //Await a given number of bytes to be ready within a certain time
        private void AwaitAvailable(int BytesCount)
        {
            int DelayCounter = 0;
            int PreviousAvailableBytes = 0;

            while (GetAvailableBytesFromPort() < BytesCount && DelayCounter < MaxTimeout5msTickCount)
            {
                Thread.Sleep(5);
                if (PreviousAvailableBytes == GetAvailableBytesFromPort())
                {  //No data came within the timeframe, start counting the timeout
                    DelayCounter++;
                }
                else
                {  //Data came, we can reset the timeout counter
                    DelayCounter = 0;
                }
                PreviousAvailableBytes = GetAvailableBytesFromPort();
            }
        }

        //Discard all the bytes currently stored in the receive buffer of the trasport
        private void FlushPort()
        {
            if (OpenedPort != null)
            {
                OpenedPort.DiscardInBuffer();
            }
            else if (OpenTcpClient != null)
            {
                int ReceivedBytes = 0;
                ReadDataFromPort(ref ReceivedBytes);
            }
            else
            {
                // Not initialized properly
            }
        }

        //Read a specified frame of a Memory Card
        public override byte[] ReadMemoryCardFrame(ushort FrameNumber)
        {
            //Buffer for storing read data from PS1CLnk
            byte[] ReadData = null;
            int ReceivedBytes = 0;

            //128 byte frame data from a Memory Card
            byte[] ReturnDataBuffer = new byte[128];

            byte FrameLsb = (byte)(FrameNumber & 0xFF);     //Least significant byte
            byte FrameMsb = (byte)(FrameNumber >> 8);       //Most significant byte
            byte XorData = (byte)(FrameMsb ^ FrameLsb);     //XOR variable for consistency checking

            //Read a frame from the Memory Card
            SendDataToPort((byte)PS1CLnkCommands.MCR, 0);
            SendDataToPort(FrameMsb, 0);
            SendDataToPort(FrameLsb, 0);

            //Wait for the buffer to fill (or to timeout)
            AwaitAvailable(130);

            ReadData = ReadDataFromPort(ref ReceivedBytes);
            if (ReceivedBytes != 130)
            {  //Unexpected amount of data has been received, refuse to continue
                return null;
            }

            //Copy recieved data
            Array.Copy(ReadData, 0, ReturnDataBuffer, 0, 128);

            //Calculate XOR checksum
            for (int i = 0; i < 128; i++)
            {
                XorData ^= ReturnDataBuffer[i];
            }

            //Return null if there is a checksum missmatch
            if (XorData != ReadData[128] || ReadData[129] != (byte)PS1CLnkResponses.GOOD)
            {
                return null;
            }

            //Return read data
            return ReturnDataBuffer;
        }

        //Write a specified frame to a Memory Card
        public override bool WriteMemoryCardFrame(ushort FrameNumber, byte[] FrameData)
        {
            //Buffer for storing read data from PS1CLnk
            byte[] ReadData = null;

            byte FrameLsb = (byte)(FrameNumber & 0xFF);     //Least significant byte
            byte FrameMsb = (byte)(FrameNumber >> 8);       //Most significant byte
            byte XorData = (byte)(FrameMsb ^ FrameLsb);     //XOR variable for consistency checking

            //Calculate XOR checksum
            for (int i = 0; i < 128; i++)
            {
                XorData ^= FrameData[i];
            }

            FlushPort();

            //Write a frame to the Memory Card
            SendDataToPort((byte)PS1CLnkCommands.MCW, 0);
            SendDataToPort(FrameMsb, 0);
            SendDataToPort(FrameLsb, 0);
            SendDataToPort(FrameData, 0, 128);
            SendDataToPort(XorData, 0);                      //XOR Checksum

            //Wait for the buffer to fill (or to timeout)
            AwaitAvailable(1);

            //Fetch PS1CLnk's response to the last command
            int ReceivedBytes = 0;
            ReadData = ReadDataFromPort(ref ReceivedBytes);

            if (ReceivedBytes == 1 && ReadData[0x0] == (byte)PS1CLnkResponses.GOOD) return true;

            //Data was not written sucessfully
            return false;
        }

        public PS1CardLink(int mode, int commMode) : base(mode, commMode)
        {
            Type = (int)Types.ps1cardlink;
        }
	}
}
