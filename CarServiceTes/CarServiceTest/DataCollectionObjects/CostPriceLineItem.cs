using Expedia.CarInterface.CarServiceTest.Util;
using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.FinanceTypes.V4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expedia.CarInterface.CarServiceTest.DataCollectionObjects
{
    public class CostPriceLineItem
    {
        public String description { get; set; }
        public String currencyCode { get; set; }
        public Double amount { get; set; }
        public Double multiplier { get; set; }
        public String financeCategoryCode { get; set; }
        public String financeApplicationCode { get; set; }
        public String financeApplicationUnitCount { get; set; }
        public String referenceCategoryCode { get; set; }
        public String referenceCode { get; set; }


        public CostPriceLineItem()
        {
        }

        public CostPriceLineItem(Double amount, String currencyCode, String description, String financeApplicationCode,
            String financeApplicationUnitCount, string financeCategoryCode)
        {
            if (currencyCode == null) this.multiplier = amount;
            else this.amount = amount;
            this.currencyCode = currencyCode;
            this.description = description;
            this.financeApplicationCode = financeApplicationCode;
            this.financeApplicationUnitCount = financeApplicationUnitCount;
            this.financeCategoryCode = financeCategoryCode;
        }
    }
}
