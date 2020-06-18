using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;

namespace Expedia.CarInterface.CarServiceTest.Communication
{
    public class SOAPProtocol
    {
        public static MemoryStream GetSOAPBody(MemoryStream bodyContent)
        {
            if (bodyContent == null)
                throw new ArgumentNullException("bodyContent");

            XmlNode body = GetSOAPBodyNode(bodyContent);
            MemoryStream ms = CommunicationUtil.ConvertXmlNodeToMemoryStream(body);

            return ms;
        }

        public static XmlNode GetSOAPBodyNode(MemoryStream bodyContent)
        {
            if (bodyContent == null)
                throw new ArgumentNullException("bodyContent");

            XmlDocument doc = new XmlDocument();

            if (bodyContent.CanSeek)
                bodyContent.Position = 0;

            doc.Load(bodyContent);

            XmlNode root = doc.FirstChild;
            if (root != null &&
                root.NodeType == XmlNodeType.XmlDeclaration)
            {
                root = root.NextSibling;
            }

            if (root == null)
                throw new ApplicationException("Root element node is missing in XmlDocument");

            XmlNode rspbody = root["s:Body"];
            if (rspbody == null)
                throw new ApplicationException("Body element node is missing in XmlDocument");

            return rspbody;
        }

        public static MemoryStream CreateSoapRequest(MemoryStream bodyContent, CommunicationInformation commInfo)
        {
            if (bodyContent == null)
                throw new ArgumentNullException("bodyContent");
            if (commInfo == null)
                throw new ArgumentNullException("commInfo");

            XmlDocument doc = CreateSOAPRequest(bodyContent, commInfo.HostPathSoapHdr, commInfo.HostPathSoapHdrTo);

            MemoryStream ms = CommunicationUtil.ConvertXmlDocumentToMemoryStream(doc);

            return ms;
        }

        public static XmlDocument CreateSOAPRequest(MemoryStream bodyContent, string headerAction, string headerTo)
        {
            if (bodyContent == null)
                throw new ArgumentNullException("bodyContent");
            if (headerAction == null)
                throw new ArgumentNullException("action");

            XmlDocument doc = new XmlDocument();
            string xmlStr = "" + //"<?xml version='1.0' encoding='utf-8'?>\r\n" +
            "<s:Envelope " +
            "xmlns:s='http://www.w3.org/2003/05/soap-envelope' " +
            "xmlns:a='http://www.w3.org/2005/08/addressing' " +
            "xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' " +
            "xmlns:xsd='http://www.w3.org/2001/XMLSchema'>\r\n" +
            "</s:Envelope>";
            doc.LoadXml(xmlStr);

            //Get root element
            XmlElement envelope = doc.DocumentElement;

            //Create header
            XmlElement header = doc.CreateElement("s", "Header", "http://www.w3.org/2003/05/soap-envelope");

            // Create header elements
            XmlElement action = doc.CreateElement("a", "Action", "http://www.w3.org/2005/08/addressing");
            action.SetAttribute("s:mustUnderstand", "1");
            action.InnerText = headerAction;

            XmlElement messageID = doc.CreateElement("a", "MessageID", "http://www.w3.org/2005/08/addressing");
            //messageID.InnerText = "urn:uuid:c395079a-065e-40aa-8a70-0b4899c71a3c"; // Todo: Auto-generate
            messageID.InnerText = "urn:uuid:" + Guid.NewGuid().ToString();
            
            XmlElement address = doc.CreateElement("a", "Address", "http://www.w3.org/2005/08/addressing");
            address.InnerText = "http://www.w3.org/2005/08/addressing/anonymous";

            XmlElement replyTo = doc.CreateElement("a", "ReplyTo", "http://www.w3.org/2005/08/addressing");
            replyTo.AppendChild(address);

            XmlElement to = doc.CreateElement("a", "To", "http://www.w3.org/2005/08/addressing");
            to.SetAttribute("s:mustUnderstand", "1");
            to.InnerText = headerTo;

            header.AppendChild(action);
            header.AppendChild(messageID);
            header.AppendChild(replyTo);
            header.AppendChild(to);

            // Append header to envelope
            envelope.AppendChild(header);

            //Create body
            XmlElement body = doc.CreateElement("s", "Body", "http://www.w3.org/2003/05/soap-envelope");
            body.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            body.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");

            bodyContent.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(bodyContent); 
            body.InnerXml = reader.ReadToEnd();

            envelope.AppendChild(body);

            return doc;
        }
    }
}
