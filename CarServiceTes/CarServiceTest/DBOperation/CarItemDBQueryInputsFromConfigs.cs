using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Expedia.CarInterface.CarServiceTest.TestDataGenenator.TestConfigData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Expedia.CarInterface.CarServiceTest.Util;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class CarItemDBQueryInputsFromConfigs
    {
        public long carProductCatalogID { get; set; }
        public String pickupCountryCode { get; set; }
        public int oneWayBool { get; set; } //0,1
        public int onAirportBool { get; set; } //0,1
        public int standaloneBool { get; set; } //0,1
        public int flightOption { get; set; } //1,2 -- 1 means no flight
        public String hotelOption { get; set; } //"1" or "2,3" --- "1" means on hotel, "2,3" means hotel included(Merchant, Agency hotel all should be included)

        public CarItemDBQueryInputsFromConfigs(TestDataBase searchConfigs)
        {
            setCarProductCatalogID(searchConfigs);
            setPickupCountryOneWayBool(searchConfigs);
            setStandaloneBoolFlightHotelOption(searchConfigs);
            setOnAirportBool(searchConfigs);
        }

        public void setCarProductCatalogID(TestDataBase searchConfigs)
        {
            int tpid = int.Parse(searchConfigs.tpid);
            this.carProductCatalogID = CarsInventory.GetCarProductCatalogIDByTPID(tpid);
            //this.carProductCatalogID = CarProductCatalogID.GetByTPID_EAPID(tpid, 0);
        }

        public void setOnAirportBool(TestDataBase searchConfigs)
        {
            this.onAirportBool = searchConfigs.isOnAirPort == true ? 1 : 0;
        }

        public void setPickupCountryOneWayBool(TestDataBase searchConfigs)
        {
            // Support for IATA_GEO code [CDGT001] , handle the format. -- add by v-pxie 2013-5-9
            String pickupAirport = searchConfigs.pickupAirport.Substring(0, 3);
            String dropoffAirport = searchConfigs.dropoffAirport == null ? pickupAirport : searchConfigs.dropoffAirport.Substring(0, 3);
            pickupCountryCode = CarsInventory.GetCountryCode(pickupAirport);
            if (pickupAirport.Equals(dropoffAirport))
            {
                this.oneWayBool = 0;
            }
            else
            {
                this.oneWayBool = 1;
            }

            if (searchConfigs.BusinessModelID!= CarCommonEnumManager.BusinessModel.Merchant)
            {
                this.pickupCountryCode = "'" + pickupCountryCode + "'" + ", ''";
            }
            else
            {
                this.pickupCountryCode = "'" + pickupCountryCode + "'";
            }
        }

        public void setStandaloneBoolFlightHotelOption(TestDataBase searchConfigs)
        {
            uint purchaseTypeMask = searchConfigs.PurchaseTypeMask == null ? 128 : uint.Parse(searchConfigs.PurchaseTypeMask);

            switch (purchaseTypeMask)
            {
                case 4: //HC package
                    this.standaloneBool = 0;
                    this.hotelOption = "2,3";
                    this.flightOption = 1;
                    break;
                case 8: //FC package
                    this.standaloneBool = 0;
                    this.hotelOption = "1";
                    this.flightOption = 2;
                    break;
                case 16: //FHC package
                    this.standaloneBool = 0;
                    this.hotelOption = "2,3";
                    this.flightOption = 2;
                    break;
                case 128: //standalone car
                    this.standaloneBool = 1;
                    this.hotelOption = "1";
                    this.flightOption = 1;
                    break;
                case 256: //TC package
                    this.standaloneBool = 0;
                    this.hotelOption = "1";
                    this.flightOption = 1;
                    break;
                case 512: //FC bundle
                    this.standaloneBool = 1;
                    this.hotelOption = "1";
                    this.flightOption = 2;
                    break;
                case 1024:  // HC bundle
                    this.standaloneBool = 1;
                    this.hotelOption = "2,3";
                    this.flightOption = 1;
                    break;
                case 4096: // THC package
                    this.standaloneBool = 0;
                    this.hotelOption = "2,3";
                    this.flightOption = 1;
                    break;
                case 8192:  // FHC bundle
                    this.standaloneBool = 1;
                    this.hotelOption = "2,3";
                    this.flightOption = 2;
                    break;
                default:
                    Assert.Fail("Configged purchasetypemask is not a valid value for car");
                    break;
            }
        }
    }
}
