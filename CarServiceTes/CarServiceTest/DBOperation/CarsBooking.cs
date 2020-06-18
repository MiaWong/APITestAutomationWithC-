using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Expedia.CarInterface.CarServiceTest.DBOperation.DBUtil;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class CarsBooking
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["CarsBooking"].ConnectionString;

        //Purpose: Get the max TRL to get an available TRl to be used for booking, but it seems like that the logic is not good, so don't use this method now
        //Meichun: get max TRL from CarsBooking/Booking still can't work, if the valid logic to query max TRL which can be used for booking is availalbe, please update this method.
        public static int getMaxTRL(int tpid)
        {
            int maxTRL = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select TRL from Booking where BookingID = (select MAX(BookingID) from Booking where TravelProductID = {0})", tpid);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    maxTRL = int.Parse(reader[0].ToString());
                }
                conn.Close();
            }

            return maxTRL;

        }

        //Get the bookingID from table Booking according to TRL
        public static int getBookingID(int trl)
        {
            int bookingID = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("select BookingID from Booking where TRL = {0}", trl);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookingID = int.Parse(reader[0].ToString());
                }
                conn.Close();
            }
            return bookingID;
        }

        public static string GetBookingItemIDByTRL(string TRL)
        {
            string bookingItemID = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("select BookingItemID from BookingItem where BookingID = (select BookingID from Booking where TRL = {0})", TRL);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookingItemID = reader[0].ToString();
                }
                conn.Close();
            }
            return bookingItemID;
        }

        public static int GetBookingRecordSystemIDByBookingItemID(int BookingItemID)
        {
            int BookingRecordSystemID = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("select BookingRecordSystemID from BookingItem where BookingItemID = {0}", BookingItemID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    BookingRecordSystemID = Convert.ToInt16(reader[0]);
                }
                conn.Close();
            }
            return BookingRecordSystemID;
        }

        //Get the bookingItemID from table BookingItem according to bookingID
        public static int getBookingItemID(int bookingID)
        {
            int bookingItemID = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("select BookingItemID from BookingItem where BookingID = {0}", bookingID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookingItemID = int.Parse(reader[0].ToString());
                }
                conn.Close();
            }
            return bookingItemID;
        }

        //Get the priceAmount basic on bookingID
        public static DataTable getBookingAmount(int bookingID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from BookingAmount where BookingID = {0}", bookingID);
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        public static List<BookingAmount> getBookingAmountByID(int bookingID, string cancelBool = null)
        {
            List<BookingAmount> bookingAmountList = new List<BookingAmount>();
            BookingAmount bookingAmount;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                //string cancelBoolS = (null != cancelBool) ? "and cancelbool = " + cancelBool : "";
                if (cancelBool == "null" || cancelBool == "0")
                {
                    cmd.CommandText = string.Format("select * from BookingAmount where BookingID = {0} and BookingAmountSeqNbr > 0", bookingID);
                }
                else if (cancelBool == "1")
                {
                    cmd.CommandText = string.Format("select * from BookingAmount where BookingID = {0} and CancelBool = 1", bookingID);
                }
                else
                {
                    cmd.CommandText = string.Format("select * from BookingAmount where BookingID = {0}", bookingID);
                }
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookingAmount = new BookingAmount(reader["BookingID"], reader["BookingAmountSeqNbr"], reader["BookingItemID"], reader["BookingItemTypeID"],
                        reader["CurrencyCodeCost"], reader["CurrencyCodePrice"], reader["BookingAmountRefCodeCost"], reader["BookingAmountRefCodePrice"],
                        reader["CancelBool"], reader["CreateDate"], reader["CreateTUID"], reader["BookingAmountLevelID"], reader["BookingItemInventorySeqNbr"],
                        reader["MonetaryClassID"], reader["MonetaryCalculationSystemID"], reader["MonetaryCalculationID"], reader["BookingAmountDescCost"],
                        reader["TransactionAmtCost"], reader["TransactionAmtPrice"], reader["BookingAmountDescPrice"], reader["BookingAmountRowGUID"]);
                    bookingAmountList.Add(bookingAmount);
                }
                conn.Close();
            }
            return bookingAmountList;
        }

        public static List<Bookingtraveler> getBookingtravelerByID(int bookingID)
        {
            List<Bookingtraveler> bookingtravelerList = new List<Bookingtraveler>();
            Bookingtraveler bookingtraveler;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from Bookingtraveler where BookingID = {0}", bookingID);                
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookingtraveler = new Bookingtraveler(reader["BookingID"], reader["BookingTravelerSeqNbr"], reader["ActiveBool"], reader["TUID"],
                        reader["PersonalTitleID"], reader["PersonAgeYearCnt"], reader["PersonAgeTypeID"], reader["UserEmailAdr"],
                        reader["CityName"], reader["CreateDate"], reader["CreateTUID"], reader["RegionName"], reader["PostalCode"],
                        reader["CountryCode"], reader["UpdateDate"], reader["UpdateTUID"], reader["BirthDate"], reader["GenderID"], reader["NationalityCountryCode"]);
                    bookingtravelerList.Add(bookingtraveler);
                }
                conn.Close();
            }
            return bookingtravelerList;
        }

        public static DataTable GetBookingAmountTable(string bookingID)
        {
            string sqlCmd = string.Format("select * from BookingAmount where BookingId = {0}", bookingID);
            try
            {
                return GetDataTableUtil(sqlCmd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static List<BookingItem> getBookingItemByID(int bookingID)
        {
            List<BookingItem> bookingItemList = new List<BookingItem>();
            BookingItem bookingItem;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from BookingItem where BookingID  = {0}", bookingID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookingItem = new BookingItem(reader["BookingItemID"], reader["BookingID"], reader["BookingRecordSystemReferenceCode"], reader["BookingItemIDOriginal"], reader["BookingItemIDPrior"],
                        reader["BookingItemTypeID"], reader["BookingItemStateID"], reader["BookingItemStateIDPending"], reader["BookingFulfillmentMethodID"],
                        reader["BookingFulfillmentStateID"], reader["BookingFulfillmentDate"], reader["UseDateBegin"], reader["UseDateEnd"], reader["BookingRecordSystemID"],
                        reader["AccountingVendorID"], reader["SupplierBookingConfirmationCode"], reader["SupplierBookingConfirmationDate"], reader["BookingItemDesc"], reader["CancelDate"],
                        reader["CancelTUID"], reader["CreateDate"], reader["CreateTUID"], reader["UpdateDate"], reader["UpdateTravelProductID"], reader["UpdateTUID"],
                        reader["RevenueReportingTypeID"], reader["BookDate"], reader["BookTUID"]);
                    bookingItemList.Add(bookingItem);
                }
                conn.Close();
            }
            return bookingItemList;
        }

        //Get the BookingItemCar basic on bookingItemID
        public static DataTable getBookingItemCar(int bookingItemID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from BookingItemCar where BookingItemID = {0}", bookingItemID);
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        public static List<BookingItemCar> getBookingItemCarByBookingItemID(int bookingItemID)
        {
            List<BookingItemCar> bookingItemCarList = new List<BookingItemCar>();
            BookingItemCar bookingItemCar;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from BookingItemCar where BookingItemID = {0}", bookingItemID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    object discountAmt = null, discountCurrency = null;
                    if (reader.GetSchemaTable().Columns.Contains("DiscountAmt")) discountAmt = reader["DiscountAmt"];
                    if (reader.GetSchemaTable().Columns.Contains("DiscountAmtCurrencyCode")) discountCurrency = reader["DiscountAmtCurrencyCode"];
                    bookingItemCar = new BookingItemCar(reader["BookingItemID"], reader["CarVendorCode"], reader["AirportCodePickUp"], reader["AirportCodeDropOff"],
                        reader["CarVendorLocationCodePickUp"], reader["CarVendorLocationCodeDropOff"], reader["CarCategoryID"], reader["CarTypeID"],
                        reader["CarTransmissionDriveID"], reader["CarFuelAirConditionID"], reader["SearchTypeIDPickup"], reader["ResultTypeIDPickUp"],
                        reader["FlightNumberArrival"], reader["AirlineCodeArrival"], reader["CarBusinessModelID"], reader["CarItemID"],
                        reader["SupplySubsetID"], reader["CarCorpDiscountCodeCRSReq"], reader["CarCorpDiscountCodeCRSResp"], reader["CarRateCodeCRSReq"],
                        reader["CarRateCodeCRSResp"], reader["CarClubNbrCRSReq"], reader["CarClubNbrCRSResp"], reader["CarCouponCodeCRSReq"],
                        reader["CarCouponCodeCRSResp"], reader["CarSupplementalCodeCRSReq"], reader["CarSupplementalCodeCRSResp"], reader["ExchangeRateCurrencyCode"],
                        reader["ExchangeRate"], reader["EstimatedTotalTaxAndFeeAmt"], reader["EstimatedTotalTaxAndFeeAmtCurrencyCode"], reader["CarSpecialEquipmentMask"],
                        reader["CarInsuranceAndWaiverMask"], reader["FrequentFlyerPlanName"], reader["FrequentFlyerPlanNumber"], reader["CarCorpDiscountCodeTraveler"],
                        reader["CarRateCodeTraveler"], reader["CarClubNbrTraveler"], reader["CarCouponCodeTraveler"], reader["CarSupplementalCodeTraveler"],
                        reader["CarRateOptionTypeIDTraveler"], reader["ReservationGuaranteeMethodID"], reader["CreditCardTypeID"], reader["CreditCardLastFourDigitNbr"],
                        reader["UnlimitedMileageBool"], reader["MileageChargeAmt"], reader["MileageChargeAmtCurrencyCode"], reader["MileageUnitInKMBool"],
                        reader["CarVoucherNbr"], reader["CarAgreementID"], reader["CarVendorAgreementNbr"], reader["CarRentalDaysCnt"],
                        reader["PublishedPriceAmt"], reader["CarBasePricePeriodPublishedID"], reader["CurrencyCodePublishedPrice"], reader["MarginMaxAmt"],
                        reader["MarginMaxCurrencyCode"], reader["MarginMinAmt"], reader["MarginMinCurrencyCode"], reader["MarkupPct"], discountAmt, discountCurrency);
                    bookingItemCarList.Add(bookingItemCar);
                }
                conn.Close();
            }
            return bookingItemCarList;
        }

        public static List<BookingItemCarMarkup> getBookingItemCarMarkupByBookingItemID(int bookingItemID)
        {
            List<BookingItemCarMarkup> bookingItemCarMarkupList = new List<BookingItemCarMarkup>();
            BookingItemCarMarkup bookingItemCarMarkup;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from BookingItemCarMarkup where BookingItemID = {0}", bookingItemID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookingItemCarMarkup = new BookingItemCarMarkup(reader["BookingItemID"], reader["SourceCategory"], reader["SourceID"], reader["MarkupPct"],
                        reader["MarkupAmt"], reader["MarkupCurrencyCode"], reader["CreateDate"]);
                    bookingItemCarMarkupList.Add(bookingItemCarMarkup);
                }
                conn.Close();
            }
            return bookingItemCarMarkupList;
        }

        public static List<BookingItemCarMarkupExtended> getBookingItemCarMarkupExtendedByBookingItemID(int bookingItemID)
        {
            List<BookingItemCarMarkupExtended> bookingItemCarMarkupExtendedList = new List<BookingItemCarMarkupExtended>();
            BookingItemCarMarkupExtended bookingItemCarMarkupExtended;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from BookingItemCarMarkupExtended where BookingItemID = {0}", bookingItemID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookingItemCarMarkupExtended = new BookingItemCarMarkupExtended(reader["BookingItemID"], reader["SourceCategory"], reader["SourceID"], reader["AttributeName"],
                        reader["AttributeValue"], reader["CreateDate"]);
                    bookingItemCarMarkupExtendedList.Add(bookingItemCarMarkupExtended);
                }
                conn.Close();
            }
            return bookingItemCarMarkupExtendedList;
        }

        //Get CorpDisc logged BookingItemCar table according to TRL
        public static string getCorpDiscLoggedInBookingItemCar(int trl)
        {
            string corpDisc = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select CarCorpDiscountCodeCRSReq from BookingItemCar where BookingItemID in "
                    + "(select BookingItemID from BookingItem where BookingID in (select BookingID from Booking where trl = {0}))", trl);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    corpDisc = reader[0].ToString();
                }
                conn.Close();

                return corpDisc;
            }
        }

        //Get MarkupPct from table BookingItemCar according to BookingItemID
        public static decimal getMarkupPct(int bookingItemID)
        {
            decimal markupPct = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select MarkupPct from BookingItemCar where BookingItemID = {0}", bookingItemID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    markupPct = decimal.Parse(reader[0].ToString());
                }
                conn.Close();

                return markupPct;
            }
        }

        //Get voucher logged BookingItemCar table according to TRL
        public static string getVoucherLoggedInBookingItemCar(int trl)
        {
            string voucher = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select CarVoucherNbr from BookingItemCar where BookingItemID in "
                    + "(select BookingItemID from BookingItem where BookingID in (select BookingID from Booking where trl = {0}))", trl);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    voucher = reader[0].ToString();
                }
                conn.Close();

                return voucher;
            }
        }

        //Get SupplementalInfo logged BookingItemCar table according to TRL
        public static string getSupplementalInfoLoggedInBookingItemCar(int trl)
        {
            string supplementalInfo = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select CarSupplementalCodeCRSReq from BookingItemCar where BookingItemID in "
                    + "(select BookingItemID from BookingItem where BookingID in (select BookingID from Booking where trl = {0}))", trl);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    supplementalInfo = reader[0].ToString();
                }
                conn.Close();

                return supplementalInfo;
            }
        }

        //Get RateCode logged BookingItemCar table according to TRL
        public static string getRateCodeLoggedInBookingItemCar(int trl)
        {
            string rateCode = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select CarRateCodeCRSReq from BookingItemCar where BookingItemID in "
                    + "(select BookingItemID from BookingItem where BookingID in (select BookingID from Booking where trl = {0}))", trl);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    rateCode = reader[0].ToString();
                }
                conn.Close();

                return rateCode;
            }
        }

        //select PublishedPriceAmt,CurrencyCodePublishedPrice from bookingitemcar where bookingitemid 
        //    in(select bookingitemid from bookingitem where bookingid in (select bookingid from booking where trl = @trl)
        //Get PublishedPriceAmt,CurrencyCodePublishedPrice logged BookingItemCar table according to TRL
        public static DataTable getPublishedPriceAmtFromBookingItemCarTable(int trl)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select PublishedPriceAmt,CurrencyCodePublishedPrice from bookingitemcar where bookingitemid in "
                    + "(select BookingItemID from BookingItem where BookingID in (select BookingID from Booking where trl = {0}))", trl);
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();

            }

            return dt;
        }

        //Get the CancelBool from table BookingItem according to bookingID
        public static bool getCancel(int bookingID)
        {
            int countCancel0 = 0;
            int countCancel1 = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("select count(*) from BookingAmount where BookingID = {0} and CancelBool = 0", bookingID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    countCancel0 = int.Parse(reader[0].ToString());
                }
                conn.Close();
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("select count(*) from BookingAmount where BookingID = {0} and CancelBool = 1", bookingID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    countCancel1 = int.Parse(reader[0].ToString());
                }
                conn.Close();
            }
            if (0 < countCancel0 && 0 < countCancel1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        ////Get The MonetaryClassID, MonetaryCalculationSystemID, TransactionAmtPrice, CurrencyCodePrice, BookingAmountDescPrice from BookingAmount Table according to bookingID.
        //public static DataTable getMonetaryPriceInfoFromBookingAmountTable(int bookingID)
        //{
        //    DataTable dt = new DataTable();
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        SqlCommand cmd = conn.CreateCommand();
        //        conn.Open();
        //        cmd.CommandText = string.Format("select MonetaryClassID, MonetaryCalculationSystemID, TransactionAmtPrice, CurrencyCodePrice, BookingAmountDescPrice "
        //            + " from bookingamount where bookingid = {0}", bookingID);
        //        cmd.CommandTimeout = 0;

        //        SqlDataAdapter ad = new SqlDataAdapter(cmd);
        //        ad.Fill(dt);
        //        conn.Close();

        //    }
        //    return dt;
        //}

        ////Get The EstimatedTotalTaxAndFeeAmt, EstimatedTotalTaxAndFeeAmtCurrencyCode from BookingItemCar Table according to bookingID.
        //public static DataTable getTaxAndFeeInfoFromBookingItemCarTable(int bookingID)
        //{
        //    DataTable dt = new DataTable();
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        SqlCommand cmd = conn.CreateCommand();
        //        conn.Open();
        //        cmd.CommandText = string.Format("select EstimatedTotalTaxAndFeeAmt, EstimatedTotalTaxAndFeeAmtCurrencyCode from bookingitemcar where bookingitemid in "
        //            + " (select bookingitemid from bookingitem where bookingid = {0})", bookingID);
        //        cmd.CommandTimeout = 0;

        //        SqlDataAdapter ad = new SqlDataAdapter(cmd);
        //        ad.Fill(dt);
        //        conn.Close();

        //    }
        //    return dt;
        //}

        ////Get The StartDate,EndDate,BaseAmtCost,PeakSurchargeAmtCost from BookingItemCarInventory Table according to bookingID.
        //public static DataTable getDateAndCostInfoFromBookingItemCarInventoryTable(int bookingID)
        //{
        //    DataTable dt = new DataTable();
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        SqlCommand cmd = conn.CreateCommand();
        //        conn.Open();
        //        cmd.CommandText = string.Format("select StartDate,EndDate,BaseAmtCost,PeakSurchargeAmtCost from bookingitemcarinventory where bookingitemid in "
        //            + " (select bookingitemid from bookingitem where bookingid = {0}))", bookingID);
        //        cmd.CommandTimeout = 0;

        //        SqlDataAdapter ad = new SqlDataAdapter(cmd);
        //        ad.Fill(dt);
        //        conn.Close();

        //    }
        //    return dt;
        //}


        //Get The MonetaryClassID, MonetaryCalculationSystemID, TransactionAmtPrice, CurrencyCodePrice, BookingAmountDescPrice from BookingAmount Table according to TRL.
        public static DataTable getMonetaryPriceInfoFromBookingAmountTable(int trl)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select MonetaryClassID, MonetaryCalculationSystemID, TransactionAmtPrice, CurrencyCodePrice, BookingAmountDescPrice, CurrencyCodeCost, TransactionAmtCost "
                    + " from bookingamount where bookingid in (select bookingid from booking where trl = {0}) and CancelBool = 0 and (TransactionAmtPrice  > 0 or BookingAmountDescPrice like '%Peak%' )", trl);
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();

            }
            return dt;
        }

        //Get The EstimatedTotalTaxAndFeeAmt, EstimatedTotalTaxAndFeeAmtCurrencyCode from BookingItemCar Table according to TRL.
        public static DataTable getTaxAndFeeInfoFromBookingItemCarTable(int trl)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select EstimatedTotalTaxAndFeeAmt, EstimatedTotalTaxAndFeeAmtCurrencyCode, ExchangeRate from bookingitemcar where bookingitemid in "
                    + " (select bookingitemid from bookingitem where bookingid in (select bookingid from booking where trl = {0}))", trl);
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();

            }
            return dt;
        }

        public static List<Booking> getBookingByID(int bookingID)
        {
            Booking booking = new Booking();
            List<Booking> bookingList = new List<Booking>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from Booking where BookingID = {0}", bookingID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    booking = new Booking(reader["BookingID"], reader["TravelProductID"], reader["TRL"], reader["TUID"],
                        reader["TravelPackageTypeID"], reader["BookingEndDate"], reader["WizardID"], reader["MarketingProgramID"], reader["PartnerID"],
                        reader["ReferralTrackingServiceID"], reader["ReferralTrackingNbr"], reader["ABTestGroupID"], reader["ItineraryPurposeMask"], reader["GroupAccountID"],
                        reader["BookingDesc"], reader["CreateDate"], reader["CreateTUID"], reader["UpdateDate"], reader["UpdateTUID"],
                        reader["LangID"], reader["GroupAccountDepartmentID"], reader["AgentAssistedBool"], reader["AffiliateID"], reader["OperatingUnitID"]);
                    bookingList.Add(booking);
                }
                conn.Close();
            }
            return bookingList;
        }

        public static List<BookingItemCarInventory> getBookingItemCarInventoryByBookingID(int bookingID)
        {
            List<BookingItemCarInventory> bookingItemCarInventoryList = new List<BookingItemCarInventory>();
            BookingItemCarInventory bookingItemCarInventory;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from bookingitemcarinventory where bookingitemid in (select bookingitemid from bookingitem where bookingid = {0})", bookingID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookingItemCarInventory = new BookingItemCarInventory(reader["BookingItemID"], reader["BookingItemInventorySeqNbr"],
                        reader["StartDate"], reader["EndDate"], reader["CarInternalBasePricePeriodID"], reader["BaseAmtCost"],
                        reader["PeakSurchargeAmtCost"]);
                    bookingItemCarInventoryList.Add(bookingItemCarInventory);
                }
                conn.Close();
            }
            return bookingItemCarInventoryList;
        }


        #region Maserati booking logging     add by v-pxie 2012-08-29
        public static DataTable GetBookingTable(string trl, string bookingID = null)
        {
            string sqlCmd = string.Format("Select * from Booking where TRL = {0}", trl);
            if (bookingID != null) sqlCmd = string.Format("Select * from Booking where bookingID = {0}", bookingID);
            try
            {
                return GetDataTableUtil(sqlCmd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static DataTable GetBookingItemTableByTRL(string trl, string bookingID = null)
        {
            string sqlCmd = string.Format("select * from BookingItem where BookingID "
                + " = (Select BookingID from Booking where TRL = {0})", trl);

            if (bookingID != null) sqlCmd = string.Format("select * from BookingItem where BookingID "
                + " = {0}", bookingID); 
            try
            {
                return GetDataTableUtil(sqlCmd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }


        public static DataTable GetBookingItemTableByBookingID(int bookingID)
        {
            string sqlCmd = string.Format("select * from BookingItem where BookingID  = {0}", bookingID);
            try
            {
                return GetDataTableUtil(sqlCmd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static DataTable GetBookingItemCarTableByBookingID(string bookingID)
        {
            string sqlCmd = string.Format("select * from BookingItemCar where BookingItemID " +
                            "in ( Select BookingItemID from BookingItem where BookingID = {0} )", bookingID);
            try
            {
                return GetDataTableUtil(sqlCmd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static DataTable GetBookingItemCarTable(string trl, string bookingID = null)
        {
            string sqlCmd = string.Format("select * from BookingItemCar where BookingItemID " +
                            "in (Select BookingItemID from BookingItem where BookingID in " +
                            "(Select BookingID from Booking Where TRL = {0}))", trl);
            if (bookingID != null)
                sqlCmd = string.Format("select * from BookingItemCar where BookingItemID " +
                            "in (Select BookingItemID from BookingItem where BookingID = {0})", bookingID);
            try
            {
                return GetDataTableUtil(sqlCmd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static DataTable GetBookingItemCarInventory(string trl)
        {
            string sqlCmd = string.Format("select * from bookingitemcarinventory where bookingitemid in "
                + " (select bookingitemid from bookingitem where bookingid in (select bookingid from booking where trl = {0}))", trl);
            try
            {
                return GetDataTableUtil(sqlCmd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private static DataTable GetDataTableUtil(string sqlString)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = sqlString;
                cmd.CommandTimeout = 0;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        public static DataTable GetBookingAmountTableByBookingID(string bookingID)
        {
            string sqlCmd = string.Format("select * from BookingAmount where BookingId  = {0}", bookingID);
            try
            {
                return GetDataTableUtil(sqlCmd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static DataTable GetBookingAmountTable(int trl)
        {
            string sqlCmd = string.Format("select * from BookingAmount where BookingId in (select BookingId from Booking where trl = {0})", trl);
            try
            {
                return GetDataTableUtil(sqlCmd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        //Get PNR by TRL from bookingitem table
        public static string getPNRbyTRL(uint TRL)
        {
            string PNR = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select BookingRecordSystemReferenceCode from bookingitem where bookingid in (select bookingid from booking where trl = {0})", TRL);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PNR = reader[0].ToString();
                }
                conn.Close();
            }

            return PNR;
        }
        #endregion

        public static string getTRLByPNR(string pnr, string myConnectionString = null)
        {
            string trl = "";
            connectionString = (null == myConnectionString) ? connectionString : myConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("select TRL from Booking join BookingItem on Booking.BookingID = BookingItem.BookingID where BookingRecordSystemReferenceCode = '{0}'", pnr);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    trl = reader["TRL"].ToString();
                }
                conn.Close();
            }
            return trl;
        }

        public static List<string> getPNRByCarVendorCode(string CarVendorCode = "GR")
        {
            string pnr = "";
            List<string> PNRLists = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("SELECT BookingRecordSystemReferenceCode FROM BookingItem where BookingItemID in ( SELECT BookingItemID FROM BookingItemCar where CarVendorCode='{0}') and CancelDate is NULL", CarVendorCode);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    pnr = reader["BookingRecordSystemReferenceCode"].ToString();
                    PNRLists.Add(pnr);
                }
                conn.Close();
            }
            return PNRLists;
        }

        public static int GetReservationGuareenteedMethodIDByBookingID(int bookingItemID)
        {
            int reservationGuareenteedMethodID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "SELECT ReservationGuaranteeMethodID FROM BookingItemCar where bookingItemID= " + bookingItemID.ToString();
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //reservationGuareenteedMethodID = (int)reader[0];
                    reservationGuareenteedMethodID = Convert.ToInt32(reader[0].ToString());
                }
                conn.Close();
            }
            return reservationGuareenteedMethodID;

        }
    }
}