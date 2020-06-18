using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.PlaceTypes.V4;
using Expedia.CarInterface.CarServiceTest.XSDObjects.S3.CarBS.Getdetails.V4;
using Expedia.CarInterface.CarServiceTest.XSDObjects.S3.CarBS.Search.V4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expedia.CarInterface.CarServiceTest.DataCollectionObjects
{
    public class MessageData
    {
        public String originatingGUID { get; set; }
        public String messageGUID { get; set; }
        public String messageName { get; set; }
        public String messageVersion { get; set; }
        public DateTime dataTimeStamp { get; set; }
        public PointOfSaleKey pointOfSaleKey { get; set; }
        public String clientCode { get; set; }
        public List<MessageDataItem> messageDataItems { get; set; }
        public long purchaseTypeMask { get; set; }
        public Object errorCollection { get; set; } 

        //For test investigation
        public String originalJasonString { get; set; }

        //For User Story 504836: Search Package Car based on CarECommerceSearchRequest.PackageBoolean and ProductCategoryCodeList - edit by Qiuhua
        public Boolean packageBoolean { get; set; }
        public List<String> productCategoryCodeList { get; set; }
        
        public MessageData()
        {
        }
      
    }  

}
