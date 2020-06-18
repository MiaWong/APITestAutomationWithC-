using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;
using Expedia.CarInterface.CarServiceTest.Util;

namespace Expedia.CarInterface.CarServiceTest.Communication
{
    public class CurrencyConversionRequest
    {
        private static String FxRatesRequestTemplate()
        {
            return "<urn:FXRatesRequest xmlns:urn=\"urn:expedia:xmlapi:fxrs:v1\">" +
             "<urn:MessageInfo>" +
                "<urn:CreateDateTime>2010-05-06T23:52:04.446-08:00</urn:CreateDateTime>" +
             "</urn:MessageInfo>" +
             "<urn:FXRateQueryList>" +
                "<urn:FXRateQuery>" +
                   "<urn:BaseCurrencyCode>USD</urn:BaseCurrencyCode>" +
                   "<urn:TargetCurrencyCode>CNY</urn:TargetCurrencyCode>" +
                "</urn:FXRateQuery>" +
             "</urn:FXRateQueryList>" +
            "</urn:FXRatesRequest>";
        }

        //fixed issue: FXRS is unstable, need to retry sometime. 2/27/2015
        public static Double GetCurrencyConversionRate(string baseCurCode, string targetCurCode)
        {
            Double exchangeRate = -1.00; // error value
            int iCount = 1;
            while (iCount <= 5)
            {
                Console.WriteLine(string.Format("{0}: Try to get rate from FXRS for baseCurCode:{1}, targetCurCode: {2}. ", iCount, baseCurCode, targetCurCode));               
                exchangeRate = GetCurrencyConversionRateSend(baseCurCode, targetCurCode);
                if (exchangeRate != -1.00)
                {
                    Console.WriteLine(string.Format("Get rate {0} from FXRS for baseCurCode:{1}, targetCurCode: {2}. ", exchangeRate, baseCurCode, targetCurCode));                
                    break;
                }
                iCount++;
            }

            if (exchangeRate == -1.00)
            {
                throw new Exception(string.Format("Can't get rate from FXRS for baseCurCode:{0}, targetCurCode: {1}. ", baseCurCode, targetCurCode));
            }
            return exchangeRate;
        }

        public static Double GetCurrencyConversionRateSend(String baseCurCode, String targetCurCode)
        {
            if (baseCurCode == null)
                throw new ArgumentNullException("baseCurCode");

            if (targetCurCode == null)
                throw new ArgumentNullException("targetCurCode");

            Double exchangeRate = -1.00; // error value

            XmlDocument docRequest = new XmlDocument();
            string xmlStr = "" + //"<?xml version='1.0' encoding='utf-8'?>\r\n" +
            "<s:Envelope " +
            "xmlns:s='http://www.w3.org/2003/05/soap-envelope' " +
            "xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' " +
            "xmlns:xsd='http://www.w3.org/2001/XMLSchema'>\r\n" +
            "</s:Envelope>";

            try
            {
                docRequest.LoadXml(xmlStr);

                //Get root element
                XmlElement envelope = docRequest.DocumentElement;

                // Create body
                XmlElement body = docRequest.CreateElement("s", "Body", "http://www.w3.org/2003/05/soap-envelope");
                body.InnerXml = FxRatesRequestTemplate();
                envelope.AppendChild(body);

                // Set DateTime
                String currentDate = DateTime.Now.ToString("o");
                XmlNode createDateTime = docRequest.GetElementsByTagName("urn:CreateDateTime")[0];
                createDateTime.InnerXml = currentDate;

                // Set BaseCurrencyCode
                XmlNode baseCurrencyCode = docRequest.GetElementsByTagName("urn:BaseCurrencyCode")[0];
                baseCurrencyCode.InnerXml = baseCurCode;

                // Set TargetCurrencyCode
                XmlNode targetCurrencyCode = docRequest.GetElementsByTagName("urn:TargetCurrencyCode")[0];
                targetCurrencyCode.InnerXml = targetCurCode;

                // Send/Recv
                String fxRsUri = CarConfigurationManager.AppSetting("FXRSUri");
                CommunicationInformation commInfo = new CommunicationInformation(fxRsUri, MsgProtocol.SOAP, MessageFormat.MessageContentType.Xml, MessageEncodeDecode.MessageContentEncoding.gzip, false);

                HttpWebResponse rsp = CommunicationManager.SendReqAndRecvResp(CommunicationUtil.ConvertXmlDocumentToMemoryStream(docRequest), commInfo);
                MemoryStream msRsp = CommunicationUtil.StreamToMemoryStream(rsp.GetResponseStream());
                msRsp.Seek(0, SeekOrigin.Begin);

                // Load Response in doc
                XmlDocument docResponse = new XmlDocument();
                docResponse.Load(msRsp);

                // Extract exchange rate
                XmlNode rate = null;
                if (docResponse.GetElementsByTagName("Rate", "urn:expedia:xmlapi:fxrs:v1").Count == 0)
                {
                    return exchangeRate;
                }

                rate = docResponse.GetElementsByTagName("Rate", "urn:expedia:xmlapi:fxrs:v1")[0];
                exchangeRate = Convert.ToDouble(rate.InnerXml);
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Error when sending FXRS for baseCurCode:{0}, targetCurCode: {1}. ", baseCurCode, targetCurCode));
                //throw e;
            }            

            return exchangeRate;
        }
    }
}
