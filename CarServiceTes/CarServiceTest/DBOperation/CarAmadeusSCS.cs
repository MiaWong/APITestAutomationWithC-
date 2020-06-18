//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Configuration;
//using System.Data.SqlClient;
//using Expedia.CarInterface.CarServiceTest.DBOperation.DBUtil;
//using Expedia.CarInterface.CarServiceTest.TestDataGenenator.TestConfigData;

//namespace Expedia.CarInterface.CarServiceTest.DBOperation
//{
//    public class CarAmadeusSCS
//    {
//        private static string connectionString = ConfigurationManager.ConnectionStrings["CarAmadeusSCS"].ConnectionString;

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

//        public static void getPOSByOfficeID(string officeID, out string jurisdictionCode, out string companyCode, out string managementUnitCode)
//        {
//            jurisdictionCode = "";
//            companyCode = "";
//            managementUnitCode = "";
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                cmd.CommandText = string.Format("select JurisdictionCode, CompanyCode, ManagementUnitCode from PoSToAmadeusDefaultSegmentMap where OfficeID = '{0}'", officeID);
//                cmd.CommandTimeout = 0;
//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    jurisdictionCode = reader["JurisdictionCode"].ToString();
//                    companyCode = reader["CompanyCode"].ToString();
//                    managementUnitCode = reader["ManagementUnitCode"].ToString();
//                    break;
//                }
//                conn.Close();
//            }
//        }

//        public static bool isEgenciaPOS(string jurisdictionCode, string companyCode, string managementUnitCode)
//        {
//            bool isEgenciaPOS = false;
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                cmd.CommandText = string.Format("select * from PoSToAmadeusDefaultSegmentMap where JurisdictionCode ='{0}' and CompanyCode = '{1}' and ManagementUnitCode = '{2}'", jurisdictionCode, companyCode, managementUnitCode);
//                cmd.CommandTimeout = 0;
//                SqlDataReader reader = cmd.ExecuteReader();
//                if (reader.HasRows) isEgenciaPOS = true;
//                conn.Close();
//            }

//            return isEgenciaPOS;
//        }

//        #region Supplier Configuration
//        public static string GetSupplierConfig(string settingName, string env = null, string supplierID = null)
//        {
//            string settingValue = null;

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                string envString = (null != env) ? string.Format("='{0}'", env) : "is null";
//                string supplierIDString = (null != supplierID) ? string.Format("='{0}'", supplierID) : "is null";
//                cmd.CommandText = string.Format("select SettingValue from SupplierConfiguration where SettingName = '{0}' and EnvironmentName {1} and SupplierID {2}", settingName, envString, supplierIDString);

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

//        //Get a record in SupplierConfiguration table by supplierConfiguration
//        public static List<SupplierConfigurationTbl> getSupplierConfig(SupplierConfiguration supplierConfiguration)
//        {
//            List<SupplierConfigurationTbl> supplierConfigurationTbls = new List<SupplierConfigurationTbl>();
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                string envStr = (null == supplierConfiguration.environment || supplierConfiguration.environment == "null") ? "and EnvironmentName is null" : string.Format("and EnvironmentName='{0}'", supplierConfiguration.environment);
//                string supplierIDStr = (null == supplierConfiguration.supplierID) ? "and SupplierID is null" : string.Format("and SupplierID='{0}'", supplierConfiguration.supplierID);
//                string settingValue = (null == supplierConfiguration.value) ? "and SettingValue is null" : string.Format("and SettingValue='{0}'", supplierConfiguration.value);
//                string updatedBy = string.Empty;
//                if (supplierConfiguration.updatedBy != null)
//                {
//                    if (supplierConfiguration.updateType == (int)supplierUpdateType.Add)
//                    {
//                        updatedBy = string.Format("and CreatedBy = '{0}'", supplierConfiguration.updatedBy);
//                    }
//                    if (supplierConfiguration.updateType == (int)supplierUpdateType.Update)
//                    {
//                        updatedBy = string.Format("and LastUpdatedBy = '{0}'", supplierConfiguration.updatedBy);
//                    }

//                }
//                cmd.CommandText = string.Format("select * from SupplierConfiguration where SettingName = '{0}'  {1}  {2} {3} {4}", 
//                    supplierConfiguration.settingName, envStr, supplierIDStr, settingValue, updatedBy);

//                cmd.CommandTimeout = 0;

//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    SupplierConfigurationTbl supplierConfigurationTbl = new SupplierConfigurationTbl();
//                    supplierConfigurationTbl.SupplierConfigurationID = reader["SupplierConfigurationID"].ToString();
//                    if (reader["EnvironmentName"] != null && reader["EnvironmentName"].ToString() != null)
//                        supplierConfigurationTbl.EnvironmentName = reader["EnvironmentName"].ToString();
//                    if (reader["SupplierID"] != null && reader["SupplierID"].ToString() != null)
//                        supplierConfigurationTbl.SupplierID = reader["SupplierID"].ToString();
//                    supplierConfigurationTbl.SetttingName = reader["SettingName"].ToString();
//                    if (reader["SettingValue"] != null && reader["SettingValue"].ToString() != null)
//                        supplierConfigurationTbl.SettingValue = reader["SettingValue"].ToString();
//                    supplierConfigurationTbl.CreateDate = Convert.ToDateTime(reader["CreateDate"].ToString());
//                    supplierConfigurationTbl.CreatedBy = reader["CreatedBy"].ToString();
//                    supplierConfigurationTbl.UpdateDate = Convert.ToDateTime(reader["UpdateDate"].ToString());
//                    supplierConfigurationTbl.LastUpdatedBy = reader["LastUPdatedBy"].ToString();
//                    supplierConfigurationTbls.Add(supplierConfigurationTbl);
//                }
//                conn.Close();
//            }
//            return supplierConfigurationTbls;
//        }

//        //Get all of records in SupplierConfiguration table
//        public static List<SupplierConfigurationTbl> getSupplierConfig()
//        {
//            List<SupplierConfigurationTbl> supplierConfigurationTbls = new List<SupplierConfigurationTbl>();
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
               
//                cmd.CommandText = string.Format("select * from SupplierConfiguration ");

//                cmd.CommandTimeout = 0;

//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    SupplierConfigurationTbl supplierConfigurationTbl = new SupplierConfigurationTbl();
//                    supplierConfigurationTbl.SupplierConfigurationID = reader["SupplierConfigurationID"].ToString();
//                    if (reader["EnvironmentName"] == null || reader["EnvironmentName"].ToString() == null || (reader["EnvironmentName"] is System.DBNull))
//                        supplierConfigurationTbl.EnvironmentName = "null";
//                    else
//                        supplierConfigurationTbl.EnvironmentName = reader["EnvironmentName"].ToString();
//                    if (reader["SupplierID"] == null || reader["SupplierID"].ToString() == null || (reader["SupplierID"] is System.DBNull))
//                        supplierConfigurationTbl.SupplierID = "null";
//                    else
//                        supplierConfigurationTbl.SupplierID = reader["SupplierID"].ToString();
//                    supplierConfigurationTbl.SetttingName = reader["SettingName"].ToString();
//                    if (reader["SettingValue"] == null || reader["SettingValue"].ToString() == null )
//                        supplierConfigurationTbl.SettingValue = "null";
//                    else
//                        supplierConfigurationTbl.SettingValue = reader["SettingValue"].ToString();
//                    supplierConfigurationTbl.CreateDate = Convert.ToDateTime(reader["CreateDate"].ToString());
//                    supplierConfigurationTbl.CreatedBy = reader["CreatedBy"].ToString();
//                    supplierConfigurationTbl.UpdateDate = Convert.ToDateTime(reader["UpdateDate"].ToString());
//                    supplierConfigurationTbl.LastUpdatedBy = reader["LastUPdatedBy"].ToString();
//                    supplierConfigurationTbls.Add(supplierConfigurationTbl);
//                }
//                conn.Close();
//            }
//            return supplierConfigurationTbls;
//        }

//        //Get a record in SupplierConfigurationLog table by supplierConfiguration
//        public static List<SupplierConfigurationLogTbl> getSupplierConfigLog(SupplierConfiguration supplierConfiguration)
//        {
//            List<SupplierConfigurationLogTbl> supplierConfigurationLogTbls = new List<SupplierConfigurationLogTbl>();
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                string envStr = (null == supplierConfiguration.environment || supplierConfiguration.environment == "null") ? "and EnvironmentName is null" : string.Format("and EnvironmentName='{0}'", supplierConfiguration.environment);
//                string supplierIDStr = (null == supplierConfiguration.supplierID) ? "and SupplierID is null" : string.Format("and SupplierID='{0}'", supplierConfiguration.supplierID);
//                string settingValue = (null == supplierConfiguration.value) ? "and SettingValue is null" : string.Format("and SettingValue='{0}'", supplierConfiguration.value);
//                string updatedBy = string.Format("and CreatedBy = '{0}'", supplierConfiguration.updatedBy);

//                cmd.CommandText = string.Format("select top 1 * from SupplierConfigurationLog where SettingName = '{0}'  {1}  {2} {3} {4}  order by supplierConfigurationLogID desc", supplierConfiguration.settingName, envStr, supplierIDStr, settingValue, updatedBy);
//                cmd.CommandTimeout = 0;

//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    SupplierConfigurationLogTbl supplierConfigurationLogTbl = new SupplierConfigurationLogTbl();

//                    supplierConfigurationLogTbl.AuditActionID = Convert.ToInt32(reader["AuditActionID"].ToString());
//                    if (reader["EnvironmentName"] != null && reader["EnvironmentName"].ToString() != null)
//                        supplierConfigurationLogTbl.EnvironmentName = reader["EnvironmentName"].ToString();
//                    if (reader["SupplierID"] != null && reader["SupplierID"].ToString() != null)
//                        supplierConfigurationLogTbl.SupplierID = reader["SupplierID"].ToString();
//                    supplierConfigurationLogTbl.SetttingName = reader["SettingName"].ToString();
//                    if (reader["SettingValue"] != null && reader["SettingValue"].ToString() != null)
//                        supplierConfigurationLogTbl.SettingValue = reader["SettingValue"].ToString();
//                    supplierConfigurationLogTbl.CreateDate = Convert.ToDateTime(reader["CreateDate"].ToString());
//                    supplierConfigurationLogTbl.CreatedBy = reader["CreatedBy"].ToString();

//                    supplierConfigurationLogTbls.Add(supplierConfigurationLogTbl);
//                }
//                conn.Close();
//            }
//            return supplierConfigurationLogTbls;
//        }
        
//        //Update a config in SupplierConfguration table
//        public static void updateSupplierConfig(string settingName, string settingValue, string env = null, string supplierID = null)
//        {
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                string envStr = (null != env) ? string.Format("='{0}'", env) : "is null";
//                string supplierIDStr = (null != supplierID) ? string.Format("='{0}'", supplierID) : "is null";
//                cmd.CommandText = string.Format("update SupplierConfiguration set SettingValue = '{0}' where SettingName ='{1}' and EnvironmentName {2} and SupplierID  {3}", settingValue, settingName, envStr, supplierIDStr);

//                cmd.CommandTimeout = 0;
//                cmd.ExecuteNonQuery();
//                conn.Close();
//            }
        
//        }
//        #endregion

//        #region Error Mapping Config
//        public static bool isErrorMapppingExist(string messageType, string errorCode, string textRegex)
//        {
//            bool result = false;
//            string commandTextAdd = " and MessageType" + (messageType == null ? " is null" : " = '" + messageType + "'");
//            commandTextAdd += " and errorCode" + (errorCode == null ? " is null" : " = '" + errorCode + "'");
//            commandTextAdd += " and TextRegEx" + (textRegex == null ? " is null" : " = '" + textRegex + "'");
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                cmd.CommandText = string.Format("select * from ErrorMap where 1=1 {0}", commandTextAdd);
//                cmd.CommandTimeout = 0;
//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    result = true;
//                    break;
//                }
//                conn.Close();
//            }
//            return result;
//        }

//        public static List<ErrorMapConfigTbl> getErrorMapConfig(string messageType = null, string errorCode = null, string textRegex = null, bool queryAll = false)
//        {
//            List<ErrorMapConfigTbl> errorMapConfig = new List<ErrorMapConfigTbl>();

//            string commandTextAdd = "";
//            if (queryAll == false)
//            {
//                commandTextAdd = " and MessageType" + (messageType == null ? " is null" : " = '" + messageType + "'");
//                commandTextAdd += " and errorCode" + (errorCode == null ? " is null" : " = '" + errorCode + "'");
//                commandTextAdd += " and TextRegEx" + (textRegex == null ? " is null" : " = '" + textRegex + "'");
//            }
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                cmd.CommandText = string.Format("select * from ErrorMap where 1=1 {0}", commandTextAdd);
//                cmd.CommandTimeout = 0;
//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    ErrorMapConfigTbl errorMapConfigTbl = new ErrorMapConfigTbl();
//                    errorMapConfigTbl.messageType = reader["MessageType"].ToString();
//                    errorMapConfigTbl.errorCode = reader["ErrorCode"].ToString();
//                    errorMapConfigTbl.textRegex = reader["TextRegEx"].ToString();
//                    errorMapConfigTbl.errorNode = reader["ErrorNode"].ToString();
//                    errorMapConfigTbl.message = reader["Message"].ToString();
//                    errorMapConfigTbl.interruptProcessing = reader["InterruptProcessingBool"].ToString();
//                    errorMapConfigTbl.logError = reader["LogErrorBool"].ToString();
//                    errorMapConfigTbl.CreateDate = Convert.ToDateTime(reader["CreateDate"].ToString());
//                    errorMapConfigTbl.CreatedBy = reader["CreatedBy"].ToString();
//                    errorMapConfigTbl.UpdateDate = Convert.ToDateTime(reader["UpdateDate"].ToString());
//                    errorMapConfigTbl.updatedBy = reader["LastUPdatedBy"].ToString();
//                    errorMapConfigTbl.errorMapID = Convert.ToInt32(reader["ErrorMapID"].ToString());
//                    errorMapConfig.Add(errorMapConfigTbl);
//                }
//                conn.Close();
//            }
//            return errorMapConfig;
//        }

//        public static List<ErrorMapConfigLogTbl> getErrorMapConfigLog(string messageType = null, string errorCode = null, string textRegex = null)
//        {
//            List<ErrorMapConfigLogTbl> errorMapConfigLogList = new List<ErrorMapConfigLogTbl>();
//            string commandTextAdd = messageType == null ? "" : " and MessageType = '" + messageType + "'";
//            commandTextAdd += errorCode == null ? "" : " and errorCode = '" + errorCode + "'";
//            commandTextAdd += textRegex == null ? "" : " and TextRegEx = '" + textRegex + "'";
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                cmd.CommandText = string.Format("select * from ErrorMapLog where 1=1 {0} order by ErrorMapLogID desc", commandTextAdd);
//                cmd.CommandTimeout = 0;
//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    ErrorMapConfigLogTbl errorMapConfigLog = new ErrorMapConfigLogTbl();
//                    errorMapConfigLog.errorMapConfig.messageType = reader["MessageType"].ToString();
//                    errorMapConfigLog.errorMapConfig.errorCode = reader["ErrorCode"].ToString();
//                    errorMapConfigLog.errorMapConfig.textRegex = reader["TextRegEx"].ToString();
//                    errorMapConfigLog.errorMapConfig.errorNode = reader["ErrorNode"].ToString();
//                    errorMapConfigLog.errorMapConfig.message = reader["Message"].ToString();
//                    errorMapConfigLog.errorMapConfig.interruptProcessing = reader["InterruptProcessingBool"].ToString();
//                    errorMapConfigLog.errorMapConfig.logError = reader["LogErrorBool"].ToString();
//                    errorMapConfigLog.errorMapConfig.CreateDate = Convert.ToDateTime(reader["CreateDate"].ToString());
//                    errorMapConfigLog.errorMapConfig.CreatedBy = reader["CreatedBy"].ToString();
//                    errorMapConfigLog.errorMapID = Convert.ToInt32(reader["ErrorMapID"].ToString());
//                    errorMapConfigLog.auditAcationID = Convert.ToInt32(reader["AuditActionID"].ToString());
//                    errorMapConfigLogList.Add(errorMapConfigLog);
//                    break;
//                }
//                conn.Close();
//            }
//            return errorMapConfigLogList;
//        }

//        public static void ErrorMapConfigUpdate(string updateStr)
//        {
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                SqlCommand cmd = conn.CreateCommand();
//                conn.Open();
//                cmd.CommandText = updateStr;

//                cmd.CommandTimeout = 0;
//                cmd.ExecuteNonQuery();
//                conn.Close();
//            }

//        }
//        #endregion
//    }
//}
