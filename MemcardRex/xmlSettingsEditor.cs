//XML Writer/Reader class for MemcardRex
//Shendo 2012

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MemcardRex
{
    class xmlSettingsEditor
    {
        //Writer and reader declarations
        XmlTextWriter xmlWriter = null;
        XmlTextReader xmlReader = null;

        //List of all the XML elements and values
        List<string> xmlElements = new List<string>();
        List<string> xmlValues = new List<string>();

        //In order to read an XML file this function needs to be called
        public void openXmlReader(string fileName)
        {
            xmlReader = new XmlTextReader(fileName);

            //Read every node in the XML file
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        xmlElements.Add(xmlReader.Name);
                        break;

                    case XmlNodeType.Text:
                        xmlValues.Add(xmlReader.Value);
                        break;
                }
            }

            //Cleanly close the file
            xmlReader.Close();
        }

        //Read a key and a value from the XML file
        public string readXmlEntry(string key)
        {
            string returnString = null;

            //Check if the key element exists
            if (xmlElements.Contains(key))
            {
                returnString = xmlValues[xmlElements.IndexOf(key) - 1];
            }

            return returnString;
        }

        //Read a key and an int value from the XML file
        public int readXmlEntryInt(string key, int minValue, int maxValue)
        {
            int returnInt = 0;
            string returnString = null;

            //Check if the key element exists
            if (xmlElements.Contains(key))
            {
                returnString = xmlValues[xmlElements.IndexOf(key) - 1];
            }

            if (int.TryParse(returnString, out returnInt))
            {
                //Check if the value is lower then the minimum given value
                if (returnInt < minValue) returnInt = minValue;

                //Check if the value exceeds the maximum given value
                if (returnInt > maxValue) returnInt = maxValue;
            }

            return returnInt;
        }

        //In order to write to XML file this function needs to be called
        public void openXmlWriter(string fileName, string applicationComment)
        {
            xmlWriter = new XmlTextWriter(fileName, Encoding.UTF8);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteComment(applicationComment);
            xmlWriter.WriteStartElement("Settings");
        }

        //Write a value in the associated key
        public void writeXmlEntry(string key, string value)
        {
            xmlWriter.WriteStartElement(key);
            xmlWriter.WriteString(value);
            xmlWriter.WriteEndElement();
        }

        //When program is done writing to XML file clean close is needed
        public void closeXmlWriter()
        {
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }
    }
}
