//PS3 Memory Card Adapter communication class (based off ps3mca-ps1)
//nzgamer41 2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

namespace PS3MCACommunication
{
    public class PS3MCA
    {

        byte BULK_WRITE_ENDPOINT = 0x02; /* bEndpointAddress     0x02  EP 2 OUT (Bulk)*/
        byte BULK_READ_ENDPOINT = 0x81;  /* bEndpointAddress     0x81  EP 1 IN  (Bulk)*/
        byte[] bulk_buffer = new byte[64];                /* wMaxPacketSize     0x0040  1x 64 bytes*/

        /*byte INTERRUPT_READ_ENDPOINT = 		0x83;	/* bEndpointAddress     0x83  EP 3 IN  (Interrupt)*/
        /*int INTERRUPT_LENGTH = 				1;	/* wMaxPacketSize     0x0001  1x 1 bytes*/

        /* PS3mca commands*/
        byte PS3MCA_CMD_FIRST = 0xaa;   /* First command for ps3mca protocol*/
        byte PS3MCA_CMD_TYPE_LONG = 0x42;   /* PS1 type of command*/
        byte PS3MCA_CMD_VERIFY_CARD_TYPE = 0x40;   /* Verify what type of card (PS1 or PS2)*/

        /* PS3mca autentication response code*/
        byte RESPONSE_CODE = 0x55;   /* Expected response to PS3MCA_CMD_FIRST*/
        byte RESPONSE_STATUS_SUCCES = 0x5a;   /* Expected response to PS3MCA_CMD_TYPE_LONG*/
        byte RESPONSE_WRONG = 0xaf;   /* Response to PS3MCA_CMD_TYPE_LONG if autentication is failed*/
        byte RESPONSE_PS1_CARD = 0x01;   /* This is a PS1 card*/
        byte RESPONSE_PS2_CARD = 0x02;   /* This is a PS2 card, unused with this driver*/

        /* Command list*/
        byte PS1CARD_CMD_MEMORY_CARD_ACCESS = 0x81; /* Memory Card Access, principal command for any action with any memory card*/

        /* Classic PS1 commands list*/
        byte PS1CARD_CMD_READ = 0x52;   /* Send Read Command (ASCII "R")*/
        byte PS1CARD_CMD_GET_ID = 0x53; /* Send Get ID Command (ASCII "S")*/
        byte PS1CARD_CMD_WRITE = 0x57;  /* Send Write Command (ASCII "W")*/

        /* Expected reply from memory card (SCPH-1020) or PocketStation (SCPH-4000) or maybe other cards devices*/
        byte PS1CARD_REPLY_MC_ID_1 = 0x5a;  /* Memory Card ID1*/
        byte PS1CARD_REPLY_MC_ID_2 = 0x5d;  /* Memory Card ID2*/

        byte PS1CARD_REPLY_COMMAND_ACKNOWLEDGE_1 = 0x5c;    /* Command Acknowledge 1*/
        byte PS1CARD_REPLY_COMMAND_ACKNOWLEDGE_2 = 0x5d;    /* Command Acknowledge 2*/

        byte PS1CARD_REPLY_MEB_GOOD = 0x47; /* Memory End Byte Good (ASCII "G")*/
        byte PS1CARD_REPLY_MEB_BAD_CHECKSUM = 0x4e; /* Memory End Byte BadChecksum (ASCII "N")*/
        byte PS1CARD_REPLY_MEB_BAD_FRAME = 0xff;    /* Memory End Byte BadFrame*/
        byte POCKETSTATION_REPLY_REJECT_EXECUTED = 0xfd;    /* Reject write to Directory Entries of currently executed file*/
        byte POCKETSTATION_REPLY_REJECT_PROTECTED = 0xfe;   /* Reject write to write-protected Broken Frame region*/

        byte PS1CARD_REPLY_NUMBER_FRAME_1 = 0x04;   /* First two significant digits of the number of frame*/
        byte PS1CARD_REPLY_NUMBER_FRAME_2 = 0x00;   /* Last two significant digits of the number of frame (0400h=1024)*/
        byte PS1CARD_REPLY_FRAME_SIZE_1 = 0x00; /* First two significant digits of the frame size*/
        byte PS1CARD_REPLY_FRAME_SIZE_2 = 0x80;	/* Last two significant digits of the frame size (0080h=128)*/



        public static IUsbDevice MyUsbDevice;
        //ps3mca vid and pid from ps3mca-ps1
        public static UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder(0x054c, 0x02ea);
        private IUsbDevice wholeUsbDevice;
        /// <summary>
        /// Initialises the USB connection to the PS3 MCA
        /// </summary>
        /// <returns>null if connected successfully, anything else is an error message.</returns>
        public string StartPS3MCA()
        {
            using (var context = new UsbContext())
            {
                context.SetDebugLevel(LogLevel.Info);
                //Get a list of all connected devices
                var usbDeviceCollection = context.List();
                
                //Narrow down the device by vendor and pid
                var selectedDevice = usbDeviceCollection.FirstOrDefault(d => d.ProductId == 0x02ea && d.VendorId == 0x054c);


                

                if (selectedDevice == null) return ("PS3 Memory Card Adaptor not connected.");
               
                selectedDevice.Open();
                selectedDevice.ClaimInterface(selectedDevice.Configs[0].Interfaces[0].Number);

                MyUsbDevice = selectedDevice;
            }

            //If this all worked then PS3MCA is connected successfully
            return null;
        }

        public void StopPS3MCA()
        {
            if (!ReferenceEquals(MyUsbDevice, null))
            {
                MyUsbDevice.ReleaseInterface(0);
                MyUsbDevice.Close();
            }
        }

        public byte[] ReadMemoryCardFrame(ushort FrameNumber)
        {
            int DelayCounter = 0;

            //Buffer for storing read data from MCA
            byte[] ReadData = null;

            //128 byte frame data from a Memory Card
            byte[] ReturnDataBuffer = new byte[128];

            byte FrameLsb = (byte) (FrameNumber & 0xFF); //Least significant byte
            byte FrameMsb = (byte) (FrameNumber >> 8); //Most significant byte
            byte XorData = (byte) (FrameMsb ^ FrameLsb); //XOR variable for consistency checking

            byte[] readCmd = new byte[144];
            readCmd[0] = PS3MCA_CMD_FIRST;
            readCmd[1] = PS3MCA_CMD_TYPE_LONG;
            readCmd[2] = 0x8c;
            readCmd[3] = 0x00;
            readCmd[4] = PS1CARD_CMD_MEMORY_CARD_ACCESS;
            readCmd[5] = PS1CARD_CMD_READ;
            readCmd[6] = 0x00;
            readCmd[7] = 0x00;
            readCmd[8] = FrameMsb;
            readCmd[9] = FrameLsb;
            int numBytes = 0;
            UsbEndpointWriter writer = MyUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep02, EndpointType.Bulk);
            UsbEndpointReader reader =
                MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01, ReturnDataBuffer.Length, EndpointType.Bulk);

            byte[] ps1_ram_buffer = new byte[256];
            try
            {
                if (writer.Transfer(readCmd, 0, readCmd.Length, 5000, out numBytes) == Error.Success)
                {
                    if (reader.Transfer(ps1_ram_buffer, 0, ps1_ram_buffer.Length, 5000, out numBytes) == Error.Success)
                    {
                        if (numBytes <= ps1_ram_buffer.Length)
                        {
                            /* Verify if PS3mca send status succes code*/
                            if (ps1_ram_buffer[0] == RESPONSE_CODE & ps1_ram_buffer[1] == RESPONSE_STATUS_SUCCES)
                            {
#if DEBUG
                                Console.WriteLine("Authentication verified on frame");
#endif

                            }

                            /* Verify if PS3mca send status wrong code*/
                            else if (ps1_ram_buffer[0] == RESPONSE_CODE & ps1_ram_buffer[1] == RESPONSE_WRONG)
                            {
                                Console.WriteLine("Autentication failed on frame .\n");
                            }

                            /* Other unknown PS3mca error*/
                            else
                            {
                                Console.WriteLine("Unknown error on PS3mca protocol on frame .\n");
                            }

                            /* Verify command acknowledge*/
                            if (ps1_ram_buffer[10] == PS1CARD_REPLY_COMMAND_ACKNOWLEDGE_1 &
                                ps1_ram_buffer[11] == PS1CARD_REPLY_COMMAND_ACKNOWLEDGE_2)
                            {
#if DEBUG
                                Console.WriteLine("Good command acknowledge on frame .\n");
#endif
                            }

                            /* Unknown command acknowledge error*/
                            else
                            {
                                Console.WriteLine("Unknown command acknowledge error on frame .\n");
                            }

                            /* Verify msb and lsb*/
                            if (ps1_ram_buffer[12] == FrameMsb & ps1_ram_buffer[13] == FrameLsb)
                            {
#if DEBUG
                                Console.WriteLine("Confirmed frame number on frame .\n");
#endif
                            }

                            /* Unknown frame error*/
                            else
                            {
                                Console.WriteLine("Unknown frame number error on frame .\n");
                                Console.WriteLine("Return frame number  .\n\n", ps1_ram_buffer[12], ps1_ram_buffer[13]);
                            }

                            /* This permit to select and save only the received Data Frame (PS1CARD_FRAME_SIZE=128 bytes).*/
                            /* First 14 bytes are about PS3mca (4 bytes) and PS1 (10 bytes) protocol.*/
                            /* Last 2 bytes are Checksum & Memory End Byte.*/

                            //fwrite(&ps1_ram_buffer[14], 1, PS1CARD_FRAME_SIZE, output);

                            Array.Copy(ps1_ram_buffer, 14, ReturnDataBuffer, 0, 128);

                            /* Verify checksum*/
                            if (ps1_ram_buffer[142] == XorData)
                            {
#if DEBUG
                                Console.WriteLine("Good checksum on frame .\n");
#endif
                            }

                            /* Checksum error*/
                            else
                            {
                                Console.WriteLine("Incorrect checksum on frame .\n");
                                Console.WriteLine(String.Format("Received %x, should be %x.\n\n", ps1_ram_buffer[142],
                                    XorData));
                            }


                            /* Verify MEB*/
                            if (ps1_ram_buffer[143] == PS1CARD_REPLY_MEB_GOOD)
                            {
#if DEBUG
                                Console.WriteLine("Good Memory End Byte on frame .\n");
#endif
                            }

                            /* Other unknown MEB error*/
                            else
                            {
                                Console.WriteLine("Unknown Memory End Byte on frame .\n");
                                Console.WriteLine("Received %x, should be 47.\n\n", ps1_ram_buffer[143]);
                            }

                            return ReturnDataBuffer;

                        }
                        else
                        {
                            Console.WriteLine(String.Format(
                                "Received %d bytes, expected a maximum of %lu bytes on frame\n",
                                numBytes, ps1_ram_buffer.Length));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error receiving data from PS3MCA");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public bool WriteMemoryCardFrame(ushort FrameNumber, byte[] FrameData)
        {
            int DelayCounter = 0;

            //Buffer for storing read data from PS3MCA
            byte[] ReadData = null;

            //128 byte frame data from a Memory Card
            byte[] ReturnDataBuffer = new byte[128];

            byte FrameLsb = (byte) (FrameNumber & 0xFF); //Least significant byte
            byte FrameMsb = (byte) (FrameNumber >> 8); //Most significant byte
            byte XorData = (byte) (FrameMsb ^ FrameLsb); //XOR variable for consistency checking

            //Calculate XOR checksum
            for (int i = 0; i < 128; i++)
            {
                XorData ^= FrameData[i];
            }

            byte[] cmd_write = new byte[142];
            byte[] ps1_ram_buffer = new byte[256];
            cmd_write[0] = PS3MCA_CMD_FIRST;            /* First command for ps3mca protocol*/
            cmd_write[1] = PS3MCA_CMD_TYPE_LONG;            /* PS1 type of command*/
            cmd_write[2] = 0x8a;                    /* 142-4=138=8ah lenght of command. Can be cmd_write[2]=sizeof(cmd_write)-4*/
            cmd_write[3] = 0x00;                    /* memset set all to 0x00, but I prefer to specify it a second time ;)*/
            /* this is the real command write with lenght of 138=0x8a*/
            cmd_write[4] = PS1CARD_CMD_MEMORY_CARD_ACCESS;  /* Memory Card Access, principal command for any action with any memory card*/
            cmd_write[5] = PS1CARD_CMD_WRITE;           /* Send write Command (ASCII "W")*/
            cmd_write[6] = 0x00;                    /* Ask Memory Card ID1*/
            cmd_write[7] = 0x00;                    /* Ask Memory Card ID2*/
            cmd_write[8] = FrameMsb;                 /* First two significant digits of the frame value*/
            cmd_write[9] = FrameLsb;                 /* Last two significant digits of the frame value*/
            for (int i = 10; i < 138; i++)
            {
                cmd_write[i] = FrameData[i - 10];
            }
            cmd_write[138] = XorData;
            cmd_write[139] = 0x00;              /* Receive Command Acknowledge 1*/
            cmd_write[140] = 0x00;              /* Receive Command Acknowledge 2*/
            cmd_write[141] = 0x00;              /* Receive Memory End Byte (47h=Good, 4Eh=BadChecksum, FFh=BadSector)*/
            int numBytes = 0;
            UsbEndpointWriter writer = MyUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep02, EndpointType.Bulk);
            UsbEndpointReader reader =
                MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01, ReturnDataBuffer.Length, EndpointType.Bulk);

            if (writer.Transfer(cmd_write, 0, cmd_write.Length, 5000, out numBytes) == Error.Success)
            {
                if (reader.Transfer(ps1_ram_buffer, 0, ps1_ram_buffer.Length, 5000, out numBytes) == Error.Success)
                {

                    if (numBytes <= ps1_ram_buffer.Length)
                    {
                        /* Verify if PS3mca send status succes code*/
                        if (ps1_ram_buffer[0] == RESPONSE_CODE & ps1_ram_buffer[1] == RESPONSE_STATUS_SUCCES)
                        {
#if DEBUG
                            Console.WriteLine("Authentication verified on frame.\n");
#endif
                        }

                        /* Verify if PS3mca send status wrong code*/
                        else if (ps1_ram_buffer[0] == RESPONSE_CODE & ps1_ram_buffer[1] == RESPONSE_WRONG)
                        {
                            Console.WriteLine("Autentication failed on frame, retrying..");
                            return false;
                        }

                        /* Other unknown PS3mca error*/
                        else
                        {
                            Console.WriteLine("Unknown error on PS3mca protocol\n");
                        }


                        /* Verify Memory End Byte (0x47h=Good)*/
                        if (ps1_ram_buffer[141] == PS1CARD_REPLY_MEB_GOOD)
                        {
#if DEBUG
                            Console.WriteLine("Good Memory End Byte on frame.\n");
#endif
                        }

                        /* Verify Memory End Byte (0x4E=BadChecksum)*/
                        else if (ps1_ram_buffer[141] == PS1CARD_REPLY_MEB_BAD_CHECKSUM)
                        {
                            Console.WriteLine("Bad Checksum Memory End Byte on frame.\n");
                            Console.WriteLine(String.Format("In your image checksum is %x, should be %x.\n", cmd_write[138], XorData));
                        }

                        /* Verify Memory End Byte (0xFF=BadFrame)*/
                        else if (ps1_ram_buffer[141] == PS1CARD_REPLY_MEB_BAD_FRAME)
                        {
                            Console.WriteLine("Bad frame Memory End Byte on frame.\n");
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
