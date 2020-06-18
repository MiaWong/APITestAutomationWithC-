using Expedia.CarInterface.CarServiceTest.DBOperation.DBUtil;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.CarTypes.V5;
using Expedia.CarInterface.CarServiceTest.Communication;
using System.IO;
using System.Data;
using Expedia.CarInterface.CarServiceTest.Util;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class CarBSDB
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["CarBS"].ConnectionString;

        #region service config
        //Get SerivceConfig
        public static string GetServiceConfig(string settingName, string env = null, String jurisdictionCode = null, String companyCode = null, String managementUnitCode = null)
        {
            string settingValue = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                string envString = (null != env) ? string.Format("='{0}'", env) : "is null";
                string jurisdictionCodeString = (null != jurisdictionCode) ? string.Format("='{0}'", jurisdictionCode) : "is null";
                string companyCodeString = (null != companyCode) ? string.Format("='{0}'", companyCode) : " is null";
                string managementUnitCodeString = (null != managementUnitCode) ? string.Format("='{0}'", managementUnitCode) : "is null";
                cmd.CommandText = string.Format("select SettingValue from PoSConfiguration where JurisdictionCode {0} and CompanyCode{1} and ManagementUnitCode {2}"
                    + " and SettingName = '{3}' and EnvironmentName {4}", jurisdictionCodeString, companyCodeString, managementUnitCodeString, settingName, envString);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    settingValue = reader["SettingValue"].ToString();
                }
                conn.Close();

            }
            return settingValue;
        }

        //Get ServiceConfig according the env and pos, if there is not record, then query it according env = null, or pos = null
        public static string GetServiceConfigIncludeNullPOSAndEnv(string settingName, string env, string jurisdictionCode , string companyCode, string managementUnitCode)
        {
            string settingValue = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                string envString = (null != env) ? string.Format("='{0}'", env) : "is null";
                string jurisdictionCodeString = (null != jurisdictionCode) ? string.Format("='{0}'", jurisdictionCode) : "is null";
                string companyCodeString = (null != companyCode) ? string.Format("='{0}'", companyCode) : " is null";
                string managementUnitCodeString = (null != managementUnitCode) ? string.Format("='{0}'", managementUnitCode) : "is null";
                cmd.CommandText = string.Format("select top 1 SettingValue, EnvironmentName, JurisdictionCode, CompanyCode, ManagementUnitCode from PoSConfiguration " +
                    " where (EnvironmentName is null or EnvironmentName {0}) " + 
                    " and (( JurisdictionCode is null and CompanyCode is null and ManagementUnitCode is null) " + 
                    " or (JurisdictionCode {1} and CompanyCode {2} and ManagementUnitCode {3})) " +
                    " and SettingName = '{4}'  order by EnvironmentName desc", envString, jurisdictionCodeString, companyCodeString, managementUnitCodeString, settingName);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                //int flags;
                while (reader.Read())
                {
                    settingValue = reader["SettingValue"].ToString();
                }

                conn.Close();
            }
            return settingValue;
        }

        #endregion

        #region Client Configuration
        public static string GetClientConfig(string settingName, string env = null, string clientID = null)
        {
            string settingValue = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                string envString = (null != env) ? string.Format("='{0}'", env) : "is null";
                string clientIDString = (null != clientID) ? string.Format("='{0}'", clientID) : "is null";
                cmd.CommandText = string.Format("select SettingValue from ClientConfiguration where SettingName = '{0}' and EnvironmentName {1} and ClientID {2}", settingName, envString, clientIDString);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    settingValue = reader["SettingValue"].ToString();
                }
                conn.Close();

            }
            return settingValue;
        }

        public static string GetClientCodeForSpecificClientID(string clientID)
        {
            string clientCode = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                string clientIDString = (null != clientID) ? string.Format("='{0}'", clientID) : "is null";
                cmd.CommandText = string.Format("select ClientCode from Client where ClientID {0}", clientIDString);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    clientCode = reader["ClientCode"].ToString().Trim();
                }
                conn.Close();
            }

            return clientCode;
        }

        public static string GetClientConfigIncludeNullPoSAndEnv(string settingName, string env = null, string clientID = null)
        {
            string settingValue = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                string envString = (null != env) ? string.Format("(EnvironmentName='{0}' or EnvironmentName is null) ", env) : " EnvironmentName is null";
                string clientIDString = (null != clientID) ? string.Format("(ClientID='{0}' or ClientID is null)", clientID) : " ClientID is null";
                cmd.CommandText = string.Format("select top 1 SettingValue from ClientConfiguration where SettingName = '{0}' and  {1} and  {2}  order by EnvironmentName desc", settingName, envString, clientIDString);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    settingValue = reader["SettingValue"].ToString();
                }
                conn.Close();

            }
            return settingValue;
        }

        
        #endregion

        #region enhanced booking logging
        public static CarReservationData getCarReservationData(int bookingItemID)
        {
            CarReservationData carReservationData = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from CarReservationData where BookingItemID = {0}", bookingItemID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carReservationData = new CarReservationData(reader["JurisdictionCode"], reader["CompanyCode"], reader["ManagementUnitCode"], reader["BookingItemID"],
                        reader["CarReservationNodeData"], reader["CarReservationNodeMajorVersion"], reader["CarReservationNodeMinorVersion"], 
                        reader["CarReservationDataExtendedElementCnt"], reader["CarReservationDataExtendedPriceListCnt"],
                        reader["UseDateEnd"], reader["CreateDate"]);
                }
                conn.Close();
            }
            return carReservationData;
        }

        public static List<CarReservationDataExtended> getCarReservationDataExtended(int bookingItemID)
        {
            List<CarReservationDataExtended> carReserveDataExtList = new List<CarReservationDataExtended>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from CarReservationDataExtended where BookingItemID = {0}", bookingItemID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carReserveDataExtList.Add(new CarReservationDataExtended(reader["JurisdictionCode"], reader["CompanyCode"], reader["ManagementUnitCode"], reader["BookingItemID"],
                        reader["SeqNbr"], reader["MonetaryClassID"], reader["MonetaryCalculationSystemID"], reader["MonetaryCalculationID"],
                        reader["CurrencyCodeCost"], reader["TransactionAmtCost"], reader["DescCost"], reader["FinanceCategoryCodeCost"], reader["FinanceApplicationCodeCost"], reader["FinanceApplicationUnitCntCost"],
                        reader["CurrencyCodePrice"], reader["TransactionAmtPrice"], reader["DescPrice"], reader["FinanceCategoryCodePrice"],
                        reader["FinanceApplicationCodePrice"], reader["FinanceApplicationUnitCntPrice"], reader["CreateDate"], reader["MultiplierPctCost"], reader["MultiplierPctPrice"]));
                }
                conn.Close();
            }
            return carReserveDataExtList;
        }

        /// <summary>
        /// Removed from CarReservationData with BookingItemID
        /// </summary>
        /// <param name="bookingItemID"></param>
        /// <returns></returns>
        public static void RemovedEBLRecordByBooingItemID(string bookingItemID)
        {
            try
            {
                string cmdString = string.Format("delete from CarReservationData where BookingItemID = {0}", bookingItemID);
                string delectFailedString = "Delete from CarReservationData table failed,please check the DB";

                Console.WriteLine("Begin delete the data from CarReservationData*********************");
                BasicRemovedFunction(cmdString, delectFailedString);
                Console.WriteLine("End delete the data from CarReservationData*********************");
            }
            catch (Exception e)
            {
                Console.WriteLine("***********************************************************************");
                Console.WriteLine("occured an exception when delete CarReservationData table :" + e.Message);
                Console.WriteLine("***********************************************************************");
                throw ;
            }
        }

        /// <summary>
        /// Removed all of the Pricelist record in table CarReservationDataExtend with BookingItemID
        /// </summary>
        /// <param name="bookingItemID"></param>
        public static void RemovedEBLAllPriceList(string bookingItemID)
        {
            try
            {
                string cmdString = string.Format("delete from CarReservationDataExtended where BookingItemID = {0} and DescPrice is not null", 
                    bookingItemID);
                string delectFailedString = "Delete from CarReservationDataExtended table failed,please check the DB";

                Console.WriteLine("" + cmdString);
                Console.WriteLine("Begin delete the data from CarReservationDataExtended*********************");
                BasicRemovedFunction(cmdString, delectFailedString);
                Console.WriteLine("End delete the data from CarReservationDataExtended*********************");
            }
            catch (Exception e)
            {

                Console.WriteLine("***********************************************************************");
                Console.WriteLine("occured an exception when delete CarReservationDataExtended table :" + e.Message);
                Console.WriteLine("***********************************************************************");
                throw;
            }
        }

        /// <summary>
        /// Removed only total price recoed in table CarReservationDataExtend with BookingItemID
        /// </summary>
        /// <param name="bookingItemID"></param>
        public static void RemovedEBLTotalPriceInPriceList(string bookingItemID)
        {
            try
            {
                string cmdString = string.Format("delete from CarReservationDataExtended where BookingItemID = {0} and DescPrice = 'Total'", 
                    bookingItemID);
                string delectFailedString = "Delete data from CarReservationDataExtended table failed,please check the DB";
                //delete
                Console.WriteLine("Begin delete the data from CarReservationDataExtended*********************");
                BasicRemovedFunction(cmdString, delectFailedString);
                Console.WriteLine("End delete the data from CarReservationDataExtended*********************");
            }
            catch (Exception e)
            {
                Console.WriteLine("***********************************************************************");
                Console.WriteLine("occured an exception when delete CarReservationDataExtended table :" + e.Message);
                Console.WriteLine("***********************************************************************");
                throw;
            }
        }

        /// <summary>
        /// basic method for delete 
        /// </summary>
        /// <param name="cmdString"></param>
        /// <param name="exceptionString"></param>
        public static void BasicRemovedFunction(string cmdString,string exceptionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = cmdString;
                cmd.CommandTimeout = 0;
                int result = cmd.ExecuteNonQuery();
                if (!(result > 0))
                {
                    Console.WriteLine(exceptionString);
                    throw new Exception(exceptionString);
                }
                conn.Close();
            }
        }

        public static void RemovedAndUpdateEBLForPoliceList(string bookingItemID)
        {
           

            try
            {
                // Get the carReservation from EBL table CarReservationData
                CarReservationData carReservationData = CarBSDB.getCarReservationData(int.Parse(bookingItemID));
                CarReservation expectedCarReservation = (CarReservation)FastInfoSetSerializer.DeserializeFIInDB((byte[])carReservationData.CarReservationNodeData, typeof(CarReservation));

                // removed police list 
                if (expectedCarReservation.CarPolicyList.CarPolicy.Count == 0)
                {
                    Console.WriteLine("No car policy list exist in CarReservation , don't need remove it.");
                }
                else
                    expectedCarReservation.CarPolicyList = null;

                Console.WriteLine("Begin update the DB for CarReservationData ...");
                SqlConnection conn = new SqlConnection(connectionString);
                DataSet dsSet = new DataSet();
                SqlDataAdapter sdaAdapter = null;
                SqlCommandBuilder scbBuilder = null;

                //SqlCommand cmdUpdate = null;
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from CarReservationData where BookingItemID = {0}", bookingItemID);
                sdaAdapter = new SqlDataAdapter(cmd);
                scbBuilder = new SqlCommandBuilder(sdaAdapter);
                sdaAdapter.Fill(dsSet, "CarReservationData");
                //update the table value with the byte 
                //byte [] reservationDode = FastInfoSetSerializer.ObjectToByteArray(expectedCarReservation);

                MemoryStream xmlMemory= XMLSerializer.Serialize(expectedCarReservation, typeof(CarReservation));
                MemoryStream FI_steam = FastInfoSetSerializer.Serialize(xmlMemory);
                dsSet.Tables["CarReservationData"].Rows[0]["CarReservationNodeData"] = FI_steam.ToArray();
                //Post the data modification to the database.
                sdaAdapter.Update(dsSet, "CarReservationData");

                Console.WriteLine("CarReservationData updated successfully");
                conn.Close();
            }
            catch (Exception e)
            {

                Console.WriteLine("***********************************************************************");
                Console.WriteLine("occured an exception when update CarReservationData table :"+e.Message);
                Console.WriteLine("***********************************************************************");
                throw;
            }

            // Get the carReservation from EBL table CarReservationData
            CarReservationData carReservationData2 = CarBSDB.getCarReservationData(int.Parse(bookingItemID));
            CarReservation expectedCarReservation2 = (CarReservation)FastInfoSetSerializer.DeserializeFIInDB((byte[])carReservationData2.CarReservationNodeData, typeof(CarReservation));
              
        }

        public static void RemovedAndUpdateEBLForCarCatalogMakeModel(string bookingItemID)
        {
            // Get the carReservation from EBL table CarReservationData
            CarReservationData carReservationData = CarBSDB.getCarReservationData(int.Parse(bookingItemID));
            CarReservation expectedCarReservation = (CarReservation)FastInfoSetSerializer.DeserializeFIInDB((byte[])carReservationData.CarReservationNodeData, typeof(CarReservation));
            Console.WriteLine("CarReservation before Update........:");
            Print.PrintMessageToConsole(expectedCarReservation);
            // removed police list 
            expectedCarReservation.CarProduct.CarCatalogMakeModel.CarMakeString = "Test for Catalogmakemodel";

            try
            {
                Console.WriteLine("Begin update the DB for CarReservationData ...");
                SqlConnection conn = new SqlConnection(connectionString);
                DataSet dsSet = new DataSet();
                SqlDataAdapter sdaAdapter = null;
                SqlCommandBuilder scbBuilder = null;

                //SqlCommand cmdUpdate = null;
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from CarReservationData where BookingItemID = {0}", bookingItemID);
                sdaAdapter = new SqlDataAdapter(cmd);
                scbBuilder = new SqlCommandBuilder(sdaAdapter);
                sdaAdapter.Fill(dsSet, "CarReservationData");
                //update the table value with the byte 
                //byte [] reservationDode = FastInfoSetSerializer.ObjectToByteArray(expectedCarReservation);

                MemoryStream xmlMemory = XMLSerializer.Serialize(expectedCarReservation, typeof(CarReservation));
                MemoryStream FI_steam = FastInfoSetSerializer.Serialize(xmlMemory);
                dsSet.Tables["CarReservationData"].Rows[0]["CarReservationNodeData"] = FI_steam.ToArray();
                //Post the data modification to the database.
                sdaAdapter.Update(dsSet, "CarReservationData");

                Console.WriteLine("CarReservationData updated successfully");
                conn.Close();
            }
            catch (Exception e)
            {

                Console.WriteLine("***********************************************************************");
                Console.WriteLine("occured an exception when update CarReservationData table :" + e.Message);
                Console.WriteLine("***********************************************************************");
                throw;
            }

            // Get the carReservation from EBL table CarReservationData
            CarReservationData carReservationData2 = CarBSDB.getCarReservationData(int.Parse(bookingItemID));
            CarReservation expectedCarReservation2 = (CarReservation)FastInfoSetSerializer.DeserializeFIInDB((byte[])carReservationData2.CarReservationNodeData, typeof(CarReservation));

        }


        public static DataTable GetDataTable_CarReservationData(int bookingItemID)
        {
            string sqlCmd = string.Format("select * from CarReservationData where BookingItemID = {0}", bookingItemID);
            return GetDataTableUtil(sqlCmd);
        }

        public static DataTable GetDataTable_CarReservationDataExtended(int bookingItemID)
        {
            string sqlCmd = string.Format("select * from CarReservationDataExtended where BookingItemID = {0}", bookingItemID);
            return GetDataTableUtil(sqlCmd);
        }

        public static DataTable GetDataTable_CarReservationDataExtended_TotalPrice(int bookingItemID)
        {
            string sqlCmd = string.Format("select * from CarReservationDataExtended where BookingItemID = {0} "
                                +" where FinanceApplicationCodePrice = 'Total'", bookingItemID);
            return GetDataTableUtil(sqlCmd);
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
        #endregion

        #region
        public static string getClintCode(string clientID)
        {
            string clientCode = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select clientCode from client where clientID = {0}", clientID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    clientCode = reader[0].ToString();
                }
                conn.Close();
            }

            return clientCode;

        }

        public static string getClintID(string clientCode)
        {
            string clientID = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select clientID from client where clientCode = '{0}'", clientCode);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    clientID = reader[0].ToString();
                }
                conn.Close();
            }

            return clientID;

        }
        #endregion

        public static string getTransactionStateCode(string bookingRecordLocatorID)
        {
            string clientCode = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select transactionStateCode from BookingRecordLocator where BookingRecordLocatorID = {0}", bookingRecordLocatorID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    clientCode = reader[0].ToString();
                }
                conn.Close();
            }

            return clientCode;

        }

        public static bool isBRLExistingInBookingRecordLocatorGDSCancelFailed(string bookingRecordLocatorID)
        {
            bool isExisting = false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select BookingRecordLocatorID from BookingRecordLocatorGDSCancelFailed where BookingRecordLocatorID = {0}", bookingRecordLocatorID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    isExisting = true;
                }
                conn.Close();
            }

            return isExisting;

        }
                       
        public static string GetSupplierIDFromPosSupplyAttributes(string jurisdictionCode
            , string companyCode
            , string managementUnitCode
            , string pickupCountryCode
            , string supplierIDIn
            , out string billingNumber)
        {
            string supplierID = string.Empty;
            billingNumber = string.Empty;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select SupplierID, BillingNumber from PosSupplyAttributes where JurisdictionCode = '{0}' and CompanyCode = '{1}' and ManagementUnitCode='{2}' and PickupCountryCode='{3}' {4} "
                    , jurisdictionCode
                    , companyCode
                    , managementUnitCode
                    , pickupCountryCode
                    , (string.IsNullOrEmpty(supplierIDIn) ? "" : " and SupplierID = " + supplierIDIn));
                Console.WriteLine("-- Start : GetSupplierIDFromPosSupplyAttributes --");
                Console.WriteLine(cmd.CommandText);
                Console.WriteLine("-- End : GetSupplierIDFromPosSupplyAttributes --");
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    supplierID = reader[0].ToString();
                    billingNumber = reader[1].ToString();
                }
                conn.Close();
            }

            return supplierID;

        }

        public static void GetToXPathFromErrorXPathMapping(string messageNameFrom, string fromXPath
          , out string messageNameTo, out string toXPath, string messageNameToFuzzy = "")
        {
            messageNameTo = string.Empty;
            toXPath = string.Empty;
            if (messageNameToFuzzy.Length >0)
                messageNameToFuzzy = "and MessageNameTo like '%" + messageNameToFuzzy + "%'";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                // XPathFrom like '%{1}' for {$xpathBase} query
                cmd.CommandText = string.Format("select * from ErrorXPathMapping where MessageNameFrom = '{0}' and XPathFromRegex like '%{1}'  {2}"
                   , messageNameFrom, fromXPath.Replace("'", "''").Replace("[", "[[]"), messageNameToFuzzy);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    messageNameTo = reader["MessageNameTo"].ToString();
                    toXPath = reader["XPathToRegex"].ToString();
                }
                conn.Close();
            }
        }

        #region POS Configuration
        public static string GetPOSConfig(string settingName, string env = null, string POSCode = null, string CompanyCode = null, string ManagementUnitCode = null)
        {
            string settingValue = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                string envString = (null != env) ? string.Format("='{0}'", env) : "is null";
                string POSCodeString = (null != POSCode) ? string.Format("='{0}'", POSCode) : "is null";
                string CompanyCodeString = (null != CompanyCode) ? string.Format("='{0}'", CompanyCode) : "is null";
                string ManagementUnitCodeString = (null != ManagementUnitCode) ? string.Format("='{0}'", ManagementUnitCode) : "is null";

                cmd.CommandText = string.Format("select SettingValue from PoSConfiguration where SettingName = '{0}' and EnvironmentName {1} and JurisdictionCode {2} and CompanyCode {3} and ManagementUnitCode {4}", settingName, envString, POSCodeString, CompanyCodeString, ManagementUnitCodeString);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    settingValue = reader["SettingValue"].ToString();
                }
                conn.Close();
            }
            return settingValue;
        }
        #endregion
    }
}
