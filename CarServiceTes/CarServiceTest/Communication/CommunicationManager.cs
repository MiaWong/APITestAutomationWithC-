using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using Expedia.CarInterface.CarServiceTest.Util;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Expedia.CarInterface.CarServiceTest.RequestGenerator.ComponentGenerator;

namespace Expedia.CarInterface.CarServiceTest.Communication
{
    public class CommunicationManager
    {
        public enum MessageMode
        {
            Serialize,
            Deserialize
        }
        static bool NeedPrintMessageToConsole = Convert.ToBoolean(CarConfigurationManager.AppSetting("NeedPrintMessageToConsole"));

        public static string getContextType(MessageFormat.MessageContentType messageContentType = MessageFormat.MessageContentType.Xml,
            MsgProtocol msgProtocol = MsgProtocol.Interop)
        {
            string contextType = "";
            if (messageContentType == MessageFormat.MessageContentType.FastInfoSet && msgProtocol == MsgProtocol.Interop)
            {
                contextType = "application/fastinfoset";
            }
            if (messageContentType == MessageFormat.MessageContentType.Xml && msgProtocol == MsgProtocol.Interop)
            {
                contextType = "application/xml";
            }
            if ( messageContentType == MessageFormat.MessageContentType.FastInfoSet && msgProtocol == MsgProtocol.SOAP)
            {
                contextType = "application/soap+fastinfoset";
            }
            if (messageContentType == MessageFormat.MessageContentType.Xml && msgProtocol == MsgProtocol.SOAP)
            {
                contextType = "application/soap+xml";
            }
            
            return contextType;
        }

        /*
        public static string FormatRequest(MemoryStream ms, MessageFormat.MessageContentType messageContentType = MessageFormat.MessageContentType.Xml, MessageMode messageMode = MessageMode.Serialize)
        {
            string strRequest = "";
            MemoryStream outMS;
            if (messageMode == MessageMode.Serialize)
            {
                outMS = MessageFormat.Serialize(ms, messageContentType);
            }
            else
            {
                outMS = MessageFormat.Deserialize(ms, messageContentType);
            }

            return strRequest;
        }

        public static WebRequest getWebRequest(string strRequest, string uri, MessageFormat.MessageContentType messageContentType = MessageFormat.MessageContentType.Xml, 
            MsgProtocol msgProtocol = MsgProtocol.Interop, bool withE3JRequestHeader = true)
        {
            WebRequest webRequest = WebRequest.Create(uri);
            byte[] byteArray = Encoding.UTF8.GetBytes(strRequest);
            webRequest.Method = "POST";
            webRequest.ContentType = getContextType(messageContentType, msgProtocol);
            webRequest.ContentLength = byteArray.Length;

            
            if (withE3JRequestHeader)
            {
                // E3JMS-L-origServerName
                webRequest.Headers.Add("E3JMS-L-origServerName", Environment.MachineName);

                // E3JMS-L-serviceName
                webRequest.Headers.Add("E3JMS-L-serviceName", System.Diagnostics.Process.GetCurrentProcess().ProcessName);

                // e3jms-l-stand-rep
                webRequest.Headers.Add("e3jms-l-stand-rep", "TQ:e3Interop");

                // E3JMS-L-activityId
                Guid guid = Guid.NewGuid();
                webRequest.Headers.Add("e3jms-l-activityId-propname", "activityId");
                webRequest.Headers.Add("E3JMS-L-activityId", Convert.ToString(guid));

                // E3JMS-L-stand-mes
                String messageId = Convert.ToString(guid) + "-" + Guid.NewGuid();
                webRequest.Headers.Add("E3JMS-L-stand-mes", messageId);
            }

            // Get the request stream.
            Stream dataStream = webRequest.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            return webRequest;
        }*/


        public static ResponseT webSend<RequestT, ResponseT>(RequestT requestObj, CommunicationInformation comInfo, string originalGUID = null)
        {
            if (requestObj == null)
                throw new ArgumentNullException("reqObj is null.");

            if (comInfo == null)
                throw new ArgumentNullException("commInfo is null.");
            WebRequest request = WebRequest.Create(comInfo.URI);

            string strRequest = TransferTypeUtil.ObjToStr(requestObj);
            ResponseT responseObj;
            if (NeedPrintMessageToConsole)
            {
                Console.WriteLine("========== Print Request ==========");
                Console.WriteLine(strRequest);
            }
            byte[] byteArray = Encoding.UTF8.GetBytes(strRequest);
            request.Method = "POST";
            request.ContentType = getContextType(comInfo.ContentType, comInfo.MessageProtocol);
            request.ContentLength = byteArray.Length;
            //If originalGUID is not null, use it
            //Otherwise if need spoofer, get OriginalGUID by TestCaseID; otherwise, generate a new GUID
            if (Convert.ToBoolean(CarConfigurationManager.AppSetting("NeedSpoofer")) == true)
            {
                uint nTestCaseID = CarCommonRequestGenerator.getTUIDFromRequest(requestObj);
                /// add by v-pxie 2013-11-22: support for the GUID list with one TUID
                string requestTypeName = requestObj.GetType().Name;
                originalGUID = EmbededFileOperation.getGUIDFromEmbededFile(nTestCaseID);                
            }
            else if (originalGUID == null || originalGUID.Length == 0)
            {
                originalGUID = Guid.NewGuid().ToString();
            }

            //add OriginalGUID in request header
            request.Headers.Add("e3jms-l-activityId-propname", "activityId");
            request.Headers.Add("E3JMS-L-activityId", originalGUID);

            // Get the request stream.
            Stream dataStream = request.GetRequestStream();

            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine("Response.StatusDescription=" + ((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            try
            {
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                if (NeedPrintMessageToConsole)
                {
                    Console.WriteLine("========== Print Response ==========");
                    if (null != responseFromServer)
                        Console.WriteLine(responseFromServer);
                }
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();

                responseObj = (ResponseT)TransferTypeUtil.strToObj(responseFromServer, typeof(ResponseT));
                return responseObj;
            }
            catch (Exception ex)
            {
                throw new Exception("Transport failed " + ex.ToString());
            }
        }

        public static ResponseT send<RequestT, ResponseT>(RequestT reqObj, CommunicationInformation commInfo, uint tuid = 0, bool replaceFlag = false)
        {
            if (reqObj == null)
                throw new ArgumentNullException("reqObj is null.");

            if (commInfo == null)
                throw new ArgumentNullException("commInfo is null.");

            MemoryStream msReq = null;
            MemoryStream msRsp = null;

            // for Sprunk to show message with          
            if (NeedPrintMessageToConsole)
            {
                Console.WriteLine("Run Test case on Server :" + commInfo.URI);
                Console.Error.WriteLine("Now Run Test case on Server :" + commInfo.URI);
                Console.WriteLine("-----------Request:-----------");
                Print.PrintMessageToConsole(reqObj);
            }

            // Serialize Request [obj] to [objXML]
            msReq = XMLSerializer.Serialize(reqObj, typeof(RequestT));

            // Attach Protocol [objXML] to [Hdr + objXML]
            msReq = MessageProtocol.AttachProtocol(msReq, commInfo);

            // Serialize Request [Hdr + objXML] to [FI[Hdr + objXML]]
            msReq = MessageFormat.Serialize(msReq, commInfo.ContentType);

            // Encode Request [FI[Hdr + objXML]] to [GZip[FI[Hdr + objXML]]]
            msReq = MessageEncodeDecode.Encode(msReq, commInfo.ContentEncoding.ToString());

            // Send/Recv Request/Response
            if (NeedPrintMessageToConsole)
            {
                Console.WriteLine(string.Format("-----------Begin to send request to service, transport encoding is {0}: -----------", commInfo.ContentType.ToString()));
            }
            // It's just for spoofer
            if (Convert.ToBoolean(CarConfigurationManager.AppSetting("NeedSpoofer")) == true)
            {
                if (tuid > 0)
                {
                    commInfo.TestCaseID = tuid;
                    if (NeedPrintMessageToConsole)
                    {
                        Console.WriteLine("TUID from parameter:" + tuid + "!");
                    }
                }
                else
                {
                    commInfo.TestCaseID = CarCommonRequestGenerator.getTUIDFromRequest(reqObj);
                    {
                        if (commInfo.TestCaseID == 0) Console.WriteLine("TUID can't be get from request object!");
                        else Console.WriteLine("TUID from request object:" + commInfo.TestCaseID + "!");
                    }
                }
            }
            else
            {
                if (NeedPrintMessageToConsole)
                    Console.WriteLine("Spoofer is off, no testcaseID get!");
            }
            ResponseT rspobj = default(ResponseT);
            string statusCode = "";
            HttpWebResponse httpWebResponse = SendReqAndRecvResp(msReq, commInfo,typeof(RequestT).Name);
            
            if (null != httpWebResponse)
            {
                statusCode = httpWebResponse.StatusCode.ToString();
            }

            MemoryStream msRspReturn = CommunicationUtil.StreamToMemoryStream(httpWebResponse.GetResponseStream());

            if (msRspReturn == null || msRspReturn.Length == 0)
            {
                throw new Exception("Response is empty or Null, please check.");
            }
            else
            {
                // Decode Response [GZip[FI[Hdr + objXML]]] to [FI[Hdr + objXML]]
                msRspReturn = MessageEncodeDecode.Decode(msRspReturn, httpWebResponse.ContentEncoding);

                // DeSerialize Response [FI[Hdr + objXML]] to [Hdr + objXML]
                msRspReturn = MessageFormat.Deserialize(msRspReturn, commInfo.ContentType);

                // Detach Protocol [Hdr + objXML] to [objXML]
                msRspReturn = MessageProtocol.DetachProtocol(msRspReturn, commInfo.MessageProtocol);

                msRsp = msRspReturn;
                // handle aws namespace issue
                if (replaceFlag)
                {
                    string siteName = ServiceConfigUtil.CarProductTokenUriGet();
                    if (siteName.Contains("cars-business-service"))
                    {
                        string responseStr = System.Text.Encoding.GetEncoding(httpWebResponse.CharacterSet).GetString(msRspReturn.ToArray());
                        responseStr = responseStr.Replace("urn:com", "urn");
                        XmlDocument rspXml = new XmlDocument();
                        rspXml.LoadXml(responseStr);
                        msRsp = CommunicationUtil.ConvertXmlDocumentToMemoryStream(rspXml);

                    }
                }
                
                // Deserialize Response [objXML] to [obj]
                rspobj = (ResponseT)XMLSerializer.DeSerialize(msRsp, typeof(ResponseT));
                if (NeedPrintMessageToConsole)
                {
                    Console.WriteLine("-----------Response:-----------");
                    Print.PrintMessageToConsole(rspobj);
                }
            }
            return rspobj;
        }

        public static void send<RequestT>(RequestT reqObj, CommunicationInformation commInfo)
        {
            if (reqObj == null)
                throw new ArgumentNullException("reqObj is null.");

            if (commInfo == null)
                throw new ArgumentNullException("commInfo is null.");

            MemoryStream msReq = null;

            // Serialize Request [obj] to [objXML]
            msReq = XMLSerializer.Serialize(reqObj, typeof(RequestT));

            // Send/Recv Request/Response
            Console.WriteLine(string.Format("-----------Begin to send request to service, transport encoding is {0}: -----------", commInfo.ContentType.ToString()));
            HttpWebResponse httpWebResponse = SendReqAndRecvResp(msReq, commInfo,typeof(RequestT).Name);
            if (httpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Send request failed, actual Http request status code: " + httpWebResponse.StatusCode.ToString());
            }
            // Get the stream containing content returned by the server.
            try
            {
                Stream dataStream = httpWebResponse.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                Console.WriteLine("========== Print Response ==========");
                if (null != responseFromServer)
                {
                    Console.WriteLine(responseFromServer);
                }
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                httpWebResponse.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Transport failed " + ex.ToString());
            }
            //Thread.Sleep(5000);
        }

        /// add by v-pxie 2013-11-22: support for the GUID list with one TUID and requestNmae
        public static HttpWebResponse SendReqAndRecvResp(MemoryStream ms, CommunicationInformation commInfo, string requestTypeName=null) 
        {
            if (ms == null)
                throw new ArgumentNullException("ms");
            
            if (commInfo == null)
                throw new ArgumentNullException("commInfo");

            HttpWebRequest httpWebRequest = CreateRequestHeader(commInfo);

            // If need spoofer, get OriginalGUID by TestCaseID; otherwise, generate a new GUID
            string originalGUID = "";
            if (Convert.ToBoolean(CarConfigurationManager.AppSetting("NeedSpoofer")) == true)
            {
                /// add by v-pxie 2013-11-22: support for the GUID list with one TUID and requestNmae
                originalGUID = EmbededFileOperation.getGUIDFromEmbededFile(commInfo.TestCaseID);
            }
            else
            {
                originalGUID = Guid.NewGuid().ToString();
            }

            // E3JMS-L-activityId
            // Note: If need Spoofer, whatever the value in WithE3JRequestHeader, we all need to add OriginalGUID in request header.
            if (Convert.ToBoolean(CarConfigurationManager.AppSetting("NeedSpoofer")) == true || commInfo.WithE3JRequestHeader)
            {
                httpWebRequest.Headers.Add("e3jms-l-activityId-propname", "activityId");
                httpWebRequest.Headers.Add("E3JMS-L-activityId", originalGUID);
            }

            // Send E3J request with httpWebRequest.Headers.
            if (commInfo.WithE3JRequestHeader)
            {
                // E3JMS-L-origServerName
                httpWebRequest.Headers.Add("E3JMS-L-origServerName", Environment.MachineName);

                // E3JMS-L-serviceName
                httpWebRequest.Headers.Add("E3JMS-L-serviceName", System.Diagnostics.Process.GetCurrentProcess().ProcessName);

                // e3jms-l-stand-rep
                httpWebRequest.Headers.Add("e3jms-l-stand-rep", "TQ:e3Interop");

                // E3JMS-L-stand-mes
                String messageId = originalGUID + "-" + Guid.NewGuid();
                httpWebRequest.Headers.Add("E3JMS-L-stand-mes", messageId);
            }

            if(commInfo.HttpSendMode == CommunicationInformation.HTTPSendMode.POST)
            httpWebRequest.Accept = httpWebRequest.ContentType;

            httpWebRequest.ContentLength = ms.GetBuffer().Length;
            if(httpWebRequest.ContentLength > 0) httpWebRequest.GetRequestStream().Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            return SendRequestAndReceiveResponse(httpWebRequest);
        }

        public static HttpWebResponse SendRequestAndReceiveResponse(HttpWebRequest request)
        {
            HttpWebResponse response = null;
            if (request == null)
                throw new ArgumentNullException("request");
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                throw e;
            }
            return response;
        }

        public static HttpWebRequest CreateRequestHeader(CommunicationInformation commInfo)
        {
            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(commInfo.URI);
            request.Pipelined = false;
            request.Method = commInfo.HttpSendMode.ToString();
            request.KeepAlive = true;
            request.Timeout = Convert.ToInt32(CarConfigurationManager.AppSetting("MessageRequestTimeout")); 

            if (commInfo.ContentEncoding != MessageEncodeDecode.MessageContentEncoding.None)
            {
                request.Headers.Add("Accept-Encoding", commInfo.ContentEncoding.ToString());
            }

            string contentType = "application/";
            if (commInfo.MessageProtocol == MsgProtocol.SOAP)
            {
                contentType += "soap+";
            }

            switch (commInfo.ContentType)
            {
                case MessageFormat.MessageContentType.Xml:
                    contentType += "xml";
                    break;
                case MessageFormat.MessageContentType.FastInfoSet:
                    contentType += "fastinfoset";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("msgContentType");
            }

            request.ContentType = contentType;

            return request;
        }

        //Send Http request to tools, E.g: CPCAT, Car internal admin tool
        public static string SendToolHttpRequestAndReceiveResponse(String url)
        {
            try
            {
                //Print the Http request to tool,E.g: CPCAT

                //Create a new HttpWebRequest
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Pipelined = false;
                request.Method = "POST";

                //Set headers
                request.Headers.Add("x-prototype-version", "1.6.0.3");
                request.Headers.Add("x-requested-with", "XMLHttpRequest");
                request.Headers.Add("UA-CPU", "x86");
                request.Headers.Add("Pragma", "no-cache");

                request.ContentType = "application/xml";
                request.Timeout = 150000;

                // Set credentials to use for this request.
                request.Credentials = CredentialCache.DefaultCredentials;

                //Send request to tool(E.g CPCAT) and get response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Convert response to string for investigation if needed.
                byte[] responseBytes = TransferTypeUtil.ReadStream(response.GetResponseStream());
                string responseString = TransferTypeUtil.ByteArrayToString(responseBytes, CarCommonEnumManager.EncodingType.ASCII);
                return responseString;
            }
            catch (Exception e)
            {
                Assert.Fail("Http request sending fail, exception is: " + e.ToString());
                return "";
            }
        }

        public static string send(string strRequest, CommunicationInformation commInfo)
        {
            if (commInfo.HttpSendMode == CommunicationInformation.HTTPSendMode.POST && string.IsNullOrEmpty(strRequest))
            {
                Assert.Fail("Can't send request, request object is null.");
            }

            if (commInfo == null)
            {
                Assert.Fail("Can't send request, CommunicationInfo object is null.");
            }

            WebRequest request = CreateWebRequestHeader(commInfo);
            request.Timeout = System.Threading.Timeout.Infinite;

            Stream dataStream;

            request.Timeout = System.Threading.Timeout.Infinite;

            if (commInfo.HttpSendMode == CommunicationInformation.HTTPSendMode.POST)
            {
                // Print request
                Console.WriteLine("Request content:\r\n" + strRequest);
                byte[] byteArray = Encoding.UTF8.GetBytes(strRequest);
                request.ContentLength = byteArray.Length;
                
                request.GetRequestStream().Write(byteArray, 0, byteArray.Length);
            }

            try
            {
                // Get the response.
                WebResponse response = request.GetResponse();

                // Display the status.
                Console.WriteLine("Response.StatusDescription=" + ((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.

                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                Console.WriteLine("Response content:\r\n" + TransferTypeUtil.ObjToStr(responseFromServer));
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();

                return responseFromServer;
            }
            catch (WebException ex)
            {
                HttpWebResponse x = ex.Response as HttpWebResponse;
                dataStream = x.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader1 = new StreamReader(dataStream);
                // Read the content.
                return x.StatusCode + ":" + reader1.ReadToEnd();
            }
        }

        public static WebRequest CreateWebRequestHeader(CommunicationInformation commInfo)
        {
            HttpWebRequest request = null;
            request = (HttpWebRequest) WebRequest.Create(commInfo.URI);
            if (commInfo.HttpSendMode == CommunicationInformation.HTTPSendMode.GET)
            {
                if (commInfo.ContentType == MessageFormat.MessageContentType.Json) request.Accept = "application/json";
                //else request.Accept = "application/xml";
            }
            request.Method = commInfo.HttpSendMode.ToString();
            request.Timeout = Convert.ToInt32(CarConfigurationManager.AppSetting("HttpRequestTimeOut"));
            request.ContentType = getContentType(commInfo.ContentType, commInfo.MessageProtocol);


            if (commInfo.ContentEncoding != MessageEncodeDecode.MessageContentEncoding.None)
            {
                request.Headers.Add("Accept-Encoding", commInfo.ContentEncoding.ToString());
            }

            //add OriginalGUID in request header
            printRequestHeader(commInfo);
            return request;
        }

        public static string getContentType(MessageFormat.MessageContentType messageContentType = MessageFormat.MessageContentType.Xml,
            MsgProtocol msgProtocol = MsgProtocol.Interop)
        {
            string contentType = "application/";
            if (msgProtocol == MsgProtocol.SOAP)
            {
                contentType += "soap+";
            }

            switch (messageContentType)
            {
                case MessageFormat.MessageContentType.Xml:
                    contentType += "xml";
                    break;
                case MessageFormat.MessageContentType.FastInfoSet:
                    contentType += "fastinfoset";
                    break;
                case MessageFormat.MessageContentType.Json:
                    contentType += "json";
                    break;
                ////text/csv+tab
                case MessageFormat.MessageContentType.CSVTab:
                    contentType = "text/csv+tab";
                    break;
                default:
                    throw new Exception("Can't parse unsupported ContentType for request, not able to send request.");
            }
            return contentType;
        }
        public static void printRequestHeader(CommunicationInformation commInfo)
        {
            StringBuilder strCommInfo = new StringBuilder(string.Empty);
            strCommInfo.AppendLine();
            strCommInfo.AppendLine("ContentType: " + getContentType(commInfo.ContentType, commInfo.MessageProtocol));
        }
    }
}
