using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.CarTypes.V5;
using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.PlaceTypes.V4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expedia.CarInterface.CarServiceTest.DataCollectionObjects
{
    public class CarProductData
    {

        public String carProductToken { get; set; }
        public String originatingGUID { get; set; }
        public String messageGUID { get; set; }
        public DateTime dataTimeStamp { get; set; }
        public String dataVersion { get; set; }

        public CarInventoryKey carInventoryKey { get; set; }
        public String availStatusCode { get; set; }
        public String pointOfSupplyCurrencyCode { get; set; }
        public long carDoorCount { get; set; }
        public CarPickupLocation carPickupLocation { get; set; }
        public CarDropOffLocation carDropOffLocation { get; set; }
        public CarMileage carMileage { get; set; }
        public CarVehicleOptionList carVehicleOptionList { get; set; }
        public CarRateDetail carRateDetail { get; set; }
        public CarPolicyList carPolicyList { get; set; }
        public UpgradeCarProductTokenList upgradeCarProductTokenList { get; set; }
        public CarCatalogMakeModel carCatalogMakeModel { get; set; }
        public String reservationGuaranteeCategory { get; set; }
        public Boolean prePayBoolean { get; set; }

        public String originalJasonString { get; set; }//For test investigation

        public String origin { get; set; } //"REQUEST" or "RESPONSE" 

        //US 845124 combine Financial data to CarProduct data
        public CostPriceData grandTotal { get; set; }
        public CostPriceData totalTaxesAndFees { get; set; }
        public CostPriceData totalBaseRate { get; set; }
        public CostPriceData dropCharge { get; set; }
        public List<CostPriceData> costPriceDataList { get; set; }
        public CostPriceData averageDailyRate { get; set; }

        public CostPriceLineItem referenceTotal { get; set; }
        public long referenceCarItemID { get; set; }
        public CarMarkupInfo carMarkupInfo { get; set; }

        //Added by US 1025151
        public String action { get; set; }
        public PointOfSaleKey pointOfSaleKey { get; set; }
        public int leadTimeDays { get; set; }

      
    }


}
