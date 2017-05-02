//New PS1 Memory Card class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MemcardRex
{
    public class ps1mc
    {
        public enum cardTypes {container, mcr, gme, vgs, vmp};

        //Properties of a Memory Card
        public string title = "Untitled";
        public string path = null;
        public cardTypes type = cardTypes.container;
        public bool changed = false;
        public List<singleSave> saves;

        //Properties of a single save
        public struct singleSave
        {
            public string title;
            public string region;
            public byte[] icons;
            public string productCode;
            public string identifier;
            public byte[] data;
            public bool active;
            public int size;
            public string comments;
        };

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

            if (fileName == null) return "No file specified";

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

            return null;
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
