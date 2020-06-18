using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Expedia.CarInterface.CarServiceTest.Util;

namespace Expedia.CarInterface.CarServiceTest.Communication
{
    public class CommunicationInformation
    {
        public String URI;
        public MsgProtocol MessageProtocol; // soap | Interop
        public MessageFormat.MessageContentType ContentType; // xml | fastinfoset
        public MessageEncodeDecode.MessageContentEncoding ContentEncoding; // gzip
        public String HostPathSoapHdr;      
        public String HostPathSoapHdrTo;    
        public bool WithE3JRequestHeader = true;
        public uint TestCaseID;
        public HTTPSendMode HttpSendMode;

        public enum HTTPSendMode
        {
            GET,
            POST
        }

        public CommunicationInformation(String uri, MsgProtocol protocol = MsgProtocol.Interop, MessageFormat.MessageContentType contentType = MessageFormat.MessageContentType.FastInfoSet,
            MessageEncodeDecode.MessageContentEncoding contentEncoding = MessageEncodeDecode.MessageContentEncoding.None, bool withE3JRequestHeader = true, HTTPSendMode httpSendMode = HTTPSendMode.POST)
        {
            this.URI = uri;
            this.MessageProtocol = protocol;
            this.ContentType = contentType;
            this.ContentEncoding = contentEncoding;
            this.HttpSendMode = httpSendMode;
            WithE3JRequestHeader = withE3JRequestHeader;
        }
    }
}
