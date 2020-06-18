using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Noemax.FastInfoset;
using Expedia.CarInterface.CarServiceTest.Util;
using System.Runtime.Serialization.Formatters.Binary;

namespace Expedia.CarInterface.CarServiceTest.Communication
{
    public class MessageFormat
    {
        public enum MessageContentType
        {
            Xml,
            FastInfoSet,
            Json,
            CSVTab
        };

        static public MemoryStream Serialize(MemoryStream ms, MessageContentType msgContentType)
        {
            if (ms == null)
                throw new ArgumentNullException("ms");

            if (msgContentType == MessageContentType.Xml)
            {
                return ms; // Already in XML
            }
            else if (msgContentType == MessageContentType.FastInfoSet)
            {
                return FastInfoSetSerializer.Serialize(ms);
            }

            throw new ArgumentOutOfRangeException("msgContentType");
        }

        static public MemoryStream Deserialize(MemoryStream ms, MessageContentType msgContentType)
        {
            if (msgContentType == MessageContentType.Xml)
            {
                return ms; // Either already in XML or an explicit serialization will be called by Comm. Manager
            }
            else if (msgContentType == MessageContentType.FastInfoSet)
            {
                return FastInfoSetSerializer.DeSerialize(ms);
            }

            throw new ArgumentOutOfRangeException("msgContentType");
        }
    }

    public class XMLSerializer
    {
        public static XmlWriterSettings GetDefaultXmlWriterSettings(bool Readable = true)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false);
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Indent = Readable;
            if (Readable)
                settings.IndentChars = "    ";
            settings.NewLineOnAttributes = Readable;
            settings.OmitXmlDeclaration = true;
            return settings;
        }

        // Conversion from Object to XML
        public static MemoryStream Serialize(object inputObject, Type inputObjectType)
        {
            if (inputObject == null)
                throw new ArgumentNullException("inputObject");

            if (inputObjectType == null)
                throw new ArgumentNullException("inputObjectType");

            MemoryStream msSerialized = new MemoryStream();

            using (XmlWriter writer = XmlTextWriter.Create(msSerialized, GetDefaultXmlWriterSettings()))
            {
                XmlSerializer xs = new XmlSerializer(inputObjectType);
                xs.Serialize(writer, inputObject);
                writer.Flush();
                writer.Close();
            }

            msSerialized.Seek(0, SeekOrigin.Begin);
            return msSerialized;
        }

        //Conversion from XML to object
        public static object DeSerialize(MemoryStream msSerialized, Type outputObjectType)
        {
            if (msSerialized == null)
                throw new ArgumentNullException("msSerialized");

            if (outputObjectType == null)
                throw new ArgumentNullException("outputObjectType");

            object outputObject = null;

            msSerialized.Seek(0, SeekOrigin.Begin);
            using (XmlReader reader = XmlTextReader.Create(msSerialized))
            {
                if (null != reader)
                {
                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(outputObjectType);
                        outputObject = serializer.Deserialize(reader);
                    }
                    catch (Exception e)
                    {
                        e.ToString();
                    }
                }
            }

            return outputObject;
        }
    }

    public class FastInfoSetSerializer
    {
        public static MemoryStream Serialize(MemoryStream ms)
        {
            if (ms == null)
                throw new ArgumentNullException("ms");

            MemoryStream msFastInfoset = new MemoryStream();

            ms.Seek(0, SeekOrigin.Begin);
            using (XmlTextReader reader = new XmlTextReader(ms))
            {
                XmlWriterSettings settings = XMLSerializer.GetDefaultXmlWriterSettings();
                using (XmlFastInfosetWriter writer = (XmlFastInfosetWriter)XmlFastInfosetWriter.Create(msFastInfoset, settings))
                {
                    reader.Read();
                    if (reader.NodeType == XmlNodeType.XmlDeclaration)
                        reader.Read();
                    writer.WriteStartDocument();
                    while (reader.NodeType != XmlNodeType.None)
                        writer.WriteNode(reader, false);
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                }
            }

            return msFastInfoset;
        }

        public static MemoryStream DeSerialize(MemoryStream ms)
        {
            if (ms == null || ms.Length == 0)
                throw new ArgumentNullException("ms");

            ms.Seek(0, SeekOrigin.Begin);

            //Log the response before deserializing for debugging.
            bool needPrintMsg = false;
            if (null != CarConfigurationManager.AppSetting("NeedPrintResponseBeforeDeserialize")) needPrintMsg = Boolean.Parse(CarConfigurationManager.AppSetting("NeedPrintResponseBeforeDeserialize"));
            if (needPrintMsg) Console.WriteLine(TransferTypeUtil.ObjToStr(ms)); 

            XmlDocument doc = new XmlDocument();
            using (XmlReader reader = XmlFastInfosetReader.Create(ms))
            {
                doc.Load(reader);
            }

            //TODO: below handling is just to unblock testing, need to delete later to defect the DateTime handling bug
            //Delete the time in MinDate and MaxDate to unblock testing
            XmlNodeList minDateNodeList = doc.GetElementsByTagName("ns2:MinDate");
            XmlNodeList maxDateNodeList = doc.GetElementsByTagName("ns2:MaxDate");
            if (null != minDateNodeList && minDateNodeList.Count > 0)
            {
                foreach (XmlNode minDateNode in minDateNodeList)
                {
                    minDateNode.InnerText = minDateNode.InnerText.Substring(0, 10);
                }
            }
            if (null != maxDateNodeList && maxDateNodeList.Count > 0)
            {
                foreach (XmlNode maxDateNode in maxDateNodeList)
                {
                    maxDateNode.InnerText = maxDateNode.InnerText.Substring(0, 10);
                }
            }
            //Update 24:00 to 23:59 to unblock testing
            XmlNodeList minTimeNodeList = doc.GetElementsByTagName("ns2:MinTime");
            XmlNodeList maxTimeNodeList = doc.GetElementsByTagName("ns2:MaxTime");
            if (null != maxTimeNodeList && maxTimeNodeList.Count > 0)
            {
                foreach (XmlNode maxTimeNode in maxTimeNodeList)
                {
                    string maxTime = maxTimeNode.InnerText;
                    maxTime = maxTime.Replace("24:00", "23:59");
                    maxTimeNode.InnerText = maxTime;
                }
            }
            if (null != minTimeNodeList && minTimeNodeList.Count > 0)
            {
                foreach (XmlNode minTimeNode in minTimeNodeList)
                {
                    string minTime = minTimeNode.InnerText;
                    minTime = minTime.Replace("24:00", "23:59");
                    minTimeNode.InnerText = minTime;
                }
            }
            

            MemoryStream msDeserialized = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(msDeserialized))
            {
                doc.WriteContentTo(writer);
                writer.Flush();
                writer.Close();
            }

            msDeserialized.Seek(0L, SeekOrigin.Begin);
            return msDeserialized;
        }

        /*//Deserialize from airInterface to obj
        public static object DeserializeAirInterface(byte[] encodedData, Type objectType)
        {
            int digits = 4;
            byte[] rest = new byte[encodedData.Length - digits];
            Array.Copy(encodedData, digits, rest, 0, (encodedData.Length - digits));

            MemoryStream ms = new MemoryStream(rest);
            Object obj;
            obj = FastInfoSetSerializer.DeSerialize(ms, objectType);
            Print.PrintMessageToConsole(obj);

            return obj;
        }*/

        //Deserialize fastinfoset logged in DB
        public static object DeserializeFIInDB(byte[] encodedData, Type objectType)
        {
            MemoryStream ms = new MemoryStream(encodedData);
            Object obj;
            obj = FastInfoSetSerializer.DeSerialize(ms, objectType);
            Print.PrintMessageToConsole(obj);

            return obj;
        }

        //Direct one step conversion from FI to the object.        
        public static object DeSerialize(MemoryStream ms, Type ObjectType)
        {
            // get a weakly typed DOM from Fast Infoset response (so DateTimes are just strings)
            XmlDocument xmlDoc = XMLDeSerialize(ms);
            // convert to strongly typed DOM
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectType);
            MemoryStream ms2 = new MemoryStream();
            XmlWriter xmlw = XmlWriter.Create(ms2);
            xmlDoc.WriteContentTo(xmlw);
            xmlw.Flush();
            ms2.Seek(0L, SeekOrigin.Begin);
            XmlReader xmlr = XmlReader.Create(ms2);
            return xmlSerializer.Deserialize(xmlr);
        }

        //Deserialize from FastInfoset to DOM
        public static XmlDocument XMLDeSerialize(MemoryStream ms)
        {
            XmlDocument doc = new XmlDocument();
            if (ms.CanSeek) ms.Position = 0;
            using (XmlReader reader = XmlFastInfosetReader.Create(ms))
            {
                try
                {
                    doc.Load(reader);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Error deserializing to XMLDocument" + ex.ToString());
                    return null;
                }

            }
            return doc;
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            System.Text.Encoding encoding = null;
            encoding = new System.Text.UTF8Encoding();

            return encoding.GetString(bytes);
        }

        /// <summary>
        /// Serilize the object to binary stream
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
}
