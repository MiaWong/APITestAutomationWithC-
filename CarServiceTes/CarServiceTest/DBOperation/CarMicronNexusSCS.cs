//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Configuration;
//using System.Data.SqlClient;


//namespace Expedia.CarInterface.CarServiceTest.DBOperation
//{
//    public class CarMicronNexusSCS
//    {
//        private static string connectionString = ConfigurationManager.ConnectionStrings["CarMicronNexusSCS"].ConnectionString;

//        public static String GetVendorCodeBySupplierID(uint supplierid)
//        {

//            String vendorCode = "";

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                cmd.CommandText = string.Format("SELECT ExternalDomainValue AS VendorCode FROM externalsupplyservicedomainvaluemap WHERE domainvalue='{0}'", supplierid);
//                cmd.CommandTimeout = 0;

//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    vendorCode = reader[0].ToString();
//                }
//                conn.Close();
//            }
//            return vendorCode;


//        }

//        //Get SerivceConfig
//        public static string GetServiceConfig(string settingName, string env = null, String jurisdictionCode = null, String companyCode = null, String managementUnitCode = null)
//        {
//            string settingValue = null;

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                string envString = (null != env) ? string.Format("='{0}'", env) : "is null";
//                string jurisdictionCodeString = (null != jurisdictionCode) ? string.Format("='{0}'", jurisdictionCode) : "is null";
//                string companyCodeString = (null != companyCode) ? string.Format("='{0}'", companyCode) : " is null";
//                string managementUnitCodeString = (null != managementUnitCode) ? string.Format("='{0}'", managementUnitCode) : "is null";
//                cmd.CommandText = string.Format("select SettingValue from PoSConfiguration where JurisdictionCode {0} and CompanyCode{1} and ManagementUnitCode {2}"
//                    + " and SettingName = '{3}' and EnvironmentName {4}", jurisdictionCodeString, companyCodeString, managementUnitCodeString, settingName, envString);

//                cmd.CommandTimeout = 0;

//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    settingValue = reader["SettingValue"].ToString();
//                }
//                conn.Close();

//            }
//            return settingValue;
//        }

//        //Get the value of Cost.TransactionFeesPercent from CarMicronNexusSCS_STT04..PoSConfiguration
//        public static int GetTransactionFeesPercentByEnvName(string environmentName, int supplierID)
//        {
//            string transactionFeeValue = null;
//            string tempValue = null;           

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();

//                cmd.CommandText = string.Format(@"select SettingValue,EnvironmentName  from SupplierConfiguration 
//                where SettingName = 'Cost.TransactionFeesPercent' and (EnvironmentName is null or EnvironmentName = '{0}'
//                and (SupplierID is null or SupplierID = {1}))", environmentName, supplierID);

//                cmd.CommandTimeout = 0;

//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    if (reader["EnvironmentName"].ToString() == environmentName)
//                    {
//                        transactionFeeValue = reader["SettingValue"].ToString();
//                        break;
//                    }
//                    else
//                    {
//                        tempValue = reader["SettingValue"].ToString();
//                    }
//                }
//                conn.Close();                
//            }
//            if (transactionFeeValue == null)
//            {
//                transactionFeeValue = tempValue;
//            }
//            return Convert.ToInt16(transactionFeeValue);
//        }

//        public static int GetTransactionFeesPercent(string environmentName, int supplierID)
//        {
//            string transactionFeeValue = "0";

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();

//                cmd.CommandText = string.Format("select SettingValue,EnvironmentName  from SupplierConfiguration " +
//                   " where SettingName = 'Cost.TransactionFeesPercent' and (EnvironmentName is null or EnvironmentName = '{0}') and SupplierID = {1}", environmentName, supplierID);

//                cmd.CommandTimeout = 0;

//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    if (reader["EnvironmentName"].ToString() == environmentName)
//                    {
//                        transactionFeeValue = reader["SettingValue"].ToString();
//                        break;
//                    }
//                    else if(null == reader["EnvironmentName"])
//                    {
//                        transactionFeeValue = reader["SettingValue"].ToString();
//                    }

//                }
//                conn.Close();
//            }

//            return Convert.ToInt16(transactionFeeValue);
//        }

//        /// <summary>
//        ///  Query ExternalSupplyServiceDomainValueMap table to get mapping info
//        /// </summary>
//        public static string getDomainValueFromDomainValueMapTbl(string domainType, string externalDomainValue, string additionDomainValue = null)
//        {
//            string domainValue = "";
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                string additionValue = "";
//                if (additionDomainValue != null)
//                    additionValue = string.Format("and DomainValue = '{0}'", additionDomainValue);
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                cmd.CommandText = string.Format("select DomainValue from ExternalSupplyServiceDomainValueMap where DomainType = '{0}' and ExternalDomainValue = '{1}'  {2}", domainType, externalDomainValue, additionValue);
//                cmd.CommandTimeout = 0;
//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    domainValue += reader["DomainValue"].ToString();
//                }
//                conn.Close();
//            }
//            return domainValue;
//        }
//        public static string getExternalDomainValueFromDomainValueMapTbl(string domainType, string domainValue)
//        {
//            string externalDomainValue = "";
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                cmd.CommandText = string.Format("select ExternalDomainValue from ExternalSupplyServiceDomainValueMap where DomainType = '{0}' and DomainValue = '{1}'", domainType, domainValue);
//                cmd.CommandTimeout = 0;
//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    if (externalDomainValue == "") externalDomainValue = reader["ExternalDomainValue"].ToString();
//                    else externalDomainValue = externalDomainValue + "," + reader["ExternalDomainValue"].ToString();
//                }
//                conn.Close();
//            }
//            return externalDomainValue;
//        }

//    }
//}
