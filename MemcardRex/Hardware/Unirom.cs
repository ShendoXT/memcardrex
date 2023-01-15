//Unirom communication class
//Shendo 2023
//Based on commands from NOTPSXSerial by JonathanDotCel

using System;
using System.IO.Ports;
using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace UniromCommunication
{
    class Unirom
    {
        public enum Mode {
            Read,
            Write
        };

        enum ComMode { 
            Serial,
            TCP
        }

        //Unirom serial port
        private SerialPort OpenedPort = null;

        //Unirom TCP comm
        TcpClient OpenTcpClient = null;
        NetworkStream TcpStream = null;

        //Flag indicating if Unirom finished storing data to RAM on the PS1
        private bool storedInRam = false;

        private UInt32 lastChecksum = 0;

        //Serial or TCP
        private int activeComMode;

        public bool StoredInRam
        {
            get { return storedInRam; }
        }

        public UInt32 LastChecksum
        {
            get { return lastChecksum;  }
            set { lastChecksum = value; }
        }

        //Get 4 byte array from Uint32 value
        private byte[] byteFromUint32 (UInt32 value)
        {
            byte[] arr = new byte[4];

            arr[0] = (byte) value;
            arr[1] = (byte) (value << 8);
            arr[2] = (byte) (value << 16);
            arr[3] = (byte) (value << 24);

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

        public UInt32 CalculateChecksum(byte[] inBytes)
        {
            UInt32 returnVal = 0;
            for (int i = 0; i < inBytes.Length; i++)
            {
                returnVal += (UInt32)inBytes[i];
            }
            return returnVal;
        }

        //Clear input buffer
        private void FlushInput()
        {
            if (activeComMode == (int)ComMode.Serial)
                OpenedPort.DiscardInBuffer();
            else
                PortReadExisting();
        }

        //Write string data to opened port
        private void PortWrite(string message)
        {
            if (activeComMode == (int)ComMode.Serial)
                OpenedPort.Write(message);
            else
            {
                char[] msgData = message.ToCharArray();

                for(int i = 0; i < msgData.Length; i++)
                {
                    TcpStream.WriteByte((byte) msgData[i]);
                }
            }
        }

        //Write binary data to opened port
        private void PortWrite(byte[] buffer, int offset, int count)
        {
            if (activeComMode == (int)ComMode.Serial)
                OpenedPort.Write(buffer, offset, count);
            else
                TcpStream.Write(buffer, offset, count);
        }

        //Read binary data from opened port
        private void PortRead(byte[] buffer, int offset, int count)
        {
            if (activeComMode == (int)ComMode.Serial)
                OpenedPort.Read(buffer, offset, count);
            else
                TcpStream.Read(buffer, offset, count);
        }

        //Read single byte from the opened port
        private int PortReadByte()
        {
            if (activeComMode == (int)ComMode.Serial)
                return OpenedPort.ReadByte();
            else
                return TcpStream.ReadByte();
        }

        //Read all available data from the port
        private string PortReadExisting()
        {
            if (activeComMode == (int)ComMode.Serial)
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
            if (activeComMode == (int)ComMode.Serial)
                return OpenedPort.BytesToRead;
            else
            {
                return OpenTcpClient.Available;
            }
        }

        //Return number of bytes waiting in the output buffer
        private int PortBytesToWrite()
        {
            if (activeComMode == (int)ComMode.Serial)
                return OpenedPort.BytesToWrite;
            else
            {
                return 0;   //We are lying here since .net doesn't support it
            }
        }

        //Communication init sequence
        private string InitUnirom(string ComPortName, int cardSlot, int mode, int frameCount)
        {
            int timeout = 100;

            //Start clean
            FlushInput();

            if (mode == (int)Mode.Read)
            {
                PortWrite("MCDN");
                Thread.Sleep(200);

                //Check if this is Unirom
                if (!"MCDNOKV2".Equals(PortReadExisting()))
                    return "Unirom was not detected on '" + ComPortName + "' port.";
            }
            else
            {
                PortWrite("MCUP");
                Thread.Sleep(200);

                //Check if this is Unirom
                if (!"MCUPOKV2".Equals(PortReadExisting()))
                    return "Unirom was not detected on '" + ComPortName + "' port.";
            }

            Console.WriteLine("Dotud");

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
            PortWrite(byteFromUint32((UInt32)cardSlot), 0, 4);

            //Start transfer
            if (mode == (int)Mode.Read)
            {
                PortWrite("MCRD");
            }
            else
            {
                //Number of bytes to write
                PortWrite(BitConverter.GetBytes(frameCount * 128), 0, 4);

                //Checksum
                PortWrite(BitConverter.GetBytes(lastChecksum), 0, 4);

                lastChecksum = 0;
            }

            //Everything went well, Unirom is available to be used
            return null;
        }

        public string StartUnirom(string ComPortName, int cardSlot, int mode, int frameCount)
        {
            //This is serial communication
            activeComMode = (int) ComMode.Serial;

            //Define a port to open
            OpenedPort = new SerialPort(ComPortName, 115200, Parity.None, 8, StopBits.One);
            OpenedPort.ReadBufferSize = 4096;

            //Try to open a selected port (in case of an error return a descriptive string)
            try{ OpenedPort.Open(); }
            catch (Exception e) { return e.Message; }

            return InitUnirom(ComPortName, cardSlot, mode, frameCount);
        }

        public string StartUniromTCP(string remoteAddress, int remotePort, int cardSlot, int mode, int frameCount)
        {
            //This is TCP communication
            activeComMode = (int) ComMode.TCP;

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

            return InitUnirom(remoteAddress + ":" + remotePort.ToString(), cardSlot, mode, frameCount);
        }

        public byte[] ReadMemoryCardFrame(int frameNumber)
        {
            byte[] tempData = new byte[16];
            int timeout = 100;

            if (!storedInRam)
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
                storedInRam = true;

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

                lastChecksum = Uint32FromByte(tempData);
            }

            return frameData;
        }

        public bool WriteMemoryCardChunk(ushort FrameNumber, byte[] ChunkData)
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
        public void StopUnirom()
        {
            if (activeComMode == (int) ComMode.Serial)
                OpenedPort.Close();
            else
            {
                if (TcpStream != null) TcpStream.Close();
                if (TcpStream !=null) OpenTcpClient.Close();
            }
        }
    }
}
