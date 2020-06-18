using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using Expedia.CarInterface.CarServiceTest.Util;
using Expedia.CarInterface.CarServiceTest.RequestSenderFacade;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class CarWorldspanSCS_DB
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["CarWorldspanSCS"].ConnectionString;

        public static String getSupplierConfiguration(uint supplyId, String settingName, string envName)
        {
            String settingValue = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                if (String.IsNullOrEmpty(envName))
                {
                    cmd.CommandText = string.Format("select SettingValue from SupplierConfiguration where SupplierID = {0} and SettingName = '{1}'", supplyId, settingName);
                }
                else
                {
                    cmd.CommandText = string.Format("select SettingValue from SupplierConfiguration where SupplierID = {0} and SettingName = '{1}' and EnvironmentName = '{2}' ",
                        supplyId, settingName, envName);
                }
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    settingValue = reader[0].ToString();
                }
                conn.Close();
            }

            return settingValue;
        }

        public static string getPoSSettingValue(string settingName, string jurisdictionCode, string companyCode, string managementUnitCode)
        {
            string settingValue = string.Empty;
            string envName = CarConfigurationManager.AppSetting("EnvironmentName");
            if (envName == null) envName = GetEnvFromAPPConfig();
            ConfigurationDBHelper configurationDBhelper = new ConfigurationDBHelper(CarCommonEnumManager.ServiceName.CarWorldspanSCS, ConfigSettingType.POS);
            // 1.Search PosConfiguration table by EnvironmentName and POS
            settingValue = configurationDBhelper.SettingValuePOSGet(settingName, envName, jurisdictionCode, companyCode, managementUnitCode);
            // 2.Search PosConfiguration table by EnvironmentName and POS as NULL
            if (string.IsNullOrEmpty(settingValue))
                settingValue = configurationDBhelper.SettingValuePOSGet(settingName, envName);
            // 3.Search PosConfiguration table by EnvironmentName as NULL and POS as NULL
            if (string.IsNullOrEmpty(settingValue))
                settingValue = configurationDBhelper.SettingValuePOSGet(settingName);
            return settingValue;
        }

        // Update the SettingName by using inputted SettingValue and return original SettingValue
        public static string updatePosSettingValue(string settingName, string settingValue, string jurisdictionCode, string companyCode, string managementUnitCode)
        {
            string origSettingValue = string.Empty;
            string envName = CarConfigurationManager.AppSetting("EnvironmentName");
            string urlName = "CarWorldspanSCSUri";
            ConfigurationDBHelper configurationDBhelper = new ConfigurationDBHelper(CarCommonEnumManager.ServiceName.CarWorldspanSCS, ConfigSettingType.POS);
            // 1.Search PosConfiguration table by EnvironmentName and POS
            origSettingValue = configurationDBhelper.SettingValuePOSGet(settingName, envName, jurisdictionCode, companyCode, managementUnitCode);
            if (!string.IsNullOrEmpty(origSettingValue))
            {
                RequestSender.SendServiceConfigHttpRequest(urlName, settingName, settingValue, jurisdictionCode, companyCode, managementUnitCode);
                return origSettingValue;
            }
            // 2.Search PosConfiguration table by EnvironmentName and POS as NULL
            origSettingValue = configurationDBhelper.SettingValuePOSGet(settingName, envName);
            if (!string.IsNullOrEmpty(origSettingValue))
            {
                RequestSender.SendServiceConfigHttpRequest(urlName, settingName, settingValue);
                return origSettingValue;
            }
            // 3.Search PosConfiguration table by EnvironmentName as NULL and POS as NULL
            origSettingValue = configurationDBhelper.SettingValuePOSGet(settingName);
            if (!string.IsNullOrEmpty(origSettingValue))
            {
                RequestSender.SendServiceConfigHttpRequest(urlName, settingName, setEnv: false);
                return origSettingValue;
            }
            else
                return origSettingValue;
        }
//        private static string connectionString = ConfigurationManager.ConnectionStrings["CarWorldspanSCS"].ConnectionString;

//        public static string GetDatabaseFromConnectionString(string connectionString)
//        {
//            string[] split = connectionString.Split(new Char[] { ';' });
//            string[] split2 = split[1].Split(new Char[] { '=' });
//            string database = split2[1];
//            return database;
//        }

//        //Get DIR.address based on POS, environment
//        public static string GetDIRConfigBasedOnPOS(String jurisdictionCode, String companyCode, String managementUnitCode, String env)
//        {
//            //select SettingValue from PoSConfiguration where JurisdictionCode = 'USA' and CompanyCode = '10111' and ManagementUnitCode = '1010'
//            //and SettingName = 'DIR.address' and EnvironmentName = 'stt03'
//            string dirAddress = "";
//            string database = GetDatabaseFromConnectionString(connectionString);
//            string jurisdictionCodeString = "";
//            if (jurisdictionCode == null)
//            {
//                jurisdictionCodeString = " is NULL";
//            }
//            else
//            {
//                jurisdictionCodeString = " = '" + jurisdictionCode + "'";
//            }
//            string companyCodeString = "";
//            if (companyCode == null)
//            {
//                companyCodeString = " is NULL";
//            }
//            else
//            {
//                companyCodeString = " = '" + companyCode + "'";
//            }
//            string managementUnitCodeString = "";
//            if (managementUnitCode == null)
//            {
//                managementUnitCodeString = " is NULL";
//            }
//            else
//            {
//                managementUnitCodeString = " = '" + managementUnitCode + "'";
//            }
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                cmd.CommandText = string.Format("select SettingValue from PoSConfiguration where JurisdictionCode {0} and CompanyCode {1} and ManagementUnitCode {2} " +
//                    " and SettingName = 'DIR.config' and EnvironmentName = '{3}'", jurisdictionCodeString, companyCodeString, managementUnitCodeString, env);
//                //cmd.CommandText = string.Format("select SettingValue from PoSConfiguration where JurisdictionCode = '{0}' and CompanyCode = '{1}' and ManagementUnitCode = '{2}' " +
//                //    " and SettingName = 'DIR.config' and EnvironmentName = '{3}'", jurisdictionCode, companyCode, managementUnitCode, env);
//                cmd.CommandTimeout = 0;

//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    dirAddress = reader["SettingValue"].ToString();
//                }
//                conn.Close();

//            }
//            return dirAddress;
//        }

//        ////Get env form configed WSCSUri
        public static string GetEnvFromAPPConfig()
        {
            //Get the WSCSURL from app settings(E.g: http://chelcarjvafc01:52048/stt03.com-expedia-s3-cars-supplyconnectivity-worldspan.urn:com:expedia:s3:cars:supplyconnectivity:interface:v4" />)
            String wscsUri = ConfigurationManager.AppSettings["CarWorldspanSCSUri"];

           //Get the environmnet form WSCS URL, E.g: stt03
           String env = wscsUri.Substring(7).Split('/')[1].Split('.')[0];

           return env;
        }

//        public static bool GetEnableTestBookingValue()
//        {
//            //Get the environmnet form WSCS URL, E.g: stt03
//            String env = ServiceConfigUtil.EnvNameGet();// GetEnvFromAPPConfig();

//            //select SettingValue from PoSConfiguration where JurisdictionCode = 'USA' and CompanyCode = '10111' and ManagementUnitCode = '1010'
//            //and SettingName = 'DIR.address' and EnvironmentName = 'stt03'
//            string enableTestBookingValue = "0";
//            string database = GetDatabaseFromConnectionString(connectionString);

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                cmd.CommandText = string.Format("select SettingValue from PoSConfiguration where JurisdictionCode is NULL and CompanyCode is NULL and ManagementUnitCode is NULL " +
//                    " and SettingName = 'DIR.EnableTestBooking' and EnvironmentName = '{0}'", env);
//                //cmd.CommandText = string.Format("select SettingValue from PoSConfiguration where JurisdictionCode = '{0}' and CompanyCode = '{1}' and ManagementUnitCode = '{2}' " +
//                //    " and SettingName = 'CarsBooking.BeanName' and EnvironmentName = '{3}'", jurisdictionCode, companyCode, managementUnitCode, env);
//                cmd.CommandTimeout = 0;

//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    enableTestBookingValue = reader["SettingValue"].ToString();
//                }
//                conn.Close();

//            }
//            if (enableTestBookingValue == "0")
//            {
//                return false;
//            }
//            return true;
//        }

//        /// <summary>
//        ///  Query ExternalSupplyServiceDomainValueMap table to get mapping info
//        /// </summary>
//        public static string getDomainValueFromDomainValueMapTbl(int domainTypeID, string externalDomainValue, string additionDomainValue = null)
//        {
//            string domainValue = "";
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                string additionValue = "";
//                if (additionDomainValue != null)
//                    additionValue = string.Format("and DomainValue = '{0}'", additionDomainValue);
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                cmd.CommandText = string.Format("select DomainValue from ExternalSupplyServiceDomainValueMap where DomainTypeID = '{0}' and ExternalDomainValue = '{1}'  {2}", domainTypeID, externalDomainValue, additionValue);
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

//        public static string getExternalDomainValueFromDomainValueMapTbl(int domainTypeID, string domainValue)
//        {
//            string externalDomainValue = "";
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                cmd.CommandText = string.Format("select ExternalDomainValue from ExternalSupplyServiceDomainValueMap where DomainTypeID = '{0}' and DomainValue = '{1}'", domainTypeID, domainValue);
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

        // Read CarShuttleCategoryCode from CarVendorLocation table 
    }
}
