//Unirom communication class
//Shendo 2023
//Based on commands from NOTPSXSerial by JonathanDotCel

using System;
using System.IO.Ports;
using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace MemcardRex.Core
{
    public class Unirom : HardwareInterface
    {
        //Unirom serial port
        private SerialPort OpenedPort = null;

        //Unirom TCP comm
        TcpClient OpenTcpClient = null;
        NetworkStream TcpStream = null;

        private bool firstRun = true;

        //Name of the interface
        string InterfaceName = "Unirom";

        public override string Name()
        {
            return InterfaceName;
        }

        public override SupportedFeatures Features()
        {
            return SupportedFeatures.TcpMode;
        }

        //Get 4 byte array from Uint32 value
        private byte[] byteFromUint32(UInt32 value)
        {
            byte[] arr = new byte[4];

            arr[0] = (byte)value;
            arr[1] = (byte)(value << 8);
            arr[2] = (byte)(value << 16);
            arr[3] = (byte)(value << 24);

            return arr;
        }

        //Get Uint32 value from 4 byte array
        private UInt32 Uint32FromByte(byte[] arr)
        {
            UInt32 value = (UInt32)arr[0];
            value += ((UInt32)arr[1] << 8);
            value += ((UInt32)arr[2] << 16);
            value += ((UInt32)arr[3] << 24);

            return value;
        }

        //Clear input buffer
        private void FlushInput()
        {
            if(base.Mode == (int) Modes.serial)
                OpenedPort.DiscardInBuffer();
            else
                PortReadExisting();
        }

        //Write string data to opened port
        private void PortWrite(string message)
        {
            if (base.Mode == (int)Modes.serial)
                OpenedPort.Write(message);
            else
            {
                char[] msgData = message.ToCharArray();

                for (int i = 0; i < msgData.Length; i++)
                {
                    TcpStream.WriteByte((byte)msgData[i]);
                }
            }
        }

        //Write binary data to opened port
        private void PortWrite(byte[] buffer, int offset, int count)
        {
            if (base.Mode == (int)Modes.serial)
                OpenedPort.Write(buffer, offset, count);
            else
                TcpStream.Write(buffer, offset, count);
        }

        //Read binary data from opened port
        private void PortRead(byte[] buffer, int offset, int count)
        {
            if (base.Mode == (int)Modes.serial)
                OpenedPort.Read(buffer, offset, count);
            else
                TcpStream.Read(buffer, offset, count);
        }

        //Read single byte from the opened port
        private int PortReadByte()
        {
            if (base.Mode == (int)Modes.serial)
                return OpenedPort.ReadByte();
            else
                return TcpStream.ReadByte();
        }

        //Read all available data from the port
        private string PortReadExisting()
        {
            if (base.Mode == (int)Modes.serial)
                return OpenedPort.ReadExisting();
            else
            {
                if (OpenTcpClient.Available < 1) return null;

                byte[] streamData = new byte[OpenTcpClient.Available];
                TcpStream.Read(streamData, 0, streamData.Length);
                return Encoding.UTF8.GetString(streamData);
            }
        }

        //Return number of bytes waiting in the input buffer
        private int PortBytesToRead()
        {
            if (base.Mode == (int)Modes.serial)
                return OpenedPort.BytesToRead;
            else
            {
                return OpenTcpClient.Available;
            }
        }

        //Return number of bytes waiting in the output buffer
        private int PortBytesToWrite()
        {
            if (base.Mode == (int)Modes.serial)
                return OpenedPort.BytesToWrite;
            else
            {
                return 0;   //We are lying here since .net doesn't support it
            }
        }

        //Communication init sequence
        private string InitUnirom(string ComPortName)
        {
            int timeout = 100;

            //Start clean
            FlushInput();

            if(base.CommMode == (int) CommModes.read)
            {
                PortWrite("MCDN");
                Thread.Sleep(500);

                //Check if this is Unirom
                if (!"MCDNOKV2".Equals(PortReadExisting()))
                    return "Unirom was not detected on '" + ComPortName + "' port.";
            }
            else
            {
                PortWrite("MCUP");
                Thread.Sleep(500);

                //Check if this is Unirom
                if (!"MCUPOKV2".Equals(PortReadExisting()))
                    return "Unirom was not detected on '" + ComPortName + "' port.";
            }

            //Switch to V2 protocol
            PortWrite("UPV2");

            //Wait for response from Unirom
            while (PortBytesToRead() < 4)
            {
                Thread.Sleep(10);
                if (timeout > 0) timeout--; else break;
            }

            if (!"OKAY".Equals(PortReadExisting()))
            {
                return "Unirom was not detected on '" + ComPortName + "' port.";
            }

            //Send slot we want to use
            PortWrite(byteFromUint32((UInt32)CardSlot), 0, 4);

            //Start transfer
            if (base.CommMode == (int)CommModes.read)
            {
                PortWrite("MCRD");
            }

            //Everything went well, Unirom is available to be used
            return null;
        }

        public override string Start(string ComPortName, int tcpPort)
        {
            //Start TCP mode if it's active
            if(base.Mode == Modes.tcp)
            {
                return StartUniromTCP(ComPortName, tcpPort);
            }

            //Define a port to open
            OpenedPort = new SerialPort(ComPortName, 115200, Parity.None, 8, StopBits.One);
            OpenedPort.ReadBufferSize = 4096;

            //Try to open a selected port (in case of an error return a descriptive string)
            try { OpenedPort.Open(); }
            catch (Exception e) { return e.Message; }

            return InitUnirom(ComPortName);
        }

        public string StartUniromTCP(string remoteAddress, int remotePort)
        {
            //Try to set up comm link
            try
            {
                OpenTcpClient = new TcpClient(remoteAddress, remotePort);
                OpenTcpClient.ReceiveBufferSize = 4096;
                OpenTcpClient.SendBufferSize = 4096;

                TcpStream = OpenTcpClient.GetStream();
            }
            catch (Exception e)
            { return e.Message; }

            return InitUnirom(remoteAddress + ":" + remotePort.ToString());
        }

        public override byte[] ReadMemoryCardFrame(ushort frameNumber)
        {
            byte[] tempData = new byte[16];
            int timeout = 100;

            if (!StoredInRam)
            {
                //If data is still not being ready wait for the next cycle
                if (PortBytesToRead() < 12)
                {
                    Thread.Sleep(100);
                    return null;
                }

                byte[] respData = new byte[4];
                byte[] addressData = new byte[4];
                byte[] sizeData = new byte[4];

                //MCRD response
                PortRead(respData, 0, 4);

                //Address in RAM
                PortRead(addressData, 0, 4);

                //Size of data
                PortRead(sizeData, 0, 4);

                //Unirom finished storing data to RAM
                StoredInRam = true;

                Thread.Sleep(10);

                //Start dumping data
                PortWrite("DUMP");

                //Wait for response
                while (PortBytesToRead() < 16)
                {
                    Thread.Sleep(10);
                }

                //Handshake
                PortRead(tempData, 0, 16);

                //Send address in RAM
                PortWrite(addressData, 0, 4);
                PortWrite(sizeData, 0, 4);

                //Dumping will begin on the next cycle
                return null;
            }

            byte[] frameData = new byte[128];

            //Unirom requires more data request every 2048 bytes, or every 16 frames
            if (frameNumber != 0 && frameNumber % 16 == 0)
            {
                PortWrite("MORE");
            }

            //Timeout if there is no data for 1000ms
            timeout = 100;
            while (PortBytesToRead() < 128)
            {
                Thread.Sleep(10);
                if (timeout > 0) timeout--; else return null;
            }

            //Read frame data
            PortRead(frameData, 0, 128);

            //If this was the last frame fetch checksum
            if (frameNumber == 1023)
            {
                PortWrite("MORE");
                while (PortBytesToRead() < 4) ;
                PortRead(tempData, 0, 4);

                LastChecksum = Uint32FromByte(tempData);
            }

            return frameData;
        }

        public override bool WriteMemoryCardFrame(ushort FrameNumber, byte[] FrameData)
        {
            if (firstRun)
            {
                //Number of bytes to write
                PortWrite(BitConverter.GetBytes(FrameCount * 2048), 0, 4);

                //Checksum
                PortWrite(BitConverter.GetBytes(LastChecksum), 0, 4);

                LastChecksum = 0;
                firstRun = false;
            }

            //Unirom uses 2048 byte chunks to write data
            return WriteMemoryCardChunk(FrameData);
        }

        public bool WriteMemoryCardChunk(byte[] ChunkData)
        {
            ulong chunkChecksum = 0;

            PortWrite(ChunkData, 0, 2048);

            for (int j = 0; j < 2048; j++)
            {
                chunkChecksum += ChunkData[j];
            }

            while (PortBytesToWrite() != 0)
                Thread.Sleep(1);

            string cmdBuffer = "";
            while (cmdBuffer != "CHEK")
            {
                if (PortBytesToRead() != 0)
                {

                    cmdBuffer += (char)PortReadByte();

                }
                while (cmdBuffer.Length > 4)
                    cmdBuffer.Remove(0, 1);
            }

            if (cmdBuffer == "CHEK")
            {
                PortWrite(BitConverter.GetBytes(chunkChecksum), 0, 4);
                Thread.Sleep(1);

                while (cmdBuffer != "MORE" && cmdBuffer != "ERR!")
                {

                    if (PortBytesToRead() != 0)
                    {
                        char readVal = (char)PortReadByte();
                        cmdBuffer += readVal;
                    }
                    while (cmdBuffer.Length > 4)
                    {
                        cmdBuffer = cmdBuffer.Remove(0, 1);
                    }

                }

                if (cmdBuffer == "ERR!")
                {
                    //Retry this chunk
                    return false;
                }

            }

            return true;
        }

        //Cleanly close Unirom communication
        public override void Stop()
        {
            if (base.Mode == (int)Modes.serial)
                OpenedPort.Close();
            else
            {
                if (TcpStream != null) TcpStream.Close();
                if (TcpStream != null) OpenTcpClient.Close();
            }
        }

        public Unirom() : base()
        {
            Type = Types.unirom;
        }
    }
}