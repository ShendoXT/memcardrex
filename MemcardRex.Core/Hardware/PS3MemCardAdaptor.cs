using System;
using System.IO;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace MemcardRex.Core
{
    /// <summary>
    /// Implementation of the PS3 Memory Card Adaptor USB device, based on the LibUsbDotNet library.
    /// 
    /// To use this implementation, a WinUSB or LibUSB driver needs to be installed for the PS3 Memory Card Adaptor first.
    /// This driver can be created and installed with Zadig USB driver installer: https://zadig.akeo.ie
    /// </summary>
    public class PS3MemCardAdaptor : HardwareInterface
    {
        private const int ReadCommandLength = 144;
        private const int WriteCommandLength = 142;
        private const int PocketInfoLength = 25;
        private const int PocketMemoryLength = 142;
        private const int PocketTimeLength = 142;   //Should be 18, but 3rd party adapters don't support short commands
        private const int Timeout = 5000;
        private const int MaxRetries = 5;

        private static readonly byte[] CmdGetCardType = new byte[] { 0xAA, 0x40 };
        private static readonly byte[] HdrReadPS1Frame = new byte[] { 0xAA, 0x42, ReadCommandLength - 4, 0x00, 0x81, 0x52 };
        private static readonly byte[] HdrWritePS1Frame = new byte[] { 0xAA, 0x42, WriteCommandLength - 4, 0x00, 0x81, 0x57 };
        private static readonly byte[] HdrPocketInfo = new byte[] { 0xAA, 0x42, PocketInfoLength - 4, 0x00, 0x81, 0x5A };
        private static readonly byte[] HdrPocketBIOS = new byte[] { 0xAA, 0x42, PocketMemoryLength - 4, 0x00, 0x81, 0x5B };
        private static readonly byte[] HdrPocketTime = new byte[] { 0xAA, 0x42, PocketTimeLength - 4, 0x00, 0x81, 0x5C };

        private static readonly UsbDeviceFinder DeviceFinder = new UsbDeviceFinder(0x054C, 0x02EA);

        private UsbDevice _usbDevice;
        private UsbEndpointReader _reader;
        private UsbEndpointWriter _writer;

        private byte[] _buffer = new byte[256];
        private byte[] _readFrameCommand = new byte[ReadCommandLength];
        private byte[] _writeFrameCommand = new byte[WriteCommandLength];
        private byte[] _pocketInfoCommand = new byte[PocketInfoLength];
        private byte[] _pocketMemoryCommand = new byte[PocketMemoryLength];
        private byte[] _pocketTimeCommand = new byte[PocketTimeLength];

        public ErrorCode LastErrorCode { get; private set; }

        //Name of the interface
        private const string InterfaceName = "PS3 MC Adaptor";

        public override string Name()
        {
            return InterfaceName;
        }

        public override SupportedFeatures Features()
        {
            return SupportedFeatures.RealtimeMode | SupportedFeatures.PocketStation;
        }

        public PS3MemCardAdaptor() : base()
        {
            Type = Types.ps3mca;

            // Initialize command buffers by zeroing them out and copying the command headers
            Array.Clear(_readFrameCommand, 0, _readFrameCommand.Length);
            Array.Copy(HdrReadPS1Frame, _readFrameCommand, HdrReadPS1Frame.Length);

            Array.Clear(_writeFrameCommand, 0, _writeFrameCommand.Length);
            Array.Copy(HdrWritePS1Frame, _writeFrameCommand, HdrWritePS1Frame.Length);

            Array.Clear(_pocketInfoCommand, 0, _pocketInfoCommand.Length);
            Array.Copy(HdrPocketInfo, _pocketInfoCommand, HdrPocketInfo.Length);

            Array.Clear(_pocketMemoryCommand, 0, _pocketMemoryCommand.Length);
            Array.Copy(HdrPocketBIOS, _pocketMemoryCommand, HdrPocketBIOS.Length);

            Array.Clear(_pocketTimeCommand, 0, _pocketTimeCommand.Length);
            Array.Copy(HdrPocketTime, _pocketTimeCommand, HdrPocketTime.Length);
        }

        public override string Start(string dummy, int dummy2)
        {
            try
            {
                //Use LibUSB backend if available, otherwise use WinUSB (relevant only on Windows)
                UsbDevice.ForceLibUsbWinBack = File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "libusb-1.0.dll"));
                
                // Detect and open the PS3 Memory Card Adaptor USB device
                _usbDevice = UsbDevice.OpenUsbDevice(DeviceFinder);

                if (_usbDevice == null)
                {
                    UsbDevice.Exit();

                    return "Could not find the PS3 Memory Card Adaptor." + 
                        "\nPlease make sure it is connected to a USB port and that the appropriate driver is installed." + 
                        "\nConsult ReadMe for detailed instructions.";
                }

                //Check if this is a "whole" usb device
                IUsbDevice wholeUsbDevice = _usbDevice as IUsbDevice;

                if (!ReferenceEquals(wholeUsbDevice, null))
                {
                    // This is a "whole" USB device. Before it can be used, 
                    // the desired configuration and interface must be selected.

                    // Select config #1
                    wholeUsbDevice.SetConfiguration(1);

                    // Claim interface #0.
                    wholeUsbDevice.ClaimInterface(0);
                }
            }
            catch (Exception e)
            {
                _usbDevice.Close();
                UsbDevice.Exit();
                return e.Message;
            }

            // Prepare the device for reading
            _reader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
            if (_reader == null)
            {
                return "Could not open a reader for the PS3 Memory Card Adaptor device.";
            }

            // Prepare the device for writing
            _writer = _usbDevice.OpenEndpointWriter(WriteEndpointID.Ep02);
            if (_writer == null)
            {
                return "Could not open a writer for the PS3 Memory Card Adaptor device.";
            }

            // Send a command to detect the type of memory card inserted, if any
            int bytesWritten;
            LastErrorCode = _writer.Write(CmdGetCardType, Timeout, out bytesWritten);
            if (LastErrorCode != ErrorCode.None || bytesWritten != CmdGetCardType.Length)
            {
                return UsbDevice.LastErrorString;
                return "Failed to write command to the PS3 Memory Card Adaptor device.";
            }

            // Check if it's a PS1 memory card
            int bytesRead;
            LastErrorCode = _reader.Read(_buffer, Timeout, out bytesRead);
            if (LastErrorCode != ErrorCode.None || bytesRead != 2 || _buffer[0] != 0x55 || _buffer[1] != 0x01)
            {
                return "No PS1 memory card detected!";
            }

            return null;
        }

        public override void Stop()
        {
            if (_usbDevice != null && _usbDevice.IsOpen)
            {
                _usbDevice.Close();
                UsbDevice.Exit();
            }
        }

        public override byte[] DumpPocketStationBIOS(int part)
        {
            //Address of the BIOS ROM in PocketStation memory
            UInt32 address = 0x04000000;

            //Data is read in 128 byte chunks
            address += (UInt32) part * 128;

            byte [] frame = DumpPocketStationMemory(address);
            if (frame == null) return null;

            //Check if this is a 3rd party adaptor, they like to inject 'G' - good "write status"
            //even though connected peripheral should return that status and not the adaptor.
            //Since PocketStation extended commands do not use that status we get corrupted last byte.
            if (frame[127] != 0x47) return frame;

            //Found 'G' at the last byte, this might be a knockoff.
            //So read everything again shifted by 2 bytes and copy the real byte over
            byte[] reframe = DumpPocketStationMemory(address + 2);
            if (reframe == null) return null;

            //Copy the real byte over
            frame[127] = reframe[125];

            return frame;
        }

        //Dump 128 byte chunks of memory from PocketStation
        private byte[] DumpPocketStationMemory(UInt32 address)
        {
            //Get memory block function
            _pocketMemoryCommand[6] = 0x1;

            //Address
            _pocketMemoryCommand[8] = (byte) (address & 0xFF);
            _pocketMemoryCommand[9] = (byte) (address >> 8);
            _pocketMemoryCommand[10] = (byte)(address >> 16);
            _pocketMemoryCommand[11] = (byte)(address >> 24);

            //Data length
            _pocketMemoryCommand[12] = 0x80;

            int bytesWritten;
            LastErrorCode = _writer.Write(_pocketMemoryCommand, Timeout, out bytesWritten);
            if (LastErrorCode != ErrorCode.None || bytesWritten != PocketMemoryLength)
            {
                return null;
            }

            // Read response from PocketStation
            int bytesRead;
            LastErrorCode = _reader.Read(_buffer, Timeout, out bytesRead);
            if (LastErrorCode != ErrorCode.None || bytesRead != PocketMemoryLength /*|| _buffer[6] != 0x12*/)
            {
                return null;
            }

            // Strip the header data and return the requested frame
            byte[] frame = new byte[128];
            Array.Copy(_buffer, 14, frame, 0, 128);
            return frame;
        }

        public override UInt32 ReadPocketStationSerial(out string errorMsg)
        {
            //There are a couple of ways to read the serial from a PocketStation
            //Fastest one is to use a special BU command designed just for that.
            //But since short commands used to send data to PocketStation are not working
            //on 3rd party adaptors we will use a memory dump function which
            //seems to work (other than the in this case irrelevant 128th byte corruption)

            //Dump 128 byte data from memory, starting with the serial number
            byte[] frame = DumpPocketStationMemory(0x06000300);

            if (frame == null) {
                errorMsg = "PocketStation not detected.";
                return 0;
            }

            //All good, return data
            errorMsg = null;
            return (UInt32)(frame[0] | frame[1] << 8 | frame[2] << 16 | frame[3] << 24);
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
            //Set time data
            _pocketTimeCommand[9] = getBCD(DateTime.Now.Day);         //Day
            _pocketTimeCommand[10] = getBCD(DateTime.Now.Month);      //Month
            _pocketTimeCommand[11] = getBCD(DateTime.Now.Year % 100); //Year
            _pocketTimeCommand[12] = getBCD(DateTime.Now.Year / 100); //Century

            _pocketTimeCommand[13] = getBCD(DateTime.Now.Second);     //Second
            _pocketTimeCommand[14] = getBCD(DateTime.Now.Minute);     //Minute
            _pocketTimeCommand[15] = getBCD(DateTime.Now.Hour);       //Hour
            _pocketTimeCommand[16] = getBCD(((int)DateTime.Now.DayOfWeek) + 1);     //Day of week

            // Output a command to request the desired frame
            int bytesWritten;
            LastErrorCode = _writer.Write(_pocketTimeCommand, Timeout, out bytesWritten);
            if (LastErrorCode != ErrorCode.None || bytesWritten != PocketTimeLength)
            {
                errorMsg = "USB comm error";
                return false;
            }

            // Read response from PocketStation
            int bytesRead;
            LastErrorCode = _reader.Read(_buffer, Timeout, out bytesRead);
            if (LastErrorCode != ErrorCode.None || bytesRead == 0)
            {
                errorMsg = "PocketStation not detected.";
                return false;
            }

            //All good, time was set
            errorMsg = null;
            return true;
        }

        public override byte[] ReadMemoryCardFrame(ushort frameNumber)
        {
            // Split the 16-bit frame number into two bytes
            _readFrameCommand[8] = (byte)(frameNumber >> 8);
            _readFrameCommand[9] = (byte)(frameNumber & 0xFF);

            // Output a command to request the desired frame
            int bytesWritten;
            LastErrorCode = _writer.Write(_readFrameCommand, Timeout, out bytesWritten);
            if (LastErrorCode != ErrorCode.None || bytesWritten != ReadCommandLength)
            {
                return null;
            }

            // Read the frame data from the memory card
            int bytesRead;
            LastErrorCode = _reader.Read(_buffer, Timeout, out bytesRead);
            if (LastErrorCode != ErrorCode.None || bytesRead != ReadCommandLength || _buffer[0] != 0x55 || _buffer[1] != 0x5A)
            {
                return null;
            }

            // Strip the header data and return the requested frame
            byte[] frame = new byte[128];
            Array.Copy(_buffer, 14, frame, 0, 128);
            return frame;
        }

        public override bool WriteMemoryCardFrame(ushort frameNumber, byte[] frameData)
        {
            // Split the 16-bit frame number into two bytes
            _writeFrameCommand[8] = (byte)(frameNumber >> 8);
            _writeFrameCommand[9] = (byte)(frameNumber & 0xFF);

            // Copy the frame to the command buffer, right after the header data
            Array.Copy(frameData, 0, _writeFrameCommand, 10, 128);

            // Compute XOR checksum from the frame number + frame data
            byte checksum = 0;
            for (int i = 8; i < 10 + 128; ++i)
            {
                checksum ^= _writeFrameCommand[i];
            }
            _writeFrameCommand[10 + 128] = checksum;

            // Try to write the frame to the memory card, and retry a couple of times on error
            int retries = 0;
            while (retries < MaxRetries)
            {
                // Write the frame data to the memory card
                int bytesWritten;
                LastErrorCode = _writer.Write(_writeFrameCommand, Timeout, out bytesWritten);
                System.Threading.Thread.Sleep(50);  // Wait 50 milliseconds for the memory card to write the sector
                if (LastErrorCode != ErrorCode.None || bytesWritten != WriteCommandLength)
                {
                    retries++;
                    continue;
                }

                // Read the acknowledgment from the device
                int bytesRead;
                LastErrorCode = _reader.Read(_buffer, Timeout, out bytesRead);
                if (LastErrorCode != ErrorCode.None || bytesRead != WriteCommandLength || _buffer[0] != 0x55 || _buffer[1] != 0x5A)
                {
                    retries++;
                    continue;
                }

                // Successful write, break out of the retry loop
                break;
            }

            return retries < MaxRetries;
        }
    }
}