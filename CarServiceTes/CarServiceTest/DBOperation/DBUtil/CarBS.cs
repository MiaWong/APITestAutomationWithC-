using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.PlaceTypes.V4;
using Expedia.CarInterface.CarServiceTest.Util;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class CarBS_DB
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["CarBS"].ConnectionString;

        public static string GetDatabaseFromConnectionString(string connectionString)
        {
            string[] split = connectionString.Split(new Char[] { ';' });
            string[] split2 = split[1].Split(new Char[] { '=' });
            string database = split2[1];
            return database;
        }

        //Get CarsBooking config setting based on POS
        public static string GetCarsBookingBeanBasedOnPOS(String jurisdictionCode, String companyCode, String managementUnitCode)
        {
            //Get the environmnet form WSCS URL, E.g: stt03
            String env = ServiceConfigUtil.EnvNameGet();// GetEnvFromAPPConfig();

            //select SettingValue from PoSConfiguration where JurisdictionCode = 'USA' and CompanyCode = '10111' and ManagementUnitCode = '1010'
            //and SettingName = 'DIR.address' and EnvironmentName = 'stt03'
            string carsBookingBean = "";
            string database = GetDatabaseFromConnectionString(connectionString);
            string jurisdictionCodeString;
            if (jurisdictionCode == null)
            {
                jurisdictionCodeString = " is NULL";
            }
            else
            {
                jurisdictionCodeString = " = '" + jurisdictionCode + "'";
            }
            string companyCodeString = "";
            if (companyCode == null)
            {
                companyCodeString = " is NULL";
            }
            else
            {
                companyCodeString = " = '" + companyCode + "'";
            }
            string managementUnitCodeString = "";
            if (managementUnitCode == null)
            {
                managementUnitCodeString = " is NULL";
            }
            else
            {
                managementUnitCodeString = " = '" + managementUnitCode + "'";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select SettingValue from PoSConfiguration where JurisdictionCode {0} and CompanyCode {1} and ManagementUnitCode {2} " +
                    " and SettingName = 'CarsBooking.BeanName' and EnvironmentName = '{3}'", jurisdictionCodeString, companyCodeString, managementUnitCodeString, env);
                //cmd.CommandText = string.Format("select SettingValue from PoSConfiguration where JurisdictionCode = '{0}' and CompanyCode = '{1}' and ManagementUnitCode = '{2}' " +
                //    " and SettingName = 'CarsBooking.BeanName' and EnvironmentName = '{3}'", jurisdictionCode, companyCode, managementUnitCode, env);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carsBookingBean = reader["SettingValue"].ToString();
                }
                conn.Close();

            }
            return carsBookingBean;
        }

        //Get CarsBooking config setting based on POS
        public static string GetTravServerBeanBeanBasedOnPOS(String jurisdictionCode, String companyCode, String managementUnitCode)
        {
            //Get the environmnet form WSCS URL, E.g: stt03
            String env = ServiceConfigUtil.EnvNameGet();//GetEnvFromAPPConfig();

            //select SettingValue from PoSConfiguration where JurisdictionCode = 'USA' and CompanyCode = '10111' and ManagementUnitCode = '1010'
            //and SettingName = 'DIR.address' and EnvironmentName = 'stt03'
            string travServerBean = "";
            string database = GetDatabaseFromConnectionString(connectionString);
            string jurisdictionCodeString = "";
            if (jurisdictionCode == null)
            {
                jurisdictionCodeString = " is NULL";
            }
            else
            {
                jurisdictionCodeString = " = '" + jurisdictionCode + "'";
            }
            string companyCodeString = "";
            if (companyCode == null)
            {
                companyCodeString = " is NULL";
            }
            else
            {
                companyCodeString = " = '" + companyCode + "'";
            }
            string managementUnitCodeString = "";
            if (managementUnitCode == null)
            {
                managementUnitCodeString = " is NULL";
            }
            else
            {
                managementUnitCodeString = " = '" + managementUnitCode + "'";
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select SettingValue from PoSConfiguration where JurisdictionCode {0} and CompanyCode {1} and ManagementUnitCode {2} " +
                    " and SettingName = 'TravServer.BeanName' and EnvironmentName = '{3}'", jurisdictionCodeString, companyCodeString, managementUnitCodeString, env);
                //cmd.CommandText = string.Format("select SettingValue from PoSConfiguration where JurisdictionCode = '{0}' and CompanyCode = '{1}' and ManagementUnitCode = '{2}' " +
                //    " and SettingName = 'TravServer.BeanName' and EnvironmentName = '{3}'", jurisdictionCode, companyCode, managementUnitCode, env);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    travServerBean = reader["SettingValue"].ToString();
                }
                conn.Close();

            }
            return travServerBean;
        }

        //Get config/flushHostList based on env
        public static string GetFlushHostList()
        {
            //Get the environmnet form WSCS URL, E.g: stt03
            String env = ServiceConfigUtil.EnvNameGet();//GetEnvFromAPPConfig();

            //select SettingValue from PoSConfiguration where JurisdictionCode = 'USA' and CompanyCode = '10111' and ManagementUnitCode = '1010'
            //and SettingName = 'Search.maxLatLongLocationCount' and EnvironmentName = 'stt03'
            string flushHoteList = "";
            string database = GetDatabaseFromConnectionString(connectionString);
            string jurisdictionCodeString = " is NULL";
            string companyCodeString = " is NULL";
            string managementUnitCodeString = " is NULL";
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select SettingValue from PoSConfiguration where JurisdictionCode {0} and CompanyCode {1} and ManagementUnitCode {2} " +
                    " and SettingName = 'config/flushHostList' and EnvironmentName = '{3}'", jurisdictionCodeString, companyCodeString, managementUnitCodeString, env);
                //cmd.CommandText = string.Format("select SettingValue from PoSConfiguration where JurisdictionCode = '{0}' and CompanyCode = '{1}' and ManagementUnitCode = '{2}' " +
                //    " and SettingName = 'TravServer.BeanName' and EnvironmentName = '{3}'", jurisdictionCode, companyCode, managementUnitCode, env);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    flushHoteList = reader["SettingValue"].ToString();
                }
                conn.Close();

            }
            return flushHoteList;
        }

        ////Get env form configed WSCSUri
        //public static string GetEnvFromAPPConfig()
        //{
        //    //Get the WSCSURL from app settings(E.g: http://chelcarjvafc01:52048/stt03.com-expedia-s3-cars-supplyconnectivity-worldspan.urn:com:expedia:s3:cars:supplyconnectivity:interface:v4" />)
        //    String wscsUri = ConfigurationManager.AppSettings["CarWorldspanSCSUri"];

        //    //Get the environmnet form WSCS URL, E.g: stt03
        //    String env = wscsUri.Substring(7).Split('/')[1].Split('.')[0];

        //    return env;
        //}

        public static int GetCountByBookingRecordLocatorID(long BookingRecorLocatorID)
        {
            int count = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "select COUNT(*) from BookingRecordLocator where BookingRecordLocatorID = " + BookingRecorLocatorID.ToString();

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    count = Convert.ToInt32(reader[0].ToString());
                }
                conn.Close();

            }
            return count;
        }

        public static string GetCurrecyCodeByPickupLocation(string pickupLocationCode)
        {
            string correncyCode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "select CurrencyCodeDefault from Country where CountryCode = " +
                    "(select CountryCode from Airport where AirportCode = '" + pickupLocationCode + "')";

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    correncyCode = reader[0].ToString();
                }
                conn.Close();

            }
            return correncyCode;
        }



        public static void updateBookingRecordLocatorStateCode(uint recordLocator, string stateCode)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("update BookingRecordLocator set TransactionStateCode='{0}' where BookingRecordLocatorID={1}", stateCode, recordLocator);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                conn.Close();

            }
        }

        public static string GetAugmentReservetionWithDetails()
        {
            String env = ServiceConfigUtil.EnvNameGet();//GetEnvFromAPPConfig();
            string value = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "select SettingValue from PoSConfiguration where SettingName = 'Booking.augmentReservationWithDetails/enable' and EnvironmentName = '"
                    + env + "'";

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    value = reader[0].ToString();
                }
                conn.Close();
            }
            if (value == null)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();
                    cmd.CommandText = "select SettingValue from PoSConfiguration where SettingName = 'Booking.augmentReservationWithDetails/enable' and EnvironmentName is NULL";

                    cmd.CommandTimeout = 0;

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        value = reader[0].ToString();
                    }
                    conn.Close();
                }
            }
            if (value == null)
            {
                value = "0";
            }
            return value;
        }

        public static string GetmergeDetailAmountsInResponse()
        {
            String env = ServiceConfigUtil.EnvNameGet();//GetEnvFromAPPConfig();
            string value = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "select SettingValue from PoSConfiguration where SettingName = 'Booking.augmentReservationWithDetails/enable/mergeDetailAmountsInResponse' and EnvironmentName = '"
                    + env + "'";

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    value = reader[0].ToString();
                }
                conn.Close();
            }
            if (value == null)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();
                    cmd.CommandText = "select SettingValue from PoSConfiguration where SettingName = 'Booking.augmentReservationWithDetails/enable/mergeDetailAmountsInResponse' and EnvironmentName is NULL";

                    cmd.CommandTimeout = 0;

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        value = reader[0].ToString();
                    }
                    conn.Close();
                }
            }
            if (value == null)
            {
                value = "0";
            }
            return value;
        }

        public static string GetfailBookingOnMerchantDetailsFailure()
        {
            String env = ServiceConfigUtil.EnvNameGet();//GetEnvFromAPPConfig();
            string value = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "select SettingValue from PoSConfiguration where SettingName = 'Booking.augmentReservationWithDetails/enable/failBookingOnMerchantDetailsFailure' and EnvironmentName = '"
                    + env + "'";

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    value = reader[0].ToString();
                }
                conn.Close();
            }
            if (value == null)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();
                    cmd.CommandText = "select SettingValue from PoSConfiguration where SettingName = 'Booking.augmentReservationWithDetails/enable/failBookingOnMerchantDetailsFailure' and EnvironmentName is NULL";

                    cmd.CommandTimeout = 0;

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        value = reader[0].ToString();
                    }
                    conn.Close();
                }
            }
            if (value == null)
            {
                value = "0";
            }
            return value;
        }

        public static String BuildUpdateURL(String settingName, String settingValue)
        {
            //Get the WSCSURL from app settings(E.g: http://chelcarjvafc01:52048/stt03.com-expedia-s3-cars-supplyconnectivity-worldspan.urn:com:expedia:s3:cars:supplyconnectivity:interface:v4" />)
            String uri = CarConfigurationManager.AppSetting("CarBSUri"); ;

            //Get the environmnet form WSCS URL, E.g: stt03
            String env = ServiceConfigUtil.EnvNameGet();//GetEnvFromAPPConfig();

            uri = "http://" + uri.Substring(7).Split('/')[0] + "/config/set?&environment=" + env;

            uri = uri + "&settingName=" + settingName + "&value=" + settingValue + "&updatedBy=CarBSServiceAutomationTest&flushAll=1";

            ////Combine the URL for service setting config
            //uri = "http://" + uri.Substring(7).Split('/')[0] + "/config/set?&environment=" + env + "&companyCode=" + companyCode + "&managementUnitCode="
            //    + managementUnitCode + "&jurisdictionCode=" + jurisdictionCode + "&settingName=" + settingName + "&value=" + settingValue
            //    + "&updatedBy=CarBSServiceAutomationTest&flushAll=1";

            return uri;
        }
    }
}
