//PS1 Memory Card class
//Shendo 2009-2021

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;

namespace MemcardRex
{
    class ps1card
    {
        //Supported Memory Card extensions
        public const string mcExtensions = "Standard Memory Card|*.mcr;*.bin;*.ddf;*.mc;*.mcd;*.mci;*.ps;*.psm;*.srm;*.vm1;*.vmc|PSP/Vita Memory Card|*.VMP|PS Vita \"MCX\" PocketStation Memory Card|*.BIN|DexDrive Memory Card|*.gme|VGS Memory Card|*.mem;*.vgs";

        //Supported single save extensions
        public const string ssExtensions = "PSXGameEdit/Memory Juggler|*.mcs;*.ps1|PS3 single save|*.PSV|Smart Link/XP, AR, GS, Caetla/Datel|*.mcb;*.mcx;*.pda;*.psx|RAW single save|B???????????*";

        //Memory Card's name
        public string cardName = null;

        //Memory Card's location (path + filename)
        public string cardLocation = null;

        //Memory Card's type (0 - unset, 1 - raw, 2 - gme, 3 - vgs, 4 - vmp);
        public byte cardType = 0;

        //Flag used to determine if the card has been edited since the last saving
        public bool changedFlag = false;

        //Complete Memory Card in the raw format (131072 bytes)
        byte[] rawMemoryCard = new byte[131072];

        //Header data for the GME Memory Card
        public byte[] gmeHeader = new byte[3904];

        //Memory Card header data, 15 slots (128 bytes each)
        public byte[,] headerData = new byte[15,128];

        //Memory Card save data, 15 slots (8192 bytes each)
        public byte[,] saveData = new byte[15,8192];

        //Memory Card icon palette data, 15 slots
        public Color[,] iconPalette = new Color[15,16];

        //Memory Card icon data, 15 slots, 3 icons per slot, (16*16px icons)
        public Bitmap[,] iconData = new Bitmap[15,3];

        //Number of icon frames
        public int[] iconFrames = new int[15];

        //Product code of the save
        public string[] saveProdCode = new string[15];

        //Identifier string of the save
        public string[] saveIdentifier = new string[15];

        //Region of the save (16 bit value, ASCII representation: BA - America, BE - Europe, BI - Japan)
        public ushort[] saveRegion = new ushort[15];

        //Size of the save in KBs
        public int[] saveSize = new int[15];

        //Name of the save in UTF-16 encoding
        public string[] saveName = new string[15];

        //Save comments (supported by .gme files only), 255 characters allowed
        public string[] saveComments = new string[15];

        //Type of the save (0 - formatted, 1 - initial, 2 - middle link, 3 - end link, 4 - deleted initial, 5 - deleted middle link, 6 - deleted end link, 7 - corrupted)
        public byte[] saveType = new byte[15];

        readonly byte[] saveKey = { 0xAB, 0x5A, 0xBC, 0x9F, 0xC1, 0xF4, 0x9D, 0xE6, 0xA0, 0x51, 0xDB, 0xAE, 0xFA, 0x51, 0x88, 0x59 };
        readonly byte[] saveIv = { 0xB3, 0x0F, 0xFE, 0xED, 0xB7, 0xDC, 0x5E, 0xB7, 0x13, 0x3D, 0xA6, 0x0D, 0x1B, 0x6B, 0x2C, 0xDC };

        readonly byte[] mcxKey = { 0x81, 0xD9, 0xCC, 0xE9, 0x71, 0xA9, 0x49, 0x9B, 0x04, 0xAD, 0xDC, 0x48, 0x30, 0x7F, 0x07, 0x92 };
        readonly byte[] mcxIv = { 0x13, 0xC2, 0xE7, 0x69, 0x4B, 0xEC, 0x69, 0x6D, 0x52, 0xCF, 0x00, 0x09, 0x2A, 0xC1, 0xF2, 0x72 };

        //Overwrite the contents of one byte array
        private void FillByteArray(byte[] destination, int start, int fill)
        {
            for (int i = 0; i < destination.Length - start; i++)
            {
                destination[i + start] = (byte)fill;
            }
        }

        //XORs a buffer with a constant
        private void XorWithByte(byte[] buffer, byte xorByte)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ xorByte);
            }
        }

        //XORs one buffer with another
        private void XorWithIv(byte[] destBuffer, byte[] iv)
        {
            for (int i = 0; i < 16; i++)
            {
                destBuffer[i] = (byte)(destBuffer[i] ^ iv[i]);
            }
        }

        //Encrypts a buffer using AES CBC 128
        private byte[] AesCbcEncrypt(byte[] toEncrypt, byte[] key, byte[] iv)
        {
            Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Padding = PaddingMode.Zeros;
            aes.Mode = CipherMode.CBC;

            using (ICryptoTransform encryptor = aes.CreateEncryptor(key, iv))
            {
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (BinaryWriter bnEncrypt = new BinaryWriter(csEncrypt))
                        {
                            bnEncrypt.Write(toEncrypt);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        //Decrypts a buffer using AES CBC 128
        private byte[] AesCbcDecrypt(byte[] toDecrypt, byte[] key, byte[] iv)
        {
            Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Padding = PaddingMode.Zeros;
            aes.Mode = CipherMode.CBC;

            using (ICryptoTransform decryptor = aes.CreateDecryptor(key, iv))
            {
                using (MemoryStream msDecrypt = new MemoryStream(toDecrypt))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (BinaryReader bnDecrypt = new BinaryReader(csDecrypt))
                        {
                            return bnDecrypt.ReadBytes(toDecrypt.Length - (toDecrypt.Length % 16));
                        }
                    }
                }
            }
        }

        //Encrypts a buffer using AES ECB 128
        private byte[] AesEcbEncrypt(byte[] toEncrypt, byte[] key, byte[] iv)
        {
            Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Padding = PaddingMode.Zeros;
            aes.Mode = CipherMode.ECB;

            using (ICryptoTransform encryptor = aes.CreateEncryptor(key, iv))
            {
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (BinaryWriter bnEncrypt = new BinaryWriter(csEncrypt))
                        {
                            bnEncrypt.Write(toEncrypt);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        //Decrypts a buffer using AES ECB 128
        private byte[] AesEcbDecrypt(byte[] toDecrypt, byte[] key, byte[] iv)
        {
            Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Padding = PaddingMode.Zeros;
            aes.Mode = CipherMode.ECB;

            using (ICryptoTransform decryptor = aes.CreateDecryptor(key, iv))
            {
                using (MemoryStream msDecrypt = new MemoryStream(toDecrypt))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (BinaryReader bnDecrypt = new BinaryReader(csDecrypt))
                        {
                            return bnDecrypt.ReadBytes(toDecrypt.Length - (toDecrypt.Length % 16));
                        }
                    }
                }
            }
        }

        //Load Data from raw Memory Card
        private void loadDataFromRawCard()
        {
            for (int slotNumber = 0; slotNumber < 15; slotNumber++)
            {
                //Load header data
                for (int currentByte = 0; currentByte < 128; currentByte++)
                {
                    headerData[slotNumber, currentByte] = rawMemoryCard[128 + (slotNumber * 128) + currentByte];
                }

                //Load save data
                for (int currentByte = 0; currentByte < 8192; currentByte++)
                {
                    saveData[slotNumber, currentByte] = rawMemoryCard[8192 + (slotNumber * 8192) + currentByte];
                }
            }
        }

        //Recreate raw Memory Card
        private void loadDataToRawCard(bool fixData)
        {
            //Check if data needs to be fixed or left as is (mandatory for FreePSXBoot)
            if (fixData)
            {
                //Clear existing data
                rawMemoryCard = new byte[131072];

                //Recreate the signature
                rawMemoryCard[0] = 0x4D;        //M
                rawMemoryCard[1] = 0x43;        //C
                rawMemoryCard[127] = 0x0E;      //XOR (precalculated)

                rawMemoryCard[8064] = 0x4D;     //M
                rawMemoryCard[8065] = 0x43;     //C
                rawMemoryCard[8191] = 0x0E;     //XOR (precalculated)
            }

            //This can be copied freely without fixing
            for (int slotNumber = 0; slotNumber < 15; slotNumber++)
            {
                //Load header data
                for (int currentByte = 0; currentByte < 128; currentByte++)
                {
                    rawMemoryCard[128 + (slotNumber * 128) + currentByte] = headerData[slotNumber, currentByte];
                }

                //Load save data
                for (int currentByte = 0; currentByte < 8192; currentByte++)
                {
                    rawMemoryCard[8192 + (slotNumber * 8192) + currentByte] = saveData[slotNumber, currentByte];
                }
            }


            //Skip fixing data if it's not needed
            if (!fixData) return;

            //Create authentic data (just for completeness)
            for (int i = 0; i < 20; i++)
            {
                //Reserved slot typed
                rawMemoryCard[2048 + (i * 128)] = 0xFF;
                rawMemoryCard[2048 + (i * 128) + 1] = 0xFF;
                rawMemoryCard[2048 + (i * 128) + 2] = 0xFF;
                rawMemoryCard[2048 + (i * 128) + 3] = 0xFF;

                //Next slot pointer doesn't point to anything
                rawMemoryCard[2048 + (i * 128) + 8] = 0xFF;
                rawMemoryCard[2048 + (i * 128) + 9] = 0xFF;
            }
        }

        //Recreate GME header(add signature, slot description and comments)
        private void fillGmeHeader()
        {
            byte[] tempByteArray;

            //Clear existing data
            gmeHeader = new byte[3904];

            //Fill in the signature
            gmeHeader[0] = 0x31;        //1
            gmeHeader[1] = 0x32;        //2
            gmeHeader[2] = 0x33;        //3
            gmeHeader[3] = 0x2D;        //-
            gmeHeader[4] = 0x34;        //4
            gmeHeader[5] = 0x35;        //5
            gmeHeader[6] = 0x36;        //6
            gmeHeader[7] = 0x2D;        //-
            gmeHeader[8] = 0x53;        //S
            gmeHeader[9] = 0x54;        //T
            gmeHeader[10] = 0x44;       //D

            gmeHeader[18] = 0x1;
            gmeHeader[20] = 0x1;
            gmeHeader[21] = 0x4D;       //M

            for (int slotNumber = 0; slotNumber < 15; slotNumber++)
            {
                gmeHeader[22 + slotNumber] = headerData[slotNumber, 0];
                gmeHeader[38 + slotNumber] = headerData[slotNumber, 8];

                //Convert string from UTF-16 to currently used codepage
                tempByteArray = Encoding.Convert(Encoding.Unicode, Encoding.Default, Encoding.Unicode.GetBytes(saveComments[slotNumber]));

                //Inject comments to GME header
                for (int byteCount = 0; byteCount < tempByteArray.Length; byteCount++)
                    gmeHeader[byteCount + 64 + (256*slotNumber)] = tempByteArray[byteCount];
            }

        }

        //Gets the HMAC checksum for .psv or .vmp saving
        private byte[] GetHmac(byte[] data, byte[] saltSeed)
        {
            byte[] buffer = new byte[0x14];
            byte[] salt = new byte[0x40];
            byte[] temp = new byte[0x14];
            byte[] hash1 = new byte[data.Length + 0x40];
            byte[] hash2 = new byte[0x54];
            SHA1 sha1 = SHA1.Create();

            Array.Copy(saltSeed, buffer, 0x14);
            Array.Copy(AesEcbDecrypt(buffer, saveKey, saveIv), buffer, 0x10);
            Array.Copy(buffer, salt, 0x10);
            Array.Copy(saltSeed, buffer, 0x10);
            Array.Copy(AesEcbEncrypt(buffer, saveKey, saveIv), buffer, 0x14);

            Array.Copy(buffer, 0, salt, 0x10, 0x10);
            XorWithIv(salt, saveIv);
            FillByteArray(buffer, 0x14, 0xFF);
            Array.Copy(saltSeed, 0x10, buffer, 0, 0x4);
            Array.Copy(salt, 0x10, temp, 0, 0x14);
            XorWithIv(temp, buffer);
            Array.Copy(temp, 0, salt, 0x10, 0x10);
            Array.Copy(salt, temp, 0x14);
            FillByteArray(salt, 0x14, 0);
            Array.Copy(temp, salt, 0x14);
            XorWithByte(salt, 0x36);

            Array.Copy(salt, hash1, 0x40);
            Array.Copy(data, 0, hash1, 0x40, data.Length);
            Array.Copy(sha1.ComputeHash(hash1), buffer, 0x14);
            XorWithByte(salt, 0x6A);
            Array.Copy(salt, hash2, 0x40);
            Array.Copy(buffer, 0, hash2, 0x40, 0x14);
            return sha1.ComputeHash(hash2);

        }

        private byte[] DecryptMcxCard(byte[] rawCard)
        {
            byte[] mcxCard = new byte[0x200A0];
            Array.Copy(rawCard, mcxCard, mcxCard.Length);
            return AesCbcDecrypt(mcxCard, mcxKey, mcxIv);
        }
        // Check if a given card is a MCX image
        private bool IsMcxCard(byte[] rawCard)
        {
            byte[] mcxCard = DecryptMcxCard(rawCard);
            // Check for "MC" header 0x80 bytes in
            if (mcxCard[0x80] == 'M' && mcxCard[0x81] == 'C') 
                return true;
            else
                return false;
        }

        //Generate encrypted MCX Memory Card
        private byte[] MakeMcxCard(byte[] rawCard)
        {
            byte[] mcxCard = new byte[0x200A0];
            byte[] hash;

            Array.Copy(rawCard, 0, mcxCard, 0x80, 0x20000);
            
            using (SHA256 sha = SHA256.Create())
                hash = sha.ComputeHash(mcxCard, 0, 0x20080);

            Array.Copy(hash, 0, mcxCard, 0x20080, 0x20);
            Array.Copy(AesCbcEncrypt(mcxCard, mcxKey, mcxIv), 0, mcxCard, 0x0, 0x200A0);
            return mcxCard;
        }

        //Generate signed VMP Memory Card
        private byte[] MakeVmpCard(byte[] rawCard)
        {
            byte[] vmpCard = new byte[0x20080];
            byte[] saltSeed;

            vmpCard[1] = 0x50;
            vmpCard[2] = 0x4D;
            vmpCard[3] = 0x56;
            vmpCard[4] = 0x80; //header length 

            Array.Copy(rawCard, 0, vmpCard, 0x80, 0x20000);

            using (SHA1 sha = SHA1.Create())
                saltSeed = sha.ComputeHash(vmpCard);

            Array.Copy(saltSeed, 0, vmpCard, 0x0C, 0x14);
            Array.Copy(GetHmac(vmpCard, saltSeed), 0, vmpCard, 0x20, 0x14);
            return vmpCard;
        }

        //Generate signed PSV save
        private byte[] MakePsvSave(byte[] save)
        {
            byte[] psvSave = new byte[save.Length + 4];
            byte[] saltSeed;

            psvSave[1] = 0x56;
            psvSave[2] = 0x53;
            psvSave[3] = 0x50;
            psvSave[0x38] = 0x14;
            psvSave[0x3C] = 1;
            psvSave[0x44] = 0x84;
            psvSave[0x49] = 2;
            psvSave[0x5D] = 0x20;
            psvSave[0x60] = 3;
            psvSave[0x61] = 0x90;

            Array.Copy(save, 0x0A, psvSave, 0x64, 0x20);
            Array.Copy(BitConverter.GetBytes(save.Length - 0x80), 0, psvSave, 0x40, 4);
            Array.Copy(save, 0x80, psvSave, 0x84, save.Length - 0x80);

            using (SHA1 sha = SHA1.Create())
                saltSeed = sha.ComputeHash(psvSave);
            Array.Copy(saltSeed, 0, psvSave, 0x08, 0x14);
            Array.Copy(GetHmac(psvSave, saltSeed), 0, psvSave, 0x1C, 0x14);
            return psvSave;
        }

        //Recreate VGS header
        private byte[] getVGSheader()
        {
            byte[] vgsHeader = new byte[64];

            //Fill in the signature
            vgsHeader[0] = 0x56;       //V
            vgsHeader[1] = 0x67;       //g
            vgsHeader[2] = 0x73;       //s
            vgsHeader[3] = 0x4D;       //M

            vgsHeader[4] = 0x1;
            vgsHeader[8] = 0x1;
            vgsHeader[12] = 0x1;
            vgsHeader[17] = 0x2;

            return vgsHeader;
        }

        //Get the type of the save slots
        private void loadSlotTypes()
        {
            //Clear existing data
            saveType = new byte[15];

            for (int slotNumber = 0; slotNumber < 15; slotNumber++)
            {
                switch(headerData[slotNumber,0])
                {
                    default:        //Regular values have not been found, save is corrupted
                        saveType[slotNumber] = 7;
                        break;

                    case 0xA0:      //Formatted
                        saveType[slotNumber] = 0;
                        break;

                    case 0x51:      //Initial
                        saveType[slotNumber] = 1;
                        break;

                    case 0x52:      //Middle link
                        saveType[slotNumber] = 2;
                        break;

                    case 0x53:      //End link
                        saveType[slotNumber] = 3;
                        break;

                    case 0xA1:      //Initial deleted
                        saveType[slotNumber] = 4;
                        break;

                    case 0xA2:      //Middle link deleted
                        saveType[slotNumber] = 5;
                        break;

                    case 0xA3:      //End link deleted
                        saveType[slotNumber] = 6;
                        break;
                }

            }

        }

        //Load Save name, Product code and Identifier from the header data
        private void loadStringData()
        {
            //Temp array used for conversion
            byte[] tempByteArray;
            
            //Clear existing data
            saveProdCode = new string[15];
            saveIdentifier = new string[15];
            saveName = new string[15];

            for (int slotNumber = 0; slotNumber < 15; slotNumber++)
            {
                //Copy Product code
                tempByteArray = new byte[10];
                for (int byteCount = 0; byteCount < 10; byteCount++)
                    tempByteArray[byteCount] = headerData[slotNumber, byteCount + 12];

                //Convert Product Code from currently used codepage to UTF-16
                saveProdCode[slotNumber] = Encoding.Default.GetString(tempByteArray);


                //Copy Identifier
                tempByteArray = new byte[8];
                for (int byteCount = 0; byteCount < 8; byteCount++)
                    tempByteArray[byteCount] = headerData[slotNumber, byteCount + 22];

                //Convert Identifier from currently used codepage to UTF-16
                saveIdentifier[slotNumber] = Encoding.Default.GetString(tempByteArray);


                //Copy bytes from save data to temp array
                tempByteArray = new byte[64];
                for (int currentByte = 0; currentByte < 64; currentByte++)
                {
                    byte b = saveData[slotNumber, currentByte + 4];
                    if (currentByte % 2 == 0 && b == 0)
                    {
                        Array.Resize(ref tempByteArray, currentByte);
                        break;
                    }
                    tempByteArray[currentByte] = b;
                }

                //Convert save name from Shift-JIS to UTF-16 and normalize full-width characters
                saveName[slotNumber] = Encoding.GetEncoding(932).GetString(tempByteArray).Normalize(NormalizationForm.FormKC);

                //Check if the title converted properly, get ASCII if it didn't
                if (saveName[slotNumber] == null) saveName[slotNumber] = Encoding.Default.GetString(tempByteArray, 0, 32);
            }
        }

        //Load size of each slot in KB
        private void loadSaveSize()
        {
            //Clear existing data
            saveSize = new int[15];

            //Fill data for each slot
            for (int slotNumber = 0; slotNumber < 15; slotNumber++)
                saveSize[slotNumber] = (headerData[slotNumber, 4] | (headerData[slotNumber, 5]<<8) | (headerData[slotNumber, 6]<<16))/1024;
        }

        //Toggle deleted/undeleted status
        public void toggleDeleteSave(int slotNumber)
        {
            //Get all linked saves
            int[] saveSlots = findSaveLinks(slotNumber);

            //Cycle through each slot
            for (int i = 0; i < saveSlots.Length; i++)
            {
                //Check the save type
                switch (saveType[saveSlots[i]])
                {
                    default:            //Slot should not be deleted
                        break;

                    case 1:             //Regular save
                        headerData[saveSlots[i], 0] = 0xA1;
                        break;

                    case 2:             //Middle link
                        headerData[saveSlots[i], 0] = 0xA2;
                        break;

                    case 3:             //End link
                        headerData[saveSlots[i], 0] = 0xA3;
                        break;

                    case 4:             //Regular deleted save
                        headerData[saveSlots[i], 0] = 0x51;
                        break;

                    case 5:             //Middle link deleted
                        headerData[saveSlots[i], 0] = 0x52;
                        break;

                    case 6:             //End link deleted
                        headerData[saveSlots[i], 0] = 0x53;
                        break;
                }
            }

            //Reload data
            calculateXOR();
            loadSlotTypes();

            //Memory Card is changed
            changedFlag = true;
        }

        //Format save
        public void formatSave(int slotNumber)
        {
            //Get all linked saves
            int[] saveSlots = findSaveLinks(slotNumber);

            //Cycle through each slot
            for (int i = 0; i < saveSlots.Length; i++)
            {
                formatSlot(saveSlots[i]);
            }

            //Reload data
            calculateXOR();
            loadStringData();
            loadSlotTypes();
            loadRegion();
            loadSaveSize();
            loadPalette();
            loadIcons();
            loadIconFrames();

            //Set changedFlag to edited
            changedFlag = true;
        }

        //Find and return all save links
        public int[] findSaveLinks(int initialSlotNumber)
        {
            List<int> tempSlotList = new List<int>();
            int currentSlot = initialSlotNumber;

            //Maximum number of cycles is 15
            for (int i = 0; i < 15; i++)
            {
                //Add current slot to the list
                tempSlotList.Add(currentSlot);

                //Check if next slot pointer overflows
                if (currentSlot > 15) break;

                //Check if current slot is corrupted
                if (saveType[currentSlot] == 7) break;

                //Check if pointer points to the next save
                if (headerData[currentSlot, 8] == 0xFF) break;
                else currentSlot = headerData[currentSlot, 8];
            }

            //Return int array
            return tempSlotList.ToArray();
        }

        //Find and return continuous free slots
        private int[] findFreeSlots(int slotNumber, int slotCount)
        {
            List<int> tempSlotList = new List<int>();

            //Cycle through available slots
            for (int i = slotNumber; i < (slotNumber + slotCount); i++)
            {
                if (saveType[i] == 0) tempSlotList.Add(i);
                else break;

                //Exit if next save would be over the limit of 15
                if (slotNumber + slotCount > 15) break;
            }

            //Return int array
            return tempSlotList.ToArray();
        }

        //Return all bytes of the specified save
        public byte[] getSaveBytes(int slotNumber)
        {
            //Get all linked saves
            int[] saveSlots = findSaveLinks(slotNumber);

            //Calculate the number of bytes needed to store the save
            byte[] saveBytes = new byte[8320 + ((saveSlots.Length - 1) * 8192)];

            //Copy save header
            for (int i = 0; i < 128; i++)
                saveBytes[i] = headerData[saveSlots[0],i];

            //Copy save data
            for (int sNumber = 0; sNumber < saveSlots.Length; sNumber++)
            {
                for (int i = 0; i < 8192; i++)
                    saveBytes[128 + (sNumber * 8192) + i] = saveData[saveSlots[sNumber],i];
            }

            //Return save bytes
            return saveBytes;
        }

        //Input given bytes back to the Memory Card
        public bool setSaveBytes(int slotNumber, byte[] saveBytes, out int reqSlots)
        {
            //Number of slots to set
            int slotCount = (saveBytes.Length - 128) / 8192;
            int[] freeSlots = findFreeSlots(slotNumber, slotCount);
            int numberOfBytes = slotCount * 8192; ;
            reqSlots = slotCount;

            //Check if there are enough free slots for the operation
            if (freeSlots.Length < slotCount) return false;

            //Place header data
            for (int i = 0; i < 128; i++)
                headerData[freeSlots[0], i] = saveBytes[i];

            //Place save size in the header
            headerData[freeSlots[0], 4] = (byte)(numberOfBytes & 0xFF);
            headerData[freeSlots[0], 5] = (byte)((numberOfBytes & 0xFF00) >> 8);
            headerData[freeSlots[0], 6] = (byte)((numberOfBytes & 0xFF0000) >> 16);

            //Place save data(cycle through each save)
            for (int i = 0; i < slotCount; i++)
            {
                //Set all bytes
                for (int byteCount = 0; byteCount < 8192; byteCount++)
                {
                    saveData[freeSlots[i], byteCount] = saveBytes[128 + (i * 8192) + byteCount];
                }
            }

            //Recreate header data
            //Set pointer to all slots except the last
            for (int i = 0; i < (freeSlots.Length - 1); i++)
            {
                headerData[freeSlots[i], 0] = 0x52;
                headerData[freeSlots[i], 8] = (byte)freeSlots[i + 1];
                headerData[freeSlots[i], 9] = 0x00;
            }

            //Add final slot pointer to the last slot in the link
            headerData[freeSlots[freeSlots.Length - 1], 0] = 0x53;
            headerData[freeSlots[freeSlots.Length - 1], 8] = 0xFF;
            headerData[freeSlots[freeSlots.Length - 1], 9] = 0xFF;

            //Add initial saveType to the first slot
            headerData[freeSlots[0], 0] = 0x51;

            //Reload data
            calculateXOR();
            loadStringData();
            loadSlotTypes();
            loadRegion();
            loadSaveSize();
            loadPalette();
            loadIcons();
            loadIconFrames();

            //Set changedFlag to edited
            changedFlag = true;

            return true;
        }

        //Set Product code, Identifier and Region in the header of the selected save
        public void setHeaderData(int slotNumber, string sProdCode, string sIdentifier, ushort sRegion)
        {
            //Temp array used for manipulation
            byte[] tempByteArray;

            //Merge Product code and Identifier
            string headerString = sProdCode + sIdentifier;

            //Convert string from UTF-16 to currently used codepage
            tempByteArray = Encoding.Convert(Encoding.Unicode, Encoding.Default, Encoding.Unicode.GetBytes(headerString));

            //Clear existing data from header
            for (int byteCount = 0; byteCount < 20; byteCount++)
                headerData[slotNumber, byteCount + 10] = 0x00;

            //Inject new data to header
            for (int byteCount = 0; byteCount < headerString.Length; byteCount++)
                headerData[slotNumber, byteCount + 12] = tempByteArray[byteCount];

            //Add region to header
            headerData[slotNumber, 10] = (byte)(sRegion & 0xFF);
            headerData[slotNumber, 11] = (byte)(sRegion >> 8);

            //Reload data
            loadStringData();
            loadRegion();

            //Calculate XOR
            calculateXOR();

            //Set changedFlag to edited
            changedFlag = true;
        }

        //Load region of the saves
        private void loadRegion()
        {
            //Clear existing data
            saveRegion = new ushort[15];

            //Cycle trough each slot
            for (int slotNumber = 0; slotNumber < 15; slotNumber++)
            {
                //Store save region
                saveRegion[slotNumber] = (ushort)((headerData[slotNumber, 11] << 8) | headerData[slotNumber, 10]);
            }
        }

        //Load palette
        private void loadPalette()
        {
            int redChannel = 0;
            int greenChannel = 0;
            int blueChannel = 0;
            int colorCounter = 0;
            int blackFlag = 0;

            //Clear existing data
            iconPalette = new Color[15,16];

            //Cycle through each slot on the Memory Card
            for (int slotNumber = 0; slotNumber < 15; slotNumber++)
            {
                //Reset color counter
                colorCounter = 0;

                //Fetch two bytes at a time
                for (int byteCount = 0; byteCount < 32; byteCount += 2)
                {
                    redChannel = (saveData[slotNumber, byteCount + 96] & 0x1F) << 3;
                    greenChannel = ((saveData[slotNumber, byteCount + 97] & 0x3) << 6) | ((saveData[slotNumber, byteCount + 96] & 0xE0) >> 2);
                    blueChannel = ((saveData[slotNumber, byteCount + 97] & 0x7C) << 1);
                    blackFlag = (saveData[slotNumber, byteCount + 97] & 0x80);
                    
                    //Get the color value
                    if ((redChannel | greenChannel | blueChannel | blackFlag) == 0) iconPalette[slotNumber, colorCounter] = Color.Transparent;
                    else iconPalette[slotNumber, colorCounter] = Color.FromArgb(redChannel, greenChannel, blueChannel);
                    colorCounter++;
                }
            }
        }

        //Load the icons
        private void loadIcons()
        {
            int byteCount = 0;

            //Clear existing data
            iconData = new Bitmap[15,3]; 

            //Cycle through each slot
            for (int slotNumber = 0; slotNumber < 15; slotNumber++)
            {
                //Each save has 3 icons (some are data but those will not be shown)
                for (int iconNumber = 0; iconNumber < 3; iconNumber++)
                {
                    iconData[slotNumber, iconNumber] = new Bitmap(16, 16);
                    byteCount = 128 + ( 128 * iconNumber);

                    for (int y = 0; y < 16; y++)
                    {
                        for (int x = 0; x < 16; x += 2)
                        {
                            iconData[slotNumber, iconNumber].SetPixel(x, y, iconPalette[slotNumber, saveData[slotNumber, byteCount] & 0xF]);
                            iconData[slotNumber, iconNumber].SetPixel(x + 1, y, iconPalette[slotNumber, saveData[slotNumber, byteCount] >> 4]);
                            byteCount++;
                        }
                    }
                }
            }
        }

        //Get icon data as bytes
        public byte[] getIconBytes(int slotNumber)
        {
            byte[] iconBytes = new byte[416];

            //Copy bytes from the given slot
            for (int i = 0; i < 416; i++)
                iconBytes[i] = saveData[slotNumber, i + 96];

            return iconBytes;
        }

        //Set icon data to saveData
        public void setIconBytes(int slotNumber, byte[] iconBytes)
        {
            //Set bytes from the given slot
            for (int i = 0; i < 416; i++)
                saveData[slotNumber, i + 96] = iconBytes[i];

            //Reload data
            loadPalette();
            loadIcons();

            //Set changedFlag to edited
            changedFlag = true;
        }

        //Load icon frames
        private void loadIconFrames()
        {
            //Clear existing data
            iconFrames = new int[15];

            //Cycle through each slot
            for (int slotNumber = 0; slotNumber < 15; slotNumber++)
            {
                switch(saveData[slotNumber,2])
                {
                    default:        //No frames (save data is probably clean)
                        break;

                    case 0x11:      //1 frame
                        iconFrames[slotNumber] = 1;
                        break;

                    case 0x12:      //2 frames
                        iconFrames[slotNumber] = 2;
                        break;

                    case 0x13:      //3 frames
                        iconFrames[slotNumber] = 3;
                        break;
                }
            }
        }

        //Load GME comments
        private void loadGMEComments()
        {
            //Clear existing data
            saveComments = new string[15];

            //Load comments from gmeHeader to saveComments
            byte[] tempByteArray;
            for (int slotNumber = 0; slotNumber < 15; slotNumber++)
            {
                tempByteArray = new byte[256];

                for (int byteCount = 0; byteCount < 256; byteCount++)
                    tempByteArray[byteCount] = gmeHeader[byteCount+64 + (256*slotNumber)];

                //Set save comment for each slot
                saveComments[slotNumber] = Encoding.Default.GetString(tempByteArray);
            }
        }

        //Calculate XOR checksum
        private void calculateXOR()
        {
            byte XORchecksum = 0;

            //Cycle through each slot
            for (int slotNumber = 0; slotNumber < 15; slotNumber++)
            {
                //Set default value
                XORchecksum = 0;

                //Count 127 bytes
                for (int byteCount = 0; byteCount < 126; byteCount++)
                    XORchecksum ^= headerData[slotNumber, byteCount];

                //Store checksum in 128th byte
                headerData[slotNumber, 127] = XORchecksum;
            }
        }

        //Format a specified slot (Data MUST be reloaded after the use of this function)
        private void formatSlot(int slotNumber)
        {
            //Clear headerData
            for (int byteCount = 0; byteCount < 128; byteCount++)
                headerData[slotNumber,byteCount] = 0x00;

            //Clear saveData
            for (int byteCount = 0; byteCount < 8192; byteCount++)
                saveData[slotNumber, byteCount] = 0x00;

            //Clear GME comment for selected slot
            saveComments[slotNumber] = new string('\0',256);

            //Place default values in headerData
            headerData[slotNumber, 0] = 0xA0;
            headerData[slotNumber, 8] = 0xFF;
            headerData[slotNumber, 9] = 0xFF;
        }

        //Format a complete Memory Card
        private void formatMemoryCard()
        {
            //Format each slot in Memory Card
            for (int slotNumber = 0; slotNumber < 15; slotNumber++)
                formatSlot(slotNumber);

            //Reload data
            calculateXOR();
            loadStringData();
            loadSlotTypes();
            loadRegion();
            loadSaveSize();
            loadPalette();
            loadIcons();
            loadIconFrames();

            //Set changedFlag to edited
            changedFlag = true;
        }

        //Save single save to the given filename
        public bool saveSingleSave(string fileName, int slotNumber, int singleSaveType)
        {
            BinaryWriter binWriter;
            byte[] outputData = getSaveBytes(slotNumber);

            //Check if the file is allowed to be opened for writing
            try
            {
                binWriter = new BinaryWriter(File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));
            }
            catch (Exception)
            {
                return false;
            }

            //Check what kind of file to output according to singleSaveType
            switch (singleSaveType)
            {
                default:        //Action Replay single save
                    byte[] arHeader = new byte[54];
                    byte[] arName = null;

                    //Copy header data to arHeader
                    for (int byteCount = 0; byteCount < 22; byteCount++)
                        arHeader[byteCount] = headerData[slotNumber, byteCount + 10];

                    //Convert save name to bytes
                    arName = Encoding.Default.GetBytes(saveName[slotNumber]);

                    //Copy save name to arHeader
                    for (int byteCount = 0; byteCount < arName.Length; byteCount++)
                        arHeader[byteCount + 21] = arName[byteCount];

                    binWriter.Write(arHeader);
                    binWriter.Write(outputData, 128, outputData.Length - 128);
                    break;

                case 2:         //MCS single save
                    binWriter.Write(outputData);
                    break;

                case 3:         //RAW single save
                    binWriter.Write(outputData, 128, outputData.Length - 128);
                    break;
                case 4:         //PS3 signed save
                    binWriter.Write(MakePsvSave(outputData));
                    break;
            }

            //File is sucesfully saved, close the stream
            binWriter.Close();

            return true;
        }

        //Import single save to the Memory Card
        public bool openSingleSave(string fileName, int slotNumber, out int requiredSlots)
        {
            requiredSlots = 0;
            string tempString = null;
            byte[] inputData;
            byte[] finalData;
            BinaryReader binReader;

            //Check if the file is allowed to be opened
            try
            {
                binReader = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            }
            catch (Exception)
            {
                return false;
            }

            //Put data into temp array
            inputData = binReader.ReadBytes(123008);

            //File is sucesfully read, close the stream
            binReader.Close();

            //Check the format of the save and if it's supported load it (filter illegal characters from types)
            if (inputData.Length > 3) tempString = Encoding.ASCII.GetString(inputData, 0, 2).Trim((char)0x0);

            switch (tempString)
            {
                default:            //Action Replay single save

                    //Check if this is really an AR save (check for SC magic)
                    if (!(inputData[0x36] == 0x53 && inputData[0x37] == 0x43)) return false;

                    finalData = new byte[inputData.Length + 74];

                    //Recreate save header
                    finalData[0] = 0x51;        //Q

                    for (int i = 0; i < 20; i++)
                        finalData[i + 10] = inputData[i];

                    //Copy save data
                    for (int i = 0; i < inputData.Length - 54; i++)
                        finalData[i + 128] = inputData[i + 54];
                    break;

                case "Q":           //MCS single save
                    finalData = inputData;
                    break;

                case "SC":          //RAW single save
                case "sc":          //Also valid as seen with MMX4 save
                    finalData = new byte[inputData.Length + 128];
                    byte[] singleSaveHeader = Encoding.Default.GetBytes(Path.GetFileName(fileName));

                    //Recreate save header
                    finalData[0] = 0x51;        //Q

                    for (int i = 0; i < 20 && i < singleSaveHeader.Length; i++)
                        finalData[i + 10] = singleSaveHeader[i];

                    //Copy save data
                    for (int i = 0; i < inputData.Length; i++)
                        finalData[i + 128] = inputData[i];
                    break;

                case "V":           //PSV single save (PS3 virtual save)
                    //Check if this is a PS1 type save
                    if(inputData[60] != 1) return false;
                    
                    finalData = new byte[inputData.Length - 4];

                    //Recreate save header
                    finalData[0] = 0x51;        //Q

                    for (int i = 0; i < 20; i++)
                        finalData[i + 10] = inputData[i + 100];

                    //Copy save data
                    for (int i = 0; i < inputData.Length - 132; i++)
                        finalData[i + 128] = inputData[i + 132];
                    break;
            }

            //Import the save to Memory Card
            if (setSaveBytes(slotNumber, finalData, out requiredSlots)) return true;
            else return false;
        }

        //Save Memory Card to the given filename
        public bool saveMemoryCard(string fileName, int memoryCardType, bool fixData)
        {
            BinaryWriter binWriter = null;

            //Check if the file is allowed to be opened for writing
            try
            {
                binWriter = new BinaryWriter(File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));
            }
            catch (Exception)
            {
                return false;
            }

            //Prepare data for saving
            loadDataToRawCard(fixData);

            //Check what kind of file to output according to memoryCardType
            switch (memoryCardType)
            {
                default:        //Raw Memory Card
                    binWriter.Write(rawMemoryCard);
                    break;

                case 2:         //GME Memory Card
                    fillGmeHeader();
                    binWriter.Write(gmeHeader);
                    binWriter.Write(rawMemoryCard);
                    break;

                case 3:         //VGS Memory Card
                    binWriter.Write(getVGSheader());
                    binWriter.Write(rawMemoryCard);
                    break;

                case 4:         //VMP Memory Card
                    binWriter.Write(MakeVmpCard(rawMemoryCard));
                    break;
                case 5:         //MCX Memory Card
                    binWriter.Write(MakeMcxCard(rawMemoryCard));
                    break;
            }

            //Store the location of the Memory Card
            cardLocation = fileName;

            //Store the filename of the Memory Card
            cardName = Path.GetFileNameWithoutExtension(fileName);

            //Set changedFlag to saved
            changedFlag = false;

            //File is sucesfully saved, close the stream
            binWriter.Close();

            return true;
        }

        //Save (export) Memory Card to a given byte stream
        public byte[] saveMemoryCardStream(bool fixData)
        {
            //Prepare data for saving
            loadDataToRawCard(fixData);

            //Return complete Memory Card data
            return rawMemoryCard;
        }


        //Open memory card from the given byte stream
        public void openMemoryCardStream(byte[] memCardData, bool fixData)
        {
            //Set the reference for the recieved data
            rawMemoryCard = memCardData;

            //Load Memory Card data from raw card
            loadDataFromRawCard();

            cardName = "Untitled";

            if(fixData) calculateXOR();
            loadStringData();
            loadGMEComments();
            loadSlotTypes();
            loadRegion();
            loadSaveSize();
            loadPalette();
            loadIcons();
            loadIconFrames();

            //Since the stream is of the unknown origin Memory Card is treated as edited
            changedFlag = true;
        }

        //Open Memory Card from the given filename (return error message if operation is not sucessfull)
        public string openMemoryCard(string fileName, bool fixData)
        {
            //Check if the Memory Card should be opened or created
            if (fileName != null)
            {
                byte[] tempData = new byte[134976];
                string tempString;
                int startOffset;
                BinaryReader binReader;

                //Check if the file is allowed to be opened
                try
                {
                    binReader = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                }
                catch (Exception errorException)
                {
                    //Return the error description
                    return errorException.Message;
                }

                //Put data into temp array
                binReader.BaseStream.Read(tempData, 0, 134976);
                
                //File is sucesfully read, close the stream
                binReader.Close();

                //Store the location of the Memory Card
                cardLocation = fileName;

                //Store the filename of the Memory Card
                cardName = Path.GetFileNameWithoutExtension(fileName);

                //Check the format of the card and if it's supported load it (filter illegal characters from types)
                tempString = Encoding.ASCII.GetString(tempData, 0, 11).Trim((char)0x0, (char)0x1, (char)0x3F);
                switch (tempString)
                {
                    default:                //File type is not supported or is MCX
                        if (IsMcxCard(tempData)) {
                            tempData = DecryptMcxCard(tempData);
                            startOffset = 128;
                            cardType = 5;
                            break;
                        }
                        return "'" + cardName + "' is not a supported Memory Card format.";
                    case "MC":              //Standard raw Memory Card
                        startOffset = 0;
                        cardType = 1;
                        break;

                    case "123-456-STD":     //DexDrive GME Memory Card
                        startOffset = 3904;
                        cardType = 2;

                        //Copy input data to gmeHeader
                        for (int i = 0; i < 3904; i++) gmeHeader[i] = tempData[i];
                        break;

                    case "VgsM":            //VGS Memory Card
                        startOffset = 64;
                        cardType = 3;
                        break;

                    case "PMV":             //PSP virtual Memory Card
                        startOffset = 128;
                        cardType = 4;
                        break;
                }

                //Copy data to rawMemoryCard array with offset from input data
                Array.Copy(tempData, startOffset, rawMemoryCard, 0, 131072);
                //Load Memory Card data from raw card
                loadDataFromRawCard();
            }
            // Memory Card should be created
            else
            {
                cardName = "Untitled";
                loadDataToRawCard(true);
                formatMemoryCard();

                //Set changedFlag to false since this is created card
                changedFlag = false;
            }

            //Calculate XOR checksum (in case if any of the saveHeaders have corrputed XOR)
            if(fixData) calculateXOR();

            //Convert various Memory Card data to strings
            loadStringData();

            //Load GME comments (if card is any other type comments will be null)
            loadGMEComments();

            //Load slot descriptions (types)
            loadSlotTypes();

            //Load region data
            loadRegion();

            //Load size data
            loadSaveSize();

            //Load icon palette data as Color values
            loadPalette();

            //Load icon data to bitmaps
            loadIcons();

            //Load number of frames
            loadIconFrames();

            //Everything went well, no error messages
            return null;
        }
    }
}
