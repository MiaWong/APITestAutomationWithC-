using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expedia.CarInterface.CarServiceTest.DBOperation.DBUtil
{
    #region CarReservationData
    public class CarReservationData
    {
        public object JurisdictionCode { get; set; }
        public object CompanyCode { get; set; }
        public object ManagementUnitCode { get; set; }
        public object BookingItemID { get; set; }
        public object CarReservationNodeData { get; set; }
        public object CarReservationNodeMajorVersion { get; set; }
        public object CarReservationNodeMinorVersion { get; set; }
        public object CarReservationDataExtendedElementCnt { get; set; }
        public object CarReservationDataExtendedPriceListCnt { get; set; }
        public object UseDateEnd { get; set; }
        public object CreateDate { get; set; }

        public CarReservationData()
        { 
        }

        public CarReservationData(object myJurisdictionCode, object myCompanyCode, object myManagementUnitCode, object myBookingItemID, 
            object myCarReservationNodeData, object myCarReservationNodeMajorVersion,  object myCarReservationNodeMinorVersion, 
            object myCarReservationDataExtendedElementCnt, object myCarReservationDataExtendedPriceListCnt,
            object myUseDateEnd, object myCreateDate)
        {
            JurisdictionCode = myJurisdictionCode;
            CompanyCode = myCompanyCode;
            ManagementUnitCode = myManagementUnitCode;
            BookingItemID = myBookingItemID;
            CarReservationNodeData = myCarReservationNodeData;
            CarReservationNodeMajorVersion = myCarReservationNodeMajorVersion;
            CarReservationNodeMinorVersion = myCarReservationNodeMinorVersion;
            CarReservationDataExtendedElementCnt = myCarReservationDataExtendedElementCnt;
            CarReservationDataExtendedPriceListCnt = myCarReservationDataExtendedPriceListCnt;
            UseDateEnd = myUseDateEnd;
            CreateDate = myCreateDate;
        }
    }

    public class CarReservationDataExtended
    {
        public object JurisdictionCode { get; set; }
        public object CompanyCode { get; set; }
        public object ManagementUnitCode { get; set; }
        public object BookingItemID { get; set; }
        public object SeqNbr { get; set; }
        public object MonetaryClassID { get; set; }
        public object MonetaryCalculationSystemID { get; set; }
        public object MonetaryCalculationID { get; set; }
        public object CurrencyCodeCost { get; set; }
        public object TransactionAmtCost { get; set; }
        public object DescCost { get; set; }
        public object FinanceCategoryCodeCost { get; set; }
        public object FinanceApplicationCodeCost { get; set; }
        public object CurrencyCodePrice { get; set; }
        public object TransactionAmtPrice { get; set; }
        public object DescPrice { get; set; }
        public object FinanceCategoryCodePrice { get; set; }
        public object FinanceApplicationCodePrice { get; set; }
        public object CreateDate { get; set; }
        public object FinanceApplicationUnitCntCost { get; set; }
        public object FinanceApplicationUnitCntPrice { get; set; }
        public object MultiplierPctCost { get; set; }
        public object MultiplierPctPrice { get; set; }

        public CarReservationDataExtended()
        {
            //Default values for Cost
            CurrencyCodeCost = "";
            TransactionAmtCost = 0;
            DescCost = "";
            FinanceApplicationCodeCost = "";
            FinanceCategoryCodeCost = "";
            MultiplierPctCost = 0;
            //Default values for Price
            CurrencyCodePrice = "";
            TransactionAmtPrice = 0;
            DescPrice = "";
            FinanceApplicationCodePrice = "";
            FinanceCategoryCodePrice = "";
            MultiplierPctPrice = 0;
            //Default values for Monetary values
            MonetaryClassID = 0;
            MonetaryCalculationSystemID = 0;
            MonetaryCalculationID = 0;
            FinanceApplicationUnitCntCost = "";
            FinanceApplicationUnitCntPrice = "";
        }

        public CarReservationDataExtended(object myJurisdictionCode, object myCompanyCode, object myManagementUnitCode, object myBookingItemID,
            object mySeqNbr, object myMonetaryClassID,  object myMonetaryCalculationSystemID, object myMonetaryCalculationID, object myCurrencyCodeCost,
            object myTransactionAmtCost, object myDescCost, object myFinanceCategoryCodeCost, object myFinanceApplicationCodeCost, object myFinanceApplicationUnitCntCost,
            object myCurrencyCodePrice, object myTransactionAmtPrice, object myDescPrice, object myFinanceCategoryCodePrice, object myFinanceApplicationCodePrice, object myFinanceApplicationUnitCntPrice,
            object myCreateDate, object myMultiplierPctCost, object myMultiplierPctPrice)
        {
            JurisdictionCode = myJurisdictionCode;
            CompanyCode = myCompanyCode;
            ManagementUnitCode = myManagementUnitCode;
            BookingItemID = myBookingItemID;
            SeqNbr = mySeqNbr;
            MonetaryClassID = myMonetaryClassID;
            MonetaryCalculationSystemID = myMonetaryCalculationSystemID;
            MonetaryCalculationID = myMonetaryCalculationID;
            CurrencyCodeCost = myCurrencyCodeCost;
            TransactionAmtCost = myTransactionAmtCost;
            DescCost = myDescCost;
            FinanceCategoryCodeCost = myFinanceCategoryCodeCost;
            FinanceApplicationCodeCost = myFinanceApplicationCodeCost;
            FinanceApplicationUnitCntCost = myFinanceApplicationUnitCntCost;
            CurrencyCodePrice = myCurrencyCodePrice;
            TransactionAmtPrice = myTransactionAmtPrice;
            DescPrice = myDescPrice;
            FinanceCategoryCodePrice = myFinanceCategoryCodePrice;
            FinanceApplicationCodePrice = myFinanceApplicationCodePrice;
            FinanceApplicationUnitCntPrice = myFinanceApplicationUnitCntPrice;
            CreateDate = myCreateDate;
            MultiplierPctCost = myMultiplierPctCost;
            MultiplierPctPrice = myMultiplierPctPrice;
        }
    }
    #endregion
}
