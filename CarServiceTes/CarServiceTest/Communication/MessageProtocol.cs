using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Expedia.CarInterface.CarServiceTest.Communication
{
    public enum MsgProtocol
    {
        SOAP,
        Interop
    };

    public class MessageProtocol
    {
        public static MemoryStream AttachProtocol(MemoryStream ms, CommunicationInformation commInfo)
        {
            if (commInfo.MessageProtocol == MsgProtocol.SOAP)
            {
                return SOAPProtocol.CreateSoapRequest(ms, commInfo);
            }
            else if (commInfo.MessageProtocol == MsgProtocol.Interop)
            {
                return ms; // There is no Header 
            }

            throw new ArgumentOutOfRangeException("commInfo.MessageProtocol");
        }

        public static MemoryStream DetachProtocol(MemoryStream ms, MsgProtocol mp)
        {
            if (mp == MsgProtocol.SOAP)
            {
                return SOAPProtocol.GetSOAPBody(ms);
            }
            else if (mp == MsgProtocol.Interop)
            {
                return ms; // There is no Header 
            }

            throw new ArgumentOutOfRangeException("commInfo.MessageProtocol");
        }
    }
}
