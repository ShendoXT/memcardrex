//New PS1 Memory Card class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace MemcardRex
{
    //Properties of a single save
    public class singleSave
    {
        public string title = "";
        public string region = "";
        public Bitmap[] icons;
        public string productCode = "";
        public string identifier = "";
        public byte[] data;
        public bool active = true;
        public int size = 0;
        public string comments = "";

        //Return save title in UTF-16, ASCII or auto by region
        public string saveTitle(int option)
        {
            charConverter SJISC = new charConverter();

            switch (option)
            {
                case 0:     //ASCII
                    return SJISC.convertSJIStoASCII(this.data);
                    //break;

                case 1:     //UTF-16
                    return this.title;

                case 2:     //Auto
                    break;
            }

            //Fail-safe, should't be reached
            return "";
        }

        //Return size in kilobytes
        public string sizeKB()
        {
            return (size / 1024).ToString() + " KB";
        }

        //Return size in slots (blocks)
        public string sizeSlot()
        {
            int slotCount = size / 8192;

            if (slotCount == 1) return slotCount.ToString() + " slot";
            else return slotCount.ToString() + " slots";
        }
    };

    public class ps1mc
    {
        public enum cardTypes {container, mcr, gme, vgs, vmp};

        //Properties of a Memory Card
        public string title = "Untitled";
        public string path = null;
        public cardTypes type = cardTypes.container;
        public bool changed = false;
        public List<singleSave> saves = new List<singleSave>();

        //Read entire file to memory
        public byte[] ReadAllBytes(BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }

        }

        //Create a blank Memory Card
        public bool createMemoryCard()
        {
            return true;
        }

        //Open Memory Card from the given filename (return error message if operation is not sucessfull)
        public string openMemoryCard(string fileName)
        {
            BinaryReader binReader = null;
            byte[] cardArray;

            if (fileName == null) return null;

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

            //Read file to memory
            cardArray = ReadAllBytes(binReader);

            //File is sucesfully read, close the stream
            binReader.Close();

            //Store the location of the Memory Card
            path = fileName;

            //Store the filename of the Memory Card
            title = Path.GetFileNameWithoutExtension(fileName);

            //Continue opening the card from the given stream
            return openMemoryCard(cardArray);
        }

        //Open Memory Card from the given byte stream
        public string openMemoryCard(byte[] data)
        {
            int startOffset;
            string magicId;
            int saveCount = 15;

            //Check the format of the card and if it's supported load it (filter illegal characters from types)
            magicId = Encoding.ASCII.GetString(data, 0, 11).Trim((char)0x0, (char)0x1, (char)0x3F);
            switch (magicId)
            {
                default:                //File type is not supported
                    return "'" + title + "' is not a supported Memory Card format.";

                case "MC":              //Standard raw Memory Card
                    startOffset = 0;
                    type = cardTypes.mcr;
                    break;

                case "123-456-STD":     //DexDrive GME Memory Card
                    startOffset = 3904;
                    type = cardTypes.gme;

                    //Copy input data to gmeHeader
                    //for (int i = 0; i < 3904; i++) gmeHeader[i] = tempData[i];
                    break;

                case "VgsM":            //VGS Memory Card
                    startOffset = 64;
                    type = cardTypes.vgs;
                    break;

                case "PMV":             //PSP virtual Memory Card
                    startOffset = 128;
                    type = cardTypes.vmp;
                    break;
            }


            //Go through each block and fetch saves
            for(int i = 0; i < saveCount; i++)
            {
                singleSave newSingleSave = new singleSave();
                int currentOffset = (128 * (i + 1)) + startOffset;
                string tempTitle = "";
                int titleIndex = 0;
                byte[] iconData = new byte[512];

                switch (data[currentOffset])
                {
                    case 0xA1:      //Deleted regular save
                    case 0x51:      //Regular save
                        if (data[currentOffset] == 0xA1) newSingleSave.active = false;

                        newSingleSave.size = data[currentOffset + 4] | (data[currentOffset + 5] << 8) | (data[currentOffset + 6] << 16);

                        //Get title and trim everything after a null character
                        tempTitle = Encoding.GetEncoding(932).GetString(data, (8192 * (i + 1)) + startOffset + 4, 0x40);
                        titleIndex = tempTitle.IndexOf("\0");
                        if (titleIndex > 0) newSingleSave.title = tempTitle.Substring(0, titleIndex);
                        else newSingleSave.title = tempTitle;

                        newSingleSave.region = Encoding.Default.GetString(data, currentOffset + 10, 2);
                        newSingleSave.productCode = Encoding.Default.GetString(data, currentOffset + 12, 10);
                        newSingleSave.identifier = Encoding.Default.GetString(data, currentOffset + 22, 8);

                        Array.Copy(data, (8192 * (i + 1)) + startOffset, iconData, 0, 512);
                        newSingleSave.icons = getIcons(iconData);

                        //Get save data
                        newSingleSave.data = new byte[8192];
                        Array.Copy(data, (8192 * (i + 1)) + startOffset, newSingleSave.data, 0, 8192);

                        saves.Add(newSingleSave);       //Push new save to the Memory Card
                        break;
                }


            }


            return null;
        }


        //Fetch icons from save data
        public Bitmap[] getIcons(byte[] data)
        {
            int frameCount = 1;
            Bitmap canvas = new Bitmap(16, 16);
            List<Bitmap> outputData = new List<Bitmap>();

            if (data[2] == 0x12) frameCount = 2;
            else if (data[2] == 0x13) frameCount = 3;

            var colorCount = 0;
            var byteCount2 = 128;
            List<Color> paletteData = new List<Color>();

            //Fetch two bytes at a time
            for (var byteCount = 0; byteCount < 32; byteCount += 2)
            {
                int redChannel, greenChannel, blueChannel, alphaChannel;

                redChannel = (data[byteCount + 96] & 0x1F) << 3;
                greenChannel = ((data[byteCount + 97] & 0x3) << 6) | ((data[byteCount + 96] & 0xE0) >> 2);
                blueChannel = ((data[byteCount + 97] & 0x7C) << 1);
                alphaChannel = (data[byteCount + 97] & 0x80);

                paletteData.Add(Color.FromArgb(redChannel, greenChannel, blueChannel));
            }

            colorCount = 0;

            for (var i = 0; i < frameCount; i++)
            {
                canvas = new Bitmap(16, 16);
                byteCount2 = 128 * (i + 1);
                colorCount = 0;

                //Construct the icon
                for (var y = 0; y < 16; y++)
                {
                    for (var x = 0; x < 16; x += 2)
                    {

                        var leftIndex = data[byteCount2] & 0x0F;
                        var rightIndex = (data[byteCount2] & 0xF0) >> 4;

                        canvas.SetPixel(x, y, paletteData[leftIndex]);
                        canvas.SetPixel(x + 1, y, paletteData[rightIndex]);

                        byteCount2++;
                        colorCount += 6;
                    }
                }

                outputData.Add(canvas);
            }

            //Return array of bitmaps
            return outputData.ToArray();
        }

        //Export Memory Card to file
        public bool exportMemoryCard(string fileName)
        {
            return true;
        }

        //Export Memory Card as a byte stream
        public byte[] exportMemoryCard()
        {
            return new byte[0];
        }

    }
}
