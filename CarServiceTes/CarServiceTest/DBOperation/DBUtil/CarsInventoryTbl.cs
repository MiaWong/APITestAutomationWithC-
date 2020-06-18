using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.CarTypes.V5;
using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.PlaceTypes.V4;

namespace Expedia.CarInterface.CarServiceTest.DBOperation.DBUtil
{
    #region TPIDToPoSAttributeMap
    public class TPIDToPoSAttributeMap
    {
        public int TravelProductID;
        public uint PartnerID;
        public string JurisdictionCode;
        public string CompanyCode;
        public string ManagementUnitCode;
        public TPIDToPoSAttributeMap(int myTPID, uint myEAPID, string myJurisdictionCode, string myCompanyCode, string myManagementUnitCode)
        {
            TravelProductID = myTPID;
            PartnerID = myEAPID;
            JurisdictionCode = myJurisdictionCode;
            CompanyCode = myCompanyCode;
            ManagementUnitCode = myManagementUnitCode;
        }
    }
    #endregion

    #region result for CarTaxRateSrch
    public class CarTaxRateSrchResult
    {
        public string AirportCode;
        public string TaxCurrencyCode;
        public string CarVendorLocationCode;
        public string CarVendorCode;
        public string CarTaxFeeTypeName;
        public double CarTaxRatePct;
        public double CarTaxAmt;
        public string CarTaxDurationCode;
        public CarTaxRateSrchResult(string myAirportCode, string myTaxCurrencyCode, string myCarVendorLocationCode, string myCarVendorCode,
            string myCarTaxFeeTypeName, string myCarTaxRatePct, string myCarTaxAmt, string myCarTaxDurationCode)
        {
            AirportCode = myAirportCode;
            TaxCurrencyCode = myTaxCurrencyCode;
            CarVendorLocationCode = myCarVendorLocationCode;
            CarVendorCode = myCarVendorCode;
            CarTaxFeeTypeName = myCarTaxFeeTypeName;
            CarTaxRatePct = Convert.ToDouble(myCarTaxRatePct);
            CarTaxAmt = Convert.ToDouble(myCarTaxAmt);
            CarTaxDurationCode = myCarTaxDurationCode;
        }
    }
    #endregion

    #region inputs for GetCarCostAndAvailGet
    public class GetCarCostAndAvailGetInputs
    {
        public uint pCarAgreementID;
        public string pAirportCode;
        public DateTime pPickUpDate;
        public DateTime pDropOffDate;
        public uint pCarCategoryID;
        public uint pCarTypeID;
        public uint pCarTransmissionDriveID;
        public uint pCarFuelAirConditionID;
        public string pCarVendorCode;
        public string pCarVendorLocationCode;
        public bool pOnAirportSearchBool;
        public GetCarCostAndAvailGetInputs(CarProduct car, bool myOnAirportSearchBool)
        {
            CarInventoryKey key = car.CarInventoryKey;
            pCarAgreementID = key.CarRate.CarAgreementID;
            pAirportCode = key.CarCatalogKey.CarPickupLocationKey.LocationCode;
            pPickUpDate = key.CarPickUpDateTime;
            pDropOffDate = key.CarDropOffDateTime;
            pCarCategoryID = key.CarCatalogKey.CarVehicle.CarCategoryCode;
            pCarTypeID = key.CarCatalogKey.CarVehicle.CarTypeCode;
            pCarTransmissionDriveID = key.CarCatalogKey.CarVehicle.CarTransmissionDriveCode;
            pCarFuelAirConditionID = key.CarCatalogKey.CarVehicle.CarFuelACCode;
            pCarVendorCode = CarsInventory.GetVendorCodeFromCarVendor(key.CarCatalogKey.VendorSupplierID);
            pCarVendorLocationCode = key.CarCatalogKey.CarPickupLocationKey.CarLocationCategoryCode + key.CarCatalogKey.CarPickupLocationKey.SupplierRawText;
            pOnAirportSearchBool = myOnAirportSearchBool;
        }
    }
    #endregion

    #region result for GetCarCostAndAvailGet
    public enum GetCarCostAndAvailGetResult_BaseRateColumns
    {
        BasicRateDailyAmt = 1,
        BasicRate2DayAmt = 2,
        BasicRate3DayAmt = 3,
        BasicRate4DayAmt = 4,
        BasicRateWeeklyAmt = 5,
        ExtraDayRateAmt = 6,
        WeekendRate2DayAmt = 7,
        WeekendRate3DayAmt = 8
    }

    //Keep MerchantPeakSurchargeType have the same value with MerchantBaseRateType for the same rate type
    //If we don't keep then the same, Expedia.CarInterface.CarServiceTest.Util.CarCommonAlgorithmManager will have issues for merchant car pricing calculation
    public enum GetCarCostAndAvailGetResult_PeakSurchargeColumns
    {
        PeakSurchargeDailyAmt = 1,
        PeakSurcharge2DayAmt = 2,
        PeakSurcharge3DayAmt = 3,
        PeakSurcharge4DayAmt = 4,
        PeakSurchargeWeeklyAmt = 5,
        PeakSurchargeExtraDayAmt = 6,
        PeakSurcharge2DayWeekendAmt = 7,
        PeakSurcharge3DayWeekendAmt = 8
    }
    #endregion

    // result for CarVendorLocation
    public class CarVendorlocation
    {
        public int CarVendorLocationID;
        public string AirportCode;
        public string CarVendorLocationCode;
        public string CarVendorLocationName;
        public string StreeAddress1;
        public string CityName;
        public string StateProvinceCode;
        public string StateProvinceName;
        public string ISOCountryCode;
        public int SupplierID;

        public double Latitude;
        public double Longitude; 
        public string StatusCode;
        public string UpdateDate;
        public string LastUpdatedBy;

        public object LocationTypeID;
        public object CarShuttleCategoryCode; // instead of CarShuttleCategoryID;
        

        public object PostalCode; 
        public object PhoneNumber;
        public object FaxNumber;  
        public string DeliveryBool; //add it latere if need.    
        public string CollectionBool; //add it latere if need.    
        public string OutOfOfficeHoursBool; //add it latere if need.   
        public string CarShuttleCategoryID;

        public double distance;


        public CarVendorlocation() { }

        public CarVendorlocation(int myCarVendorLocationID, string myAirportCode, string myCarVendorLocationCode, string myCarVendorLocationName, string myStreeAddress1, string myStateProvenceCode, string myStateProvinceName,
            string myCityName, string myISOCountryCode, int mySupplierID, Object myLocationTypeID, Object myPostalCode, Object myFaxNumber,
            Object myLatitude, Object myLongitude, string myCarShuttleCategoryID = null, string myDeliveryBool = null, 
            string myCollectionBool= null, string myOutOfOfficeHoursBool = null, string myPhoneNumber = null)
        {
            CarVendorLocationID = myCarVendorLocationID;
            AirportCode = myAirportCode;
            CarVendorLocationCode = myCarVendorLocationCode;
            CarVendorLocationName = myCarVendorLocationName;
            StreeAddress1 = myStreeAddress1;
            CityName = myCityName;
            ISOCountryCode = myISOCountryCode;
            SupplierID = mySupplierID;
            LocationTypeID = myLocationTypeID;
            StateProvinceCode = myStateProvenceCode;
            PostalCode = myPostalCode;
            StateProvinceName = myStateProvinceName;
            FaxNumber = myFaxNumber;
            Latitude = double.Parse(myLatitude.ToString());
            Longitude = double.Parse(myLongitude.ToString());
            CarVendorLocationID = myCarVendorLocationID;
            CarShuttleCategoryID = myCarShuttleCategoryID;
            DeliveryBool = myDeliveryBool;
            CollectionBool = myCollectionBool;
            OutOfOfficeHoursBool = myOutOfOfficeHoursBool;
            PhoneNumber = myPhoneNumber;
        }

        public CarPickupLocation getValuesOfCarPickupLocationFormat()
        {
            CarPickupLocation carPickupLocation = new CarPickupLocation();
            // CarLocationKey
            carPickupLocation.CarLocationKey.LocationCode = AirportCode;
            carPickupLocation.CarLocationKey.CarLocationCategoryCode = CarVendorLocationCode.Substring(0, 1);
            carPickupLocation.CarLocationKey.SupplierRawText = CarVendorLocationCode.Substring(1);

            //Address
            carPickupLocation.Address.FirstAddressLine = StreeAddress1;
            carPickupLocation.Address.CityName = CityName;
            carPickupLocation.Address.CountryAlpha3Code = ISOCountryCode;
            carPickupLocation.Address.ProvinceName = StateProvinceName;

            //Phone 
            if (PhoneNumber != null)
            {
                Phone phone = new Phone();
                carPickupLocation.PhoneList.Add(phone);
                phone.PhoneNumber = PhoneNumber.ToString().Trim();
            }

            //CarVendorLocationID
            carPickupLocation.CarVendorLocationID = Convert.ToUInt32(CarVendorLocationID);

            //SupplierID
            if (SupplierID != null)
                carPickupLocation.SupplierID = Convert.ToUInt32(SupplierID.ToString().Trim());

            return carPickupLocation;
        }

        public CarDropOffLocation getValuesOfCarDropOffLocationFormat()
        {
            CarDropOffLocation carDropOffLocation = new CarDropOffLocation();
            // CarLocationKey
            carDropOffLocation.CarLocationKey.LocationCode = AirportCode;
            carDropOffLocation.CarLocationKey.CarLocationCategoryCode = CarVendorLocationCode.Substring(0, 1);
            carDropOffLocation.CarLocationKey.SupplierRawText = CarVendorLocationCode.Substring(1);

            //Address
            carDropOffLocation.Address.FirstAddressLine = StreeAddress1;
            carDropOffLocation.Address.CityName = CityName;
            carDropOffLocation.Address.CountryAlpha3Code = ISOCountryCode;
            carDropOffLocation.Address.ProvinceName = StateProvinceName;

            //Phone 
            if (PhoneNumber != null)
            {
                Phone phone = new Phone();
                carDropOffLocation.PhoneList.Add(phone);
                phone.PhoneNumber = PhoneNumber.ToString().Trim();
            }

            //CarVendorLocationID
            carDropOffLocation.CarVendorLocationID = Convert.ToUInt32(CarVendorLocationID);

            //SupplierID
            if (SupplierID != null)
                carDropOffLocation.SupplierID = Convert.ToUInt32(SupplierID.ToString().Trim());
            return carDropOffLocation;
        }

    }

    public class CarVendorLocation_providerData
    {
        public object ProviderID;
        public object Supplier;
        public object IATACode;
        public object LocationCode;
        public object LocationName;
        public object StreetAddress1;
        public object CityName;
        public object StateProvinceCode;
        public object StateProvinceName;
        public object PostalCode;
        public object ISOCountryCode;
        public object PhoneNumber;
        public object FaxNumber;
        public object Latitude;
        public object Longitude;
        public object LocationType;
        public object ShuttleCategory;
        public object DeliveryBool;
        public object CollectionBool;
        public object OutOfOfficeHoursBool;
        public object CreateDate;
        public object CreatedBy;

        public bool nameAddressMatch { get; set; }
        public bool locCodeIdMatch { get; set; }
        public bool distanceMatch { get; set; }
        public bool distanceEqual { get; set; }


        public CarVendorLocation_providerData() { }

        public CarVendorLocation_providerData(object myProviderID, object mySupplier, object myTATACode, object myLocationCode,
            object myLocationName, object myStreetAddress1, object myCityName, object myStateProvinceCode, object myStateProvinceName,
            object myPostalCode, object myISOCountryCode, object myPhoneNumber, object myFaxNumber, object myLatitude,
            object myLongitude, object myLocationType, object myShuttleCategory, object myDeliveryBool,
            object myCollectionBool, object myOutOfOfficeHoursBool, object myCreateDate, object myCreatedBy)
        {
            ProviderID = myProviderID;
            Supplier = mySupplier;
            IATACode = myTATACode;
            LocationCode = myLocationCode;
            LocationName = myLocationName;
            StreetAddress1 = myStreetAddress1;
            CityName = myCityName;
            StateProvinceCode = myStateProvinceCode;
            StateProvinceName = myStateProvinceName;
            PostalCode = myPostalCode;
            ISOCountryCode = myISOCountryCode;
            PhoneNumber = myPhoneNumber;
            FaxNumber = myFaxNumber;
            Latitude = myLatitude;
            Longitude = myLongitude;
            LocationType = myLocationType;
            ShuttleCategory = myShuttleCategory;
            DeliveryBool = myDeliveryBool;
            CollectionBool = myCollectionBool;
            OutOfOfficeHoursBool = myOutOfOfficeHoursBool;
            CreateDate = myCreateDate;
            CreatedBy = myCreatedBy;
        }
    }

    //SupplySubSetToWorldSpanSupplierItemMap table
    public class SupplySubSetToWorldSpanSupplierItemMapInfo
    {
        public uint supplySubsetID { get; set; }
        public string iataAgencyCode { get; set; }
        public string corporateDiscountCode { get; set; }
        public bool corporateDiscountCodeRequiredInShopping { get; set; }
        public string itNumber { get; set; }
        public string rateCode { get; set; }
    }
}
