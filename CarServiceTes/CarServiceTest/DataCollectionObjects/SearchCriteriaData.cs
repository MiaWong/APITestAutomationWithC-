using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.CarTypes.V5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expedia.CarInterface.CarServiceTest.DataCollectionObjects
{
    public class SearchCriteriaData
    {
        public long sequence { get; set; }
        public StartCarLocationKey startCarLocationKey { get; set; }
        public EndCarLocationKey endCarLocationKey { get; set; }
        public StartCarLocation latLongCarLocation { get; set; }
        public List<long> supplierIDList { get; set; }
        public List<long> carClassificationIDList { get; set; }
        public String currencyCode { get; set; }
        public List<String> resultCarProductList { get; set; }

       
    }
}
