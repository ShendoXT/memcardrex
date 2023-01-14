//Hardware card reading device information window
//Shendo 2012 - 2023

using System;
using System.ComponentModel;
using System.Windows.Forms;
using DexDriveCommunication;
using MemCARDuinoCommunication;
using PS1CardLinkCommunication;
using PS3MemCardAdaptorCommunication;
using UniromCommunication;

namespace MemcardRex
{
    public partial class cardReaderWindow : Form
    {
        //DexDrive Memory Card reading device
        DexDrive dexDevice = new DexDrive();

        //MemCARDuino Memory Card reading device
        MemCARDuino CARDuino = new MemCARDuino();

        //PS1CardLink Memory Card reading device
        PS1CardLink PS1CLnk = new PS1CardLink();

        //PS3 Memory Card Adaptor device
        PS3MemCardAdaptor PS3MCA = new PS3MemCardAdaptor();

        //Unirom Memory Card reading device
        Unirom uniromDevice = new Unirom();

        //Maximum number of frames for writing (usually 1024 but 16 for quick format)
        int maxWritingFrames = 0;

        //Complete Memory Card data
        byte[] completeMemoryCard = new byte[131072];

        //Reading status flag
        bool sucessfullRead = false;

        //Device identifiers
        enum DeviceId:int {
            DexDrive,
            MemCARDuino,
            PS1CardLink,
            PS3MemCardAdaptor,
            Unirom
        };

        int currentDeviceIdentifier;

        public cardReaderWindow()
        {
            InitializeComponent();
        }

        //Read a Memory Card from DexDrive (return null if there was an error)
        public byte[] readMemoryCardDexDrive(Form hostWindow, string applicationName, string comPort)
        {
            //Initialize DexDrive
            string errorString = dexDevice.StartDexDrive(comPort);

            //Check if there were any errors
            if (errorString != null)
            {
                //Display an error message and cleanly close DexDrive communication
                MessageBox.Show(errorString, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                dexDevice.StopDexDrive();
                return null;
            }

            //Set scale for progress bar
            mainProgressBar.Maximum = 1024;

            //Set current device to DexDrive
            currentDeviceIdentifier = (int) DeviceId.DexDrive;

            //Set window title and information
            this.Text = "DexDrive communication";
            infoLabel.Text = "Reading data from DexDrive (ver. " + dexDevice.GetFirmwareVersion() +  ")...";

            //Start reading data
            backgroundReader.RunWorkerAsync();

            this.ShowDialog(hostWindow);

            //Stop working with DexDrive
            dexDevice.StopDexDrive();

            //Check the final status (return data if all is ok, otherwise return null)
            if (sucessfullRead == true) return completeMemoryCard;
            else return null;
        }

        //Read a Memory Card from MemCARDuino
        public byte[] readMemoryCardCARDuino(Form hostWindow, string applicationName, string comPort, int comSpeed)
        {
            //Initialize MemCARDuino
            string errorString = CARDuino.StartMemCARDuino(comPort, comSpeed);

            //Check if there were any errors
            if (errorString != null)
            {
                //Display an error message and cleanly close MemCARDuino communication
                MessageBox.Show(errorString, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                CARDuino.StopMemCARDuino();
                return null;
            }

            //Set scale for progress bar
            mainProgressBar.Maximum = 1024;

            //Set current device to MemCARDuino
            currentDeviceIdentifier = (int) DeviceId.MemCARDuino;

            //Set window title and information
            this.Text = "MemCARDuino communication";
            infoLabel.Text = "Reading data from MemCARDuino (ver. " + CARDuino.GetFirmwareVersion() + ")...";

            //Start reading data
            backgroundReader.RunWorkerAsync();

            this.ShowDialog(hostWindow);

            //Stop working with MemCARDuino
            CARDuino.StopMemCARDuino();

            //Check the final status (return data if all is ok, otherwise return null)
            if (sucessfullRead == true) return completeMemoryCard;
            else return null;
        }

        //Read a Memory Card from PS1CardLink
        public byte[] readMemoryCardPS1CLnk(Form hostWindow, string applicationName, string comPort, int comSpeed, string remoteAddress, int remotePort)
        {
            string errorString;

            //Initialize PS1CardLink
            if (remoteAddress.Length > 0)
            {
                errorString = PS1CLnk.StartPS1CardLinkTCP(remoteAddress, remotePort);
            }
            else
            {
                errorString = PS1CLnk.StartPS1CardLink(comPort, comSpeed);
            }

            //Check if there were any errors
            if (errorString != null)
            {
                //Display an error message and cleanly close PS1CardLink communication
                MessageBox.Show(errorString, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                PS1CLnk.StopPS1CardLink();
                return null;
            }

            //Set scale for progress bar
            mainProgressBar.Maximum = 1024;

            //Set current device to PS1CardLink
            currentDeviceIdentifier = (int) DeviceId.MemCARDuino;

            //Set window title and information
            this.Text = "PS1CardLink communication";
            infoLabel.Text = "Reading data from PS1CardLink (ver. " + PS1CLnk.GetSoftwareVersion() + ")...";

            //Start reading data
            backgroundReader.RunWorkerAsync();

            this.ShowDialog(hostWindow);

            //Stop working with PS1CardLink
            PS1CLnk.StopPS1CardLink();

            //Check the final status (return data if all is ok, otherwise return null)
            if (sucessfullRead == true) return completeMemoryCard;
            else return null;
        }

        //Read a Memory Card from PS3 Memory Card Adaptor
        public byte[] readMemoryCardPS3MCA(Form hostWindow, string applicationName, string comPort)
        {
            //Initialize PS3 Memory Card Adaptor
            string errorString = PS3MCA.OpenUsbDevice();

            //Check if there were any errors
            if (errorString != null)
            {
                //Display an error message and cleanly close PS3 Memory Card Adaptor communication
                MessageBox.Show(errorString, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                PS3MCA.StopPS3MemCardAdaptor();
                return null;
            }

            //Set scale for progress bar
            mainProgressBar.Maximum = 1024;

            //Set current device to PS3 Memory Card Adaptor
            currentDeviceIdentifier = (int) DeviceId.PS3MemCardAdaptor;

            //Set window title and information
            this.Text = "PS3 Memory Card Adaptor communication";
            infoLabel.Text = "Reading data from PS3 Memory Card Adaptor...";

            //Start reading data
            backgroundReader.RunWorkerAsync();

            this.ShowDialog(hostWindow);

            //Stop working with PS3 Memory Card Adaptor
            PS3MCA.StopPS3MemCardAdaptor();

            //Check the final status (return data if all is ok, otherwise return null)
            if (sucessfullRead == true) return completeMemoryCard;
            else return null;
        }

        //Read a Memory Card from Unirom
        public byte[] readMemoryCardUnirom(Form hostWindow, string applicationName, string comPort, string remoteAddress, int remotePort, int cardSlot)
        {
            string errorString;

            //Initialize Unirom
            if (remoteAddress.Length > 0)
            {
                errorString = uniromDevice.StartUniromTCP(remoteAddress, remotePort);
            }
            else
            {
                errorString = uniromDevice.StartUnirom(comPort, cardSlot, (int) Unirom.Mode.Read, 1024);
            }

            //Check if there were any errors
            if (errorString != null)
            {
                //Display an error message and cleanly close Unirom communication
                MessageBox.Show(errorString, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                uniromDevice.StopUnirom();
                return null;
            }

            //Hide progress bar
            mainProgressBar.Visible = false;

            //Set scale for progress bar
            mainProgressBar.Maximum = 1024;

            //Set current device to Unirom
            currentDeviceIdentifier = (int) DeviceId.Unirom;

            //Set window title and information
            this.Text = "Unirom communication";
            infoLabel.Text = "Waiting for Unirom to store contents in RAM.\nTransfer will start after all the sectors have been read.";

            //Start reading data
            backgroundReader.RunWorkerAsync();

            this.ShowDialog(hostWindow);

            //Stop working with Unirom
            uniromDevice.StopUnirom();

            //Check the final status (return data if all is ok, otherwise return null)
            if (sucessfullRead == true)
            {
                //Also check Unirom data transfer checksum
                if (uniromDevice.LastChecksum == uniromDevice.CalculateChecksum(completeMemoryCard)) return completeMemoryCard;
                else
                {
                    MessageBox.Show("Checksum mismatch. Data will not be imported.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }
            }
            else return null;
        }

            //Write a Memory Card to DexDrive
            public void writeMemoryCardDexDrive(Form hostWindow, string applicationName, string comPort, byte[] memoryCardData, int frameNumber)
        {
            //Initialize DexDrive
            string errorString = dexDevice.StartDexDrive(comPort);

            //Check if there were any errors
            if (errorString != null)
            {
                //Display an error message and cleanly close DexDrive communication
                MessageBox.Show(errorString, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                dexDevice.StopDexDrive();
                return;
            }

            //Set maximum number of frames to write
            maxWritingFrames = frameNumber;

            //Set scale for progress bar
            mainProgressBar.Maximum = frameNumber;

            //Set current device to DexDrive
            currentDeviceIdentifier = (int) DeviceId.DexDrive;

            //Set window title and information
            this.Text = "DexDrive communication";
            infoLabel.Text = "Writing data to DexDrive (ver. " + dexDevice.GetFirmwareVersion() + ")...";

            //Set reference to the Memory Card data
            completeMemoryCard = memoryCardData;

            //Start writing data
            backgroundWriter.RunWorkerAsync();

            this.ShowDialog(hostWindow);

            //Stop working with DexDrive
            dexDevice.StopDexDrive();
        }
        
        //Write a Memory Card to MemCARDuino
        public void writeMemoryCardCARDuino(Form hostWindow, string applicationName, string comPort, int comSpeed, byte[] memoryCardData, int frameNumber)
        {
            //Initialize MemCARDuino
            string errorString = CARDuino.StartMemCARDuino(comPort, comSpeed);

            //Check if there were any errors
            if (errorString != null)
            {
                //Display an error message and cleanly close MemCARDuino communication
                MessageBox.Show(errorString, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                CARDuino.StopMemCARDuino();
                return;
            }

            //Set maximum number of frames to write
            maxWritingFrames = frameNumber;

            //Set scale for progress bar
            mainProgressBar.Maximum = frameNumber;

            //Set current device to MemCARDuino
            currentDeviceIdentifier = (int) DeviceId.MemCARDuino;

            //Set window title and information
            this.Text = "MemCARDuino communication";
            infoLabel.Text = "Writing data to MemCARDuino (ver. " + CARDuino.GetFirmwareVersion() + ")...";

            //Set reference to the Memory Card data
            completeMemoryCard = memoryCardData;

            //Start writing data
            backgroundWriter.RunWorkerAsync();

            this.ShowDialog(hostWindow);

            //Stop working with MemCARDuino
            CARDuino.StopMemCARDuino();
        }

        //Write a Memory Card to PS1CardLink
        public void writeMemoryCardPS1CLnk(Form hostWindow, string applicationName, string comPort, int comSpeed, string remoteAddress, int remotePort, byte[] memoryCardData, int frameNumber)
        {
            //Initialize PS1CardLink
            string errorString;

            if (remoteAddress.Length > 0)
            {
                errorString = PS1CLnk.StartPS1CardLinkTCP(remoteAddress, remotePort);
            }
            else
            {
                errorString = PS1CLnk.StartPS1CardLink(comPort, comSpeed);
            }


            //Check if there were any errors
            if (errorString != null)
            {
                //Display an error message and cleanly close PS1CardLink communication
                MessageBox.Show(errorString, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                PS1CLnk.StopPS1CardLink();
                return;
            }

            //Set maximum number of frames to write
            maxWritingFrames = frameNumber;

            //Set scale for progress bar
            mainProgressBar.Maximum = frameNumber;

            //Set current device to PS1CardLink
            currentDeviceIdentifier = (int) DeviceId.PS1CardLink;

            //Set window title and information
            this.Text = "PS1CardLink communication";
            infoLabel.Text = "Writing data to PS1CardLink (ver. " + PS1CLnk.GetSoftwareVersion() + ")...";

            //Set reference to the Memory Card data
            completeMemoryCard = memoryCardData;

            //Start writing data
            backgroundWriter.RunWorkerAsync();

            this.ShowDialog(hostWindow);

            //Stop working with PS1CardLink
            PS1CLnk.StopPS1CardLink();
        }

        //Write a Memory Card to PS3 Memory Card Adaptor
        public void writeMemoryCardPS3MCA(Form hostWindow, string applicationName, string comPort, byte[] memoryCardData, int frameNumber)
        {
            //Initialize PS3 Memory Card Adaptor
            string errorString = PS3MCA.OpenUsbDevice();

            //Check if there were any errors
            if (errorString != null)
            {
                //Display an error message and cleanly close PS3 Memory Card Adaptor communication
                MessageBox.Show(errorString, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                PS3MCA.StopPS3MemCardAdaptor();
                return;
            }

            //Set maximum number of frames to write
            maxWritingFrames = frameNumber;

            //Set scale for progress bar
            mainProgressBar.Maximum = frameNumber;

            //Set current device to PS3 Memory Card Adaptor
            currentDeviceIdentifier = (int) DeviceId.PS3MemCardAdaptor;

            //Set window title and information
            this.Text = "PS3 Memory Card Adaptor communication";
            infoLabel.Text = "Writing data to PS3 Memory Card Adaptor...";

            //Set reference to the Memory Card data
            completeMemoryCard = memoryCardData;

            //Start writing data
            backgroundWriter.RunWorkerAsync();

            this.ShowDialog(hostWindow);

            //Stop working with PS3 Memory Card Adaptor
            PS3MCA.StopPS3MemCardAdaptor();
        }

        //Write a Memory Card to Unirom
        public void writeMemoryCardUnirom(Form hostWindow, string applicationName, string comPort, int cardSlot, string remoteAddress, int remotePort, byte[] memoryCardData, int frameNumber)
        {
            string errorString;

            //Store checksum before opening port
            uniromDevice.LastChecksum = uniromDevice.CalculateChecksum(memoryCardData);

            //Initialize Unirom
            if (remoteAddress.Length > 0)
            {
                errorString = uniromDevice.StartUniromTCP(remoteAddress, remotePort);
            }
            else
            {
                errorString = uniromDevice.StartUnirom(comPort, cardSlot, (int) Unirom.Mode.Write, frameNumber);
            }

            //Check if there were any errors
            if (errorString != null)
            {
                //Display an error message and cleanly close Unirom communication
                MessageBox.Show(errorString, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                uniromDevice.StopUnirom();
                return;
            }

            //Set number of chunks to write, not frames
            maxWritingFrames = frameNumber / 16;

            //Set scale for progress bar
            mainProgressBar.Maximum = frameNumber / 16;

            //Set current device to Unirom
            currentDeviceIdentifier = (int)DeviceId.Unirom;

            //Set window title and information
            this.Text = "Unirom communication";
            infoLabel.Text = "Writing data to Unirom...";

            //Set reference to the Memory Card data
            completeMemoryCard = memoryCardData;

            //Start writing data
            backgroundWriter.RunWorkerAsync();

            this.ShowDialog(hostWindow);

            //Stop working with Unirom
            uniromDevice.StopUnirom();
        }


        private void OKbutton_Click(object sender, EventArgs e)
        {
            //Cancel reading job
            if (backgroundReader.IsBusy)backgroundReader.CancelAsync();

            //Cancel writing job
            if(backgroundWriter.IsBusy)backgroundWriter.CancelAsync();
        }

        private void backgroundReader_DoWork(object sender, DoWorkEventArgs e)
        {
            byte[] tempDataBuffer = null;
            ushort i = 0;

            //Read all 1024 frames of the Memory Card
            while(i < 1024)
            {
                //Check if the "Abort" button has been pressed
                if (backgroundReader.CancellationPending == true) return;

                //Get 128 byte frame data from selected device
                switch (currentDeviceIdentifier)
                {
                    case (int) DeviceId.DexDrive:
                        tempDataBuffer = dexDevice.ReadMemoryCardFrame(i);
                        break;

                    case (int) DeviceId.MemCARDuino:
                        tempDataBuffer = CARDuino.ReadMemoryCardFrame(i);
                        break;

                    case (int) DeviceId.PS1CardLink:
                        tempDataBuffer = PS1CLnk.ReadMemoryCardFrame(i);
                        break;

                    case (int) DeviceId.PS3MemCardAdaptor:
                        tempDataBuffer = PS3MCA.ReadMemoryCardFrame(i);
                        break;

                    case (int)DeviceId.Unirom:
                        tempDataBuffer = uniromDevice.ReadMemoryCardFrame(i);
                        break;
                }

                //Check if there was a checksum mismatch
                if (tempDataBuffer != null)
                {
                    Array.Copy(tempDataBuffer, 0, completeMemoryCard, i * 128, 128);
                    backgroundReader.ReportProgress(i);
                    i++;
                }
            }

            //All data has been read, report success
            sucessfullRead = true;
        }

        private void backgroundReader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if(currentDeviceIdentifier == (int)DeviceId.Unirom)
            {
                if(!mainProgressBar.Visible && uniromDevice.StoredInRam)
                {
                    mainProgressBar.Visible = true;
                    infoLabel.Text = "Reading data from Unirom...";
                }
            }

            //Report the read progress to the progress bar
            mainProgressBar.Value = e.ProgressPercentage;
        }

        private void backgroundReader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Reading was completed or canceled, close the form
            this.Close();
        }

        private void backgroundWriter_DoWork(object sender, DoWorkEventArgs e)
        {
            //Default frame size
            int frameSize = 128;

            //Unirom works with 2048 byte chunks
            if (currentDeviceIdentifier == (int)DeviceId.Unirom) frameSize = 2048;

            byte[] tempDataBuffer = new byte[frameSize];
            ushort i = 0;
            bool lastStatus = false;

            //Write all required frames of the Memory Card
            while (i < maxWritingFrames)
            {
                //Check if the "Abort" button has been pressed
                if (backgroundWriter.CancellationPending == true) return;

                //Get frame data
                Array.Copy(completeMemoryCard, i * frameSize, tempDataBuffer, 0, frameSize);

                //Reset write status
                lastStatus = false;

                //Write 128 byte frame data to device (or 2048 in case of Unirom)
                switch (currentDeviceIdentifier)
                {
                    case (int) DeviceId.DexDrive:
                        lastStatus = dexDevice.WriteMemoryCardFrame(i, tempDataBuffer);
                        break;

                    case (int) DeviceId.MemCARDuino:
                        lastStatus = CARDuino.WriteMemoryCardFrame(i, tempDataBuffer);
                        break;

                    case (int) DeviceId.PS1CardLink:
                        lastStatus = PS1CLnk.WriteMemoryCardFrame(i, tempDataBuffer);
                        break;

                    case (int) DeviceId.PS3MemCardAdaptor:
                        lastStatus = PS3MCA.WriteMemoryCardFrame(i, tempDataBuffer);
                        break;

                    case (int)DeviceId.Unirom:
                        lastStatus = uniromDevice.WriteMemoryCardChunk(i, tempDataBuffer);
                        break;
                }

                //Check if there was a frame or checksum mismatch
                if (lastStatus == true)
                {
                    backgroundWriter.ReportProgress(i);
                    i++;
                }
            }

            //All data has been written, report success
            sucessfullRead = true;
        }

        private void backgroundWriter_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Report the write progress to the progress bar
            mainProgressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWriter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Writing was completed or canceled, close the form
            this.Close();
        }
    }
}
