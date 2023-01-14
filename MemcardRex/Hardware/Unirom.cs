//Unirom communication class
//Shendo 2023
//Based on commands from NOTPSXSerial by JonathanDotCel

using System;
using System.IO.Ports;
using System.Threading;

namespace UniromCommunication
{
    class Unirom
    {
        public enum Mode
        {
            Read,
            Write
        };

        //Unirom communication port
        private SerialPort OpenedPort = null;

        //Flag indicating if Unirom finished storing data to RAM on the PS1
        private bool storedInRam = false;

        private UInt32 lastChecksum = 0;

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

        public string StartUnirom(string ComPortName, int cardSlot, int mode, int frameCount)
        {
            int timeout = 10;

            //Define a port to open
            OpenedPort = new SerialPort(ComPortName, 115200, Parity.None, 8, StopBits.One);
            OpenedPort.ReadBufferSize = 4096;

            //Try to open a selected port (in case of an error return a descriptive string)
            try{ OpenedPort.Open(); }
            catch (Exception e) { return e.Message; }

            //Start clean
            OpenedPort.DiscardInBuffer();

            if (mode == (int) Mode.Read)
            {
                OpenedPort.Write("MCDN");
                Thread.Sleep(100);

                //Check if this is Unirom
                if (!"MCDNOKV2".Equals(OpenedPort.ReadExisting()))
                    return "Unirom was not detected on '" + ComPortName + "' port.";
            }
            else
            {
                OpenedPort.Write("MCUP");
                Thread.Sleep(100);

                //Check if this is Unirom
                if (!"MCUPOKV2".Equals(OpenedPort.ReadExisting()))
                    return "Unirom was not detected on '" + ComPortName + "' port.";
            }

            //Switch to V2 protocol
            OpenedPort.Write("UPV2");

            //Wait for response from Unirom
            while (OpenedPort.BytesToRead < 4)
            {
                Thread.Sleep(10);
                if (timeout > 0) timeout--; else break;
            }

            if (!"OKAY".Equals(OpenedPort.ReadExisting()))
            {
                return "Unirom was not detected on '" + ComPortName + "' port.";
            }

            //Send slot we want to use
            OpenedPort.Write(byteFromUint32((UInt32) cardSlot), 0, 4);

            //Start transfer
            if (mode == (int) Mode.Read)
            {
                OpenedPort.Write("MCRD");
            }
            else
            {
                //Number of bytes to write
                OpenedPort.Write(BitConverter.GetBytes(frameCount * 128), 0, 4);

                //Checksum
                OpenedPort.Write(BitConverter.GetBytes(lastChecksum), 0, 4);

                lastChecksum = 0;
            }

            //Everything went well, Unirom is available to be used
            return null;
        }

        public string StartUniromTCP(string Address, int Port)
        {
            return null;
        }

        public byte[] ReadMemoryCardFrame(int frameNumber)
        {
            byte[] tempData = new byte[16];
            int timeout = 10;

            if (!storedInRam)
            {
                //If data is still not being ready wait for the next cycle
                if (OpenedPort.BytesToRead < 12)
                {
                    Thread.Sleep(10);
                    return null;
                }

                byte[] respData = new byte[4];
                byte[] addressData = new byte[4];
                byte[] sizeData = new byte[4];

                //MCRD response
                OpenedPort.Read(respData, 0, 4);

                //Address in RAM
                OpenedPort.Read(addressData, 0, 4);

                //Size of data
                OpenedPort.Read(sizeData, 0, 4);

                //Unirom finished storing data to RAM
                storedInRam = true;

                Thread.Sleep(10);

                //Start dumping data
                OpenedPort.Write("DUMP");

                //Wait for response
                while (OpenedPort.BytesToRead < 16)
                {
                    Thread.Sleep(10);
                }

                //Handshake
                OpenedPort.Read(tempData, 0, 16);

                //Send address in RAM
                OpenedPort.Write(addressData, 0, 4);
                OpenedPort.Write(sizeData, 0, 4);

                //Dumping will begin on the next cycle
                return null;
            }

            byte[] frameData = new byte[128];

            //Unirom requires more data request every 2048 bytes, or every 16 frames
            if (frameNumber != 0 && frameNumber % 16 == 0)
            {
                OpenedPort.Write("MORE");
            }

            //Timeout if there is no data for 100ms
            timeout = 10;
            while (OpenedPort.BytesToRead < 128)
            {
                Thread.Sleep(10);
                if (timeout > 0) timeout--; else return null;
            }

            //Read frame data
            OpenedPort.Read(frameData, 0, 128);

            //If this was the last frame fetch checksum
            if (frameNumber == 1023)
            {
                OpenedPort.Write("MORE");
                while (OpenedPort.BytesToRead < 4) ;
                OpenedPort.Read(tempData, 0, 4);

                lastChecksum = Uint32FromByte(tempData);
            }

            return frameData;
        }

        public bool WriteMemoryCardChunk(ushort FrameNumber, byte[] ChunkData)
        {
            ulong chunkChecksum = 0;

            OpenedPort.Write(ChunkData, 0, 2048);

            for (int j = 0; j < 2048; j++)
            {
                chunkChecksum += ChunkData[j];
            }

            while (OpenedPort.BytesToWrite != 0)
                Thread.Sleep(1);

            string cmdBuffer = "";
            while (cmdBuffer != "CHEK")
            {
                if (OpenedPort.BytesToRead != 0)
                {

                    cmdBuffer += (char)OpenedPort.ReadByte();

                }
                while (cmdBuffer.Length > 4)
                    cmdBuffer.Remove(0, 1);
            }

            if (cmdBuffer == "CHEK")
            {
                OpenedPort.Write(BitConverter.GetBytes(chunkChecksum), 0, 4);
                Thread.Sleep(1);

                while (cmdBuffer != "MORE" && cmdBuffer != "ERR!")
                {

                    if (OpenedPort.BytesToRead != 0)
                    {
                        char readVal = (char)OpenedPort.ReadByte();
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
            OpenedPort.Close();
        }

    }
}
