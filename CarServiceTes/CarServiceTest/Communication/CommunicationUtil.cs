using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Expedia.CarInterface.CarServiceTest.Communication
{
    public class CommunicationUtil
    {
        public static MemoryStream StreamToMemoryStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream is null");

            MemoryStream memoryStream = new MemoryStream();
            byte[] byteArray = StreamToByteArray(stream);
            if (byteArray == null)
                throw new ApplicationException("byteArray is null");

            var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(byteArray);

            return memoryStream;
        }

        public static byte[] StreamToByteArray(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static MemoryStream ConvertXmlDocumentToMemoryStream(XmlDocument doc)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");
            //Console.WriteLine("doc.InnerText=" + doc.InnerXml);
            byte[] byteArray = Encoding.Default.GetBytes(doc.InnerXml);            
            MemoryStream ms = new MemoryStream();
            ms.Write(byteArray, 0, byteArray.Length);

            return ms;
        }

        public static MemoryStream ConvertXmlNodeToMemoryStream(XmlNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            byte[] byteArray = Encoding.ASCII.GetBytes(node.InnerXml);
            MemoryStream ms = new MemoryStream();
            ms.Write(byteArray, 0, byteArray.Length);

            return ms;
        }
    }
}
