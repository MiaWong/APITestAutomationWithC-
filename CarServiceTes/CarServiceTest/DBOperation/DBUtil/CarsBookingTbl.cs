using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Expedia.CarInterface.CarServiceTest.Verification.Common;

namespace Expedia.CarInterface.CarServiceTest.DBOperation.DBUtil
{
    #region BookingAmount
    public class BookingAmount : MaseratiBookingAbstract
    {
        public object BookingID { get; set; }
        public object BookingAmountSeqNbr { get; set; }
        public object BookingItemID { get; set; }
        public object BookingItemTypeID { get; set; }
        public object CurrencyCodeCost { get; set; }
        public object CurrencyCodePrice { get; set; }
        public object BookingAmountRefCodeCost { get; set; }
        public object BookingAmountRefCodePrice { get; set; }
        public object CancelBool { get; set; }
        public object CreateDate { get; set; }
        public object CreateTUID { get; set; }
        public object BookingAmountLevelID { get; set; }
        public object BookingItemInventorySeqNbr { get; set; }
        public object MonetaryClassID { get; set; }
        public object MonetaryCalculationSystemID { get; set; }
        public object MonetaryCalculationID { get; set; }
        public object BookingAmountDescCost { get; set; }
        public object TransactionAmtCost { get; set; }
        public object TransactionAmtPrice { get; set; }
        public object BookingAmountDescPrice { get; set; }
        public object BookingAmountRowGUID { get; set; }

        public BookingAmount(object myBookingID, object myBookingAmountSeqNbr, object myBookingItemID, object myBookingItemTypeID, object myCurrencyCodeCost, object myCurrencyCodePrice,
            object myBookingAmountRefCodeCost, object myBookingAmountRefCodePrice, object myCancelBool, object myCreateDate, object myCreateTUID, object myBookingAmountLevelID,
            object myBookingItemInventorySeqNbr, object myMonetaryClassID, object myMonetaryCalculationSystemID, object myMonetaryCalculationID, object myBookingAmountDescCost,
            object myTransactionAmtCost, object myTransactionAmtPrice, object myBookingAmountDescPrice, object myBookingAmountRowGUID)
        {
            BookingID = myBookingID;
            BookingAmountSeqNbr = myBookingAmountSeqNbr;
            BookingItemID = myBookingItemID;
            BookingItemTypeID = myBookingItemTypeID;
            CurrencyCodeCost = myCurrencyCodeCost;
            CurrencyCodePrice = myCurrencyCodePrice;
            BookingAmountRefCodeCost = myBookingAmountRefCodeCost;
            BookingAmountRefCodePrice = myBookingAmountRefCodePrice;
            CancelBool = myCancelBool;
            CreateDate = myCreateDate;
            CreateTUID = myCreateTUID;
            BookingAmountLevelID = myBookingAmountLevelID;
            BookingItemInventorySeqNbr = myBookingItemInventorySeqNbr;
            MonetaryClassID = myMonetaryClassID;
            MonetaryCalculationSystemID = myMonetaryCalculationSystemID;
            MonetaryCalculationID = myMonetaryCalculationID;
            BookingAmountDescCost = myBookingAmountDescCost;
            TransactionAmtCost = myTransactionAmtCost;
            TransactionAmtPrice = myTransactionAmtPrice;
            BookingAmountDescPrice = myBookingAmountDescPrice;
            BookingAmountRowGUID = myBookingAmountRowGUID;
        }

        public BookingAmount()
        {

        }

        public BookingAmount CloneCommonAmount()
        {
            BookingAmount amountClone = new BookingAmount();
            amountClone.BookingID = this.BookingID;
            amountClone.BookingAmountSeqNbr = this.BookingAmountSeqNbr;
            amountClone.BookingItemID = this.BookingItemID;
            amountClone.BookingItemTypeID = this.BookingItemTypeID;
            amountClone.CurrencyCodeCost = this.CurrencyCodeCost;
            amountClone.CurrencyCodePrice = this.CurrencyCodePrice;
            amountClone.CancelBool = this.CancelBool;
            amountClone.CreateDate = this.CreateDate;
            amountClone.CreateTUID = this.CreateTUID;
            return amountClone;
        }
    }
    #endregion

    #region BookingItem
    public class BookingItem
    {
        public object BookingItemID { get; set; }
        public object BookingID { get; set; }
        public object BookingRecordSystemReferenceCode { get; set; }
        public object BookingItemIDOriginal { get; set; }
        public object BookingItemIDPrior { get; set; }
        public object BookingItemTypeID { get; set; }
        public object BookingItemStateID { get; set; }
        public object BookingItemStateIDPending { get; set; }
        public object BookingFulfillmentMethodID { get; set; }
        public object BookingFulfillmentStateID { get; set; }
        public object BookingFulfillmentDate { get; set; }
        public object UseDateBegin { get; set; }
        public object UseDateEnd { get; set; }
        public object BookingRecordSystemID { get; set; }
        public object AccountingVendorID { get; set; }
        public object SupplierBookingConfirmationCode { get; set; }
        public object SupplierBookingConfirmationDate { get; set; }
        public object BookingItemDesc { get; set; }
        public object CancelDate { get; set; }
        public object CancelTUID { get; set; }
        public object CreateDate { get; set; }
        public object CreateTUID { get; set; }
        public object UpdateDate { get; set; }
        public object UpdateTravelProductID { get; set; }
        public object UpdateTUID { get; set; }
        public object RevenueReportingTypeID { get; set; }
        public object BookDate { get; set; }
        public object BookTUID { get; set; }

        public BookingItem(object myBookingItemID, object myBookingID, object myBookingRecordSystemReferenceCode, object myBookingItemIDOriginal, object myBookingItemIDPrior, object myBookingItemTypeID,
            object myBookingItemStateID, object myBookingItemStateIDPending, object myBookingFulfillmentMethodID, object myBookingFulfillmentStateID, object myBookingFulfillmentDate,
            object myUseDateBegin, object myUseDateEnd, object myBookingRecordSystemID, object myAccountingVendorID, object mySupplierBookingConfirmationCode,
            object mySupplierBookingConfirmationDate, object myBookingItemDesc, object myCancelDate, object myCancelTUID, object myCreateDate, object myCreateTUID, object myUpdateDate, object myUpdateTravelProductID,
            object myUpdateTUID, object myRevenueReportingTypeID, object myBookDate, object myBookTUID)
        {
            BookingItemID = myBookingItemID;
            BookingID = myBookingID;
            BookingRecordSystemReferenceCode = myBookingRecordSystemReferenceCode;
            BookingItemIDOriginal = myBookingItemIDOriginal;
            BookingItemIDPrior = myBookingItemIDPrior;
            BookingItemTypeID = myBookingItemTypeID;
            BookingItemStateID = myBookingItemStateID;
            BookingItemStateIDPending = myBookingItemStateIDPending;
            BookingFulfillmentMethodID = myBookingFulfillmentMethodID;
            BookingFulfillmentStateID = myBookingFulfillmentStateID;
            BookingFulfillmentDate = myBookingFulfillmentDate;
            UseDateBegin = myUseDateBegin;
            UseDateEnd = myUseDateEnd;
            BookingRecordSystemID = myBookingRecordSystemID;
            AccountingVendorID = myAccountingVendorID;
            SupplierBookingConfirmationCode = mySupplierBookingConfirmationCode;
            SupplierBookingConfirmationDate = mySupplierBookingConfirmationDate;
            BookingItemDesc = myBookingItemDesc;
            CancelDate = myCancelDate;
            CancelTUID = myCancelTUID;
            CreateDate = myCreateDate;
            CreateTUID = myCreateTUID;
            UpdateDate = myUpdateDate;
            UpdateTravelProductID = myUpdateTravelProductID;
            UpdateTUID = myUpdateTUID;
            RevenueReportingTypeID = myRevenueReportingTypeID;
            BookDate = myBookDate;
            BookTUID = myBookTUID;
        }
    }
    #endregion

    #region Booking
    public class Booking
    {
        public object BookingID { get; set; }
        public object TravelProductID { get; set; }
        public object TRL { get; set; }
        public object TUID { get; set; }
        public object TravelPackageTypeID { get; set; }
        public object BookingEndDate { get; set; }
        public object WizardID { get; set; }
        public object MarketingProgramID { get; set; }
        public object PartnerID { get; set; }
        public object ReferralTrackingServiceID { get; set; }
        public object ReferralTrackingNbr { get; set; }
        public object ABTestGroupID { get; set; }
        public object ItineraryPurposeMask { get; set; }
        public object GroupAccountID { get; set; }
        public object BookingDesc { get; set; }
        public object CreateDate { get; set; }
        public object CreateTUID { get; set; }
        public object UpdateDate { get; set; }
        public object UpdateTUID { get; set; }
        public object LangID { get; set; }
        public object GroupAccountDepartmentID { get; set; }
        public object AgentAssistedBool { get; set; }
        public object AffiliateID { get; set; }
        public object OperatingUnitID { get; set; }

        public Booking()
        {
        }

        public Booking(object myBookingID, object myTravelProductID, object myTRL, object myTUID, object myTravelPackageTypeID,
            object myBookingEndDate, object myWizardID, object myMarketingProgramID, object myPartnerID, object myReferralTrackingServiceID,
            object myReferralTrackingNbr, object myABTestGroupID, object myItineraryPurposeMask, object myGroupAccountID,
            object myBookingDesc, object myCreateDate, object myCreateTUID, object myUpdateDate, object myUpdateTUID,
            object myLangID, object myGroupAccountDepartmentID, object myAgentAssistedBool, object myAffiliateID, object myOperatingUnitID)
        {
            BookingID = myBookingID;
            TravelProductID = myTravelProductID;
            TRL = myTRL;
            TUID = myTUID;
            TravelPackageTypeID = myTravelPackageTypeID;
            BookingEndDate = myBookingEndDate;
            WizardID = myWizardID;
            MarketingProgramID = myMarketingProgramID;
            PartnerID = myPartnerID;
            ReferralTrackingServiceID = myReferralTrackingServiceID;
            ReferralTrackingNbr = myReferralTrackingNbr;
            ABTestGroupID = myABTestGroupID;
            ItineraryPurposeMask = myItineraryPurposeMask;
            GroupAccountID = myGroupAccountID;
            BookingDesc = myBookingDesc;
            CreateDate = myCreateDate;
            CreateTUID = myCreateTUID;
            UpdateDate = myUpdateDate;
            UpdateTUID = myUpdateTUID;
            LangID = myLangID;
            GroupAccountDepartmentID = myGroupAccountDepartmentID;
            AgentAssistedBool = myAgentAssistedBool;
            AffiliateID = myAffiliateID;
            OperatingUnitID = myOperatingUnitID;
        }
    }
    #endregion Booking

    #region BookingItemCar
    public class BookingItemCar
    {
        public object BookingItemID { get; set; }
        public object CarVendorCode { get; set; }
        public object AirportCodePickUp { get; set; }
        public object AirportCodeDropOff { get; set; }
        public object CarVendorLocationCodePickUp { get; set; }
        public object CarVendorLocationCodeDropOff { get; set; }
        public object CarCategoryID { get; set; }
        public object CarTypeID { get; set; }
        public object CarTransmissionDriveID { get; set; }
        public object CarFuelAirConditionID { get; set; }
        public object SearchTypeIDPickup { get; set; }
        public object ResultTypeIDPickUp { get; set; }
        public object FlightNumberArrival { get; set; }
        public object AirlineCodeArrival { get; set; }
        public object CarBusinessModelID { get; set; }
        public object CarItemID { get; set; }
        public object SupplySubsetID { get; set; }
        public object CarCorpDiscountCodeCRSReq { get; set; }
        public object CarCorpDiscountCodeCRSResp { get; set; }
        public object CarRateCodeCRSReq { get; set; }
        public object CarRateCodeCRSResp { get; set; }
        public object CarClubNbrCRSReq { get; set; }
        public object CarClubNbrCRSResp { get; set; }
        public object CarCouponCodeCRSReq { get; set; }
        public object CarCouponCodeCRSResp { get; set; }
        public object CarSupplementalCodeCRSReq { get; set; }
        public object CarSupplementalCodeCRSResp { get; set; }
        public object ExchangeRateCurrencyCode { get; set; }
        public object ExchangeRate { get; set; }
        public object EstimatedTotalTaxAndFeeAmt { get; set; }
        public object EstimatedTotalTaxAndFeeAmtCurrencyCode { get; set; }
        public object CarSpecialEquipmentMask { get; set; }
        public object CarInsuranceAndWaiverMask { get; set; }
        public object FrequentFlyerPlanName { get; set; }
        public object FrequentFlyerPlanNumber { get; set; }
        public object CarCorpDiscountCodeTraveler { get; set; }
        public object CarRateCodeTraveler { get; set; }
        public object CarClubNbrTraveler { get; set; }
        public object CarCouponCodeTraveler { get; set; }
        public object CarSupplementalCodeTraveler { get; set; }
        public object CarRateOptionTypeIDTraveler { get; set; }
        public object ReservationGuaranteeMethodID { get; set; }
        public object CreditCardTypeID { get; set; }
        public object CreditCardLastFourDigitNbr { get; set; }
        public object UnlimitedMileageBool { get; set; }
        public object MileageChargeAmt { get; set; }
        public object MileageChargeAmtCurrencyCode { get; set; }
        public object MileageUnitInKMBool { get; set; }
        public object CarVoucherNbr { get; set; }
        public object CarAgreementID { get; set; }
        public object CarVendorAgreementNbr { get; set; }
        public object CarRentalDaysCnt { get; set; }
        public object PublishedPriceAmt { get; set; }
        public object CarBasePricePeriodPublishedID { get; set; }
        public object CurrencyCodePublishedPrice { get; set; }
        public object MarginMaxAmt { get; set; }
        public object MarginMaxCurrencyCode { get; set; }
        public object MarginMinAmt { get; set; }
        public object MarginMinCurrencyCode { get; set; }
        public object MarkupPct { get; set; }
        public object DiscountAmt { get; set; }
        public object DiscountAmtCurrencyCode { get; set; }

        public BookingItemCar()
        {

        }

        public BookingItemCar(object myBookingItemID, object myCarVendorCode, object myAirportCodePickUp, object myAirportCodeDropOff,
            object myCarVendorLocationCodePickUp, object myCarVendorLocationCodeDropOff, object myCarCategoryID, object myCarTypeID,
            object myCarTransmissionDriveID, object myCarFuelAirConditionID, object mySearchTypeIDPickup, object myResultTypeIDPickUp,
            object myFlightNumberArrival, object myAirlineCodeArrival, object myCarBusinessModelID, object myCarItemID,
            object mySupplySubsetID, object myCarCorpDiscountCodeCRSReq, object myCarCorpDiscountCodeCRSResp, object myCarRateCodeCRSReq,
            object myCarRateCodeCRSResp, object myCarClubNbrCRSReq, object myCarClubNbrCRSResp, object myCarCouponCodeCRSReq,
            object myCarCouponCodeCRSResp, object myCarSupplementalCodeCRSReq, object myCarSupplementalCodeCRSResp, object myExchangeRateCurrencyCode,
            object myExchangeRate, object myEstimatedTotalTaxAndFeeAmt, object myEstimatedTotalTaxAndFeeAmtCurrencyCode, object myCarSpecialEquipmentMask,
            object myCarInsuranceAndWaiverMask, object myFrequentFlyerPlanName, object myFrequentFlyerPlanNumber, object myCarCorpDiscountCodeTraveler,
            object myCarRateCodeTraveler, object myCarClubNbrTraveler, object myCarCouponCodeTraveler, object myCarSupplementalCodeTraveler,
            object myCarRateOptionTypeIDTraveler, object myReservationGuaranteeMethodID, object myCreditCardTypeID, object myCreditCardLastFourDigitNbr,
            object myUnlimitedMileageBool, object myMileageChargeAmt, object myMileageChargeAmtCurrencyCode, object myMileageUnitInKMBool,
            object myCarVoucherNbr, object myCarAgreementID, object myCarVendorAgreementNbr, object myCarRentalDaysCnt,
            object myPublishedPriceAmt, object myCarBasePricePeriodPublishedID, object myCurrencyCodePublishedPrice, object myMarginMaxAmt,
            object myMarginMaxCurrencyCode, object myMarginMinAmt, object myMarginMinCurrencyCode, object myMarkupPct, object myDiscountAmt,
            object myDiscountAmtCurrencyCode)
        {
            BookingItemID = myBookingItemID;
            CarVendorCode = myCarVendorCode;
            AirportCodePickUp = myAirportCodePickUp;
            AirportCodeDropOff = myAirportCodeDropOff;
            CarVendorLocationCodePickUp = myCarVendorLocationCodePickUp;
            CarVendorLocationCodeDropOff = myCarVendorLocationCodeDropOff;
            CarCategoryID = myCarCategoryID;
            CarTypeID = myCarTypeID;
            CarTransmissionDriveID = myCarTransmissionDriveID;
            CarFuelAirConditionID = myCarFuelAirConditionID;
            SearchTypeIDPickup = mySearchTypeIDPickup;
            ResultTypeIDPickUp = myResultTypeIDPickUp;
            FlightNumberArrival = myFlightNumberArrival;
            AirlineCodeArrival = myAirlineCodeArrival;
            CarBusinessModelID = myCarBusinessModelID;
            CarItemID = myCarItemID;
            SupplySubsetID = mySupplySubsetID;
            CarCorpDiscountCodeCRSReq = myCarCorpDiscountCodeCRSReq;
            CarCorpDiscountCodeCRSResp = myCarCorpDiscountCodeCRSResp;
            CarRateCodeCRSReq = myCarRateCodeCRSReq;
            CarRateCodeCRSResp = myCarRateCodeCRSResp;
            CarClubNbrCRSReq = myCarClubNbrCRSReq;
            CarClubNbrCRSResp = myCarClubNbrCRSResp;
            CarCouponCodeCRSReq = myCarCouponCodeCRSReq;
            CarCouponCodeCRSResp = myCarCouponCodeCRSResp;
            CarSupplementalCodeCRSReq = myCarSupplementalCodeCRSReq;
            CarSupplementalCodeCRSResp = myCarSupplementalCodeCRSResp;
            ExchangeRateCurrencyCode = myExchangeRateCurrencyCode;
            ExchangeRate = myExchangeRate;
            EstimatedTotalTaxAndFeeAmt = myEstimatedTotalTaxAndFeeAmt;
            EstimatedTotalTaxAndFeeAmtCurrencyCode = myEstimatedTotalTaxAndFeeAmtCurrencyCode;
            CarSpecialEquipmentMask = myCarSpecialEquipmentMask;
            CarInsuranceAndWaiverMask = myCarInsuranceAndWaiverMask;
            FrequentFlyerPlanName = myFrequentFlyerPlanName;
            FrequentFlyerPlanNumber = myFrequentFlyerPlanNumber;
            CarCorpDiscountCodeTraveler = myCarCorpDiscountCodeTraveler;
            CarRateCodeTraveler = myCarRateCodeTraveler;
            CarClubNbrTraveler = myCarClubNbrTraveler;
            CarCouponCodeTraveler = myCarCouponCodeTraveler;
            CarSupplementalCodeTraveler = myCarSupplementalCodeTraveler;
            CarRateOptionTypeIDTraveler = myCarRateOptionTypeIDTraveler;
            ReservationGuaranteeMethodID = myReservationGuaranteeMethodID;
            CreditCardTypeID = myCreditCardTypeID;
            CreditCardLastFourDigitNbr = myCreditCardLastFourDigitNbr;
            UnlimitedMileageBool = myUnlimitedMileageBool;
            MileageChargeAmt = myMileageChargeAmt;
            MileageChargeAmtCurrencyCode = myMileageChargeAmtCurrencyCode;
            MileageUnitInKMBool = myMileageUnitInKMBool;
            CarVoucherNbr = myCarVoucherNbr;
            CarAgreementID = myCarAgreementID;
            CarVendorAgreementNbr = myCarVendorAgreementNbr;
            CarRentalDaysCnt = myCarRentalDaysCnt;
            PublishedPriceAmt = myPublishedPriceAmt;
            CarBasePricePeriodPublishedID = myCarBasePricePeriodPublishedID;
            CurrencyCodePublishedPrice = myCurrencyCodePublishedPrice;
            MarginMaxAmt = myMarginMaxAmt;
            MarginMaxCurrencyCode = myMarginMaxCurrencyCode;
            MarginMinAmt = myMarginMinAmt;
            MarginMinCurrencyCode = myMarginMinCurrencyCode;
            MarkupPct = myMarkupPct;
            DiscountAmt = myDiscountAmt;
            DiscountAmtCurrencyCode = myDiscountAmtCurrencyCode;
        }
    }
    #endregion

    #region BookingItemCarMarkup
    //BookingItemID        int           not null,
    //SourceCategory       varchar(50)   not null,
    //SourceID             int           not null,
    //MarkupPct            decimal(5,4)  null,
    //MarkupAmt            money         null,
    //MarkupCurrencyCode   char(3)       null,
    //CreateDate           datetime      not null,
    
    public class BookingItemCarMarkup
    {
        public object BookingItemID;
        public object SourceCategory;
        public object SourceID;
        public object MarkupPct;
        public object MarkupAmt;
        public object MarkupCurrencyCode;
        public object CreateDate;

        public BookingItemCarMarkup()
        {

        }

        public BookingItemCarMarkup(object myBookingItemID, object mySourceCategory, object mySourceID,
            object myMarkupPct, object myMarkupAmt, object myMarkupCurrencyCode, object myCreateDate)
        {
            BookingItemID = myBookingItemID;
            SourceCategory = mySourceCategory;
            SourceID = mySourceID;
            MarkupPct = myMarkupPct;
            MarkupAmt = myMarkupAmt;
            MarkupCurrencyCode = myMarkupCurrencyCode;
            CreateDate = myCreateDate;
        }
    }
    #endregion

    #region BookingItemCarMarkupExtended
    //    BookingItemID    int           not null,
    //SourceCategory   varchar(50)   not null,
    //SourceID         int           not null,
    //AttributeName      varchar(150)  not null,
    //AttributeValue     varchar(1000) not null,
    //CreateDate       datetime      not null,

    public class BookingItemCarMarkupExtended
    {
        public object BookingItemID;
        public object SourceCategory;
        public object SourceID;
        public object AttributeName;
        public object AttributeValue;
        public object CreateDate;

        public BookingItemCarMarkupExtended()
        {

        }

        public BookingItemCarMarkupExtended(object myBookingItemID, object mySourceCategory, object mySourceID,
            object myAttributeName, object myAttributeValue, object myCreateDate)
        {
            BookingItemID = myBookingItemID;
            SourceCategory = mySourceCategory;
            SourceID = mySourceID;
            AttributeName = myAttributeName;
            AttributeValue = myAttributeValue;
            CreateDate = myCreateDate;
        }
    }
    #endregion

    #region BookingItemCarInventory
    public class BookingItemCarInventory
    {
        public object BookingItemID;
        public object BookingItemInventorySeqNbr;
        public object StartDate;
        public object EndDate;
        public object CarInternalBasePricePeriodID;
        public object BaseAmtCost;
        public object PeakSurchargeAmtCost;

        public BookingItemCarInventory(object myBookingItemID, object myBookingItemInventorySeqNbr, object myStartDate, object myEndDate,
            object myCarInternalBasePricePeriodID, object myBaseAmtCost, object myPeakSurchargeAmtCost)
        {
            BookingItemID = myBookingItemID;
            BookingItemInventorySeqNbr = myBookingItemInventorySeqNbr;
            StartDate = myStartDate;
            EndDate = myEndDate;
            CarInternalBasePricePeriodID = myCarInternalBasePricePeriodID;
            BaseAmtCost = myBaseAmtCost;
            PeakSurchargeAmtCost = myPeakSurchargeAmtCost;
        }
    }
    #endregion BookingItemCar


    #region CreditCardType
    public class CreditCardType
    {
        public int CreditCardTypeID;
        public string Description;

        public CreditCardType(object myCreditCardTypeID, object myDescription)
        {
            CreditCardTypeID = Convert.ToInt32(myCreditCardTypeID);
            Description = Convert.ToString(myDescription).Replace(" ", "");
        }

        public CreditCardType()
        {
        }
    }
    #endregion


    #region Bookingtraveler
    public class Bookingtraveler : MaseratiBookingAbstract
    {
        public object BookingID { get; set; }
        public object BookingTravelerSeqNbr { get; set; }
        public object ActiveBool { get; set; }
        public object TUID { get; set; }
        public object PersonalTitleID { get; set; }
        public object PersonAgeYearCnt { get; set; }
        public object PersonAgeTypeID { get; set; }
        public object UserEmailAdr { get; set; }               
        public object CityName { get; set; }
        public object RegionName { get; set; }
        public object PostalCode { get; set; }
        public object CountryCode { get; set; }
        public object CreateDate { get; set; }
        public object CreateTUID { get; set; }
        public object UpdateDate { get; set; }
        public object UpdateTUID { get; set; }
        public object BirthDate { get; set; }
        public object GenderID { get; set; }
        public object NationalityCountryCode { get; set; }

        public Bookingtraveler(object myBookingID, object myBookingTravelerSeqNbr, object myActiveBool, object myTUID, object myPersonalTitleID, object myPersonAgeYearCnt,
            object myPersonAgeTypeID, object myUserEmailAdr, object myCityName, object myCreateDate, object myCreateTUID, object myRegionName,
            object myPostalCode, object myCountryCode, object myUpdateDate, object myUpdateTUID, object myBirthDate,
            object myGenderID, object myNationalityCountryCode)
        {
            BookingID = myBookingID;
            BookingTravelerSeqNbr = myBookingTravelerSeqNbr;
            ActiveBool = myActiveBool;
            TUID = myTUID;
            PersonalTitleID = myPersonalTitleID;
            PersonAgeYearCnt = myPersonAgeYearCnt;
            PersonAgeTypeID = myPersonAgeTypeID;
            UserEmailAdr = myUserEmailAdr;
            CityName = myCityName;
            CreateDate = myCreateDate;
            CreateTUID = myCreateTUID;
            RegionName = myRegionName;
            PostalCode = myPostalCode;
            CountryCode = myCountryCode;
            UpdateDate = myUpdateDate;
            UpdateTUID = myUpdateTUID;
            BirthDate = myBirthDate;
            GenderID = myGenderID;
            NationalityCountryCode = myNationalityCountryCode;            
        }

        public Bookingtraveler()
        {

        }

        public Bookingtraveler CloneCommonAmount()
        {
            Bookingtraveler travelerClone = new Bookingtraveler();
            travelerClone.BookingID = this.BookingID;
            travelerClone.BookingTravelerSeqNbr = this.BookingTravelerSeqNbr;
            travelerClone.ActiveBool = this.ActiveBool;
            travelerClone.TUID = this.TUID;
            travelerClone.CreateTUID = this.CreateTUID;
            travelerClone.UpdateTUID = this.UpdateTUID;
            travelerClone.CreateDate = this.CreateDate;
            travelerClone.CreateTUID = this.CreateTUID;
            return travelerClone;
        }
    }
    #endregion
}
