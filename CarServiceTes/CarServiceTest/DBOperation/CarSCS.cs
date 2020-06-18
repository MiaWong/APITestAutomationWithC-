using System;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonEnum = Expedia.CarInterface.CarServiceTest.Util.CarCommonEnumManager;
using Expedia.CarInterface.CarServiceTest.Util;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{    
    public partial class CarSCSCommonHelper
    {
        /// <summary>
        /// get configuration column name for configuration tables
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="isLogGet">is get from log table</param>
        /// <returns>table name</returns>
        public static string ConfigurationIDColNameGet(ConfigSettingType configType, bool isLogGet)
        {
            string colName = string.Empty;
            switch (configType)
            {
                case ConfigSettingType.POS:
                    colName = (isLogGet) ? "PoSConfigurationLogID" : "PoSConfigurationID";
                    break;
                case ConfigSettingType.Client:
                    colName = (isLogGet) ? "ClientConfigurationLogID" : "ClientConfigurationID";
                    break;
                case ConfigSettingType.Supplier:
                    colName = (isLogGet) ? "SupplierConfigurationLogID" : "SupplierConfigurationID";
                    break;
                case ConfigSettingType.ErrorMap:
                    colName = (isLogGet) ? "ErrorMapLogID" : "ErrorMapID";
                    break;
            }
            if (string.IsNullOrEmpty(colName))
            {
                Assert.Fail(string.Format("no mapping column Name in DB for {0}!", configType.ToString()));
            }
            return colName;
        }

        /// <summary>
        /// get config key for SCSs for app.config
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static string DBConfigKeyGet(CommonEnum.ServieProvider serviceProvider)
        {
            string key = string.Empty;
            switch (serviceProvider)
            {
                case Util.CarCommonEnumManager.ServieProvider.worldSpanSCS:
                    key = "CarWorldspanSCS";
                    break;
                case Util.CarCommonEnumManager.ServieProvider.Amadeus:
                    key = "CarAmadeusSCS";
                    break;
                case Util.CarCommonEnumManager.ServieProvider.MNSCS:
                    key = "CarMicronNexusSCS";
                    break;
                case Util.CarCommonEnumManager.ServieProvider.TitaniumSCS:
                    key = "CarTitaniumSCS";
                    break;
            }
            if (string.IsNullOrEmpty(key))
            {
                Assert.Fail(string.Format("no mapping DB Key in app.config for {0}!", serviceProvider.ToString()));
            }
            return key;
        }        

        /// <summary>
        /// get DB connection by service name
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static string DBConnectionStringGet(CommonEnum.ServiceName service)
        {
            string dbStr = ConfigSettingHelper.DBConfigKeyGet(service);
            if (string.IsNullOrEmpty(dbStr))
            {
                Assert.Fail(string.Format("no mapping ConnectionString in app.config for {0}!", service.ToString()));
            }
            return ConfigurationManager.ConnectionStrings[dbStr].ConnectionString;
        }

        /// <summary>
        /// get where condition script, e.g. A='A' or A=1, or A is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="field"></param>
        /// <param name="noReturnIfNoValue"></param>
        /// <returns></returns>
        public static string SubConditionQueryGet<T>(string value, string field = "", bool noReturnIfNoValue=false)
        {
            bool isNotNull = (null != value && "null" != value);
            string condition = string.Format("{0}='{1}'", field, value);
            if (typeof(T) == typeof(int) )
            {
                condition = string.Format("{0}={1}", field, value);
            }
            if (noReturnIfNoValue)
            {
                return (isNotNull) ? condition : "";
            }
            return (isNotNull) ? condition : string.Format("{0} is null", field);
        }

        /// <summary>
        /// check if column exists
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool ColumnExists(SqlDataReader reader, string columnName)
        {
            reader.GetSchemaTable().DefaultView.RowFilter = "ColumnName= '" + columnName + "'";
            return (reader.GetSchemaTable().DefaultView.Count > 0);
        }

        /// <summary>
        /// check column if exists and set a default value if column not exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static T ColumnNameCheck<T>(SqlDataReader reader, string columnName, T defaultVal)
        {
            if (ColumnExists(reader, columnName))
            {
                object o = reader[columnName];
                if (!string.IsNullOrEmpty(o.ToString()))
                {
                    return (T)Convert.ChangeType(o, typeof(T));
                }
            }
            return defaultVal;
        }

        /// <summary>
        /// execut DB script with non returns
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sqlScript"></param>
        public static void DBExecuteNonQuery(string connectionString, string sqlScript)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = sqlScript;
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }     
    
}
