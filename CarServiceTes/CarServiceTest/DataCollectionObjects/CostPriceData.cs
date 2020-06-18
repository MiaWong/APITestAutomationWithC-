using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expedia.CarInterface.CarServiceTest.DataCollectionObjects
{
    public class CostPriceData
    {
        public CostPriceLineItem cost { get; set; }
        public CostPriceLineItem price { get; set; }
        public CostPriceLineItem converted { get; set; }

       
    }
}
