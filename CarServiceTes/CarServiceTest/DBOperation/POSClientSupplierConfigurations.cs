using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Expedia.CarInterface.CarServiceTest.TestDataGenenator.TestConfigData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonEnum = Expedia.CarInterface.CarServiceTest.Util.CarCommonEnumManager;
using Expedia.CarInterface.CarServiceTest.Util;
using Expedia.CarInterface.CarServiceTest.Verification.Common;
using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.PlaceTypes.V4;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class ConfigurationSettingDataTbl : ConfigurationSettingData
    {
        public int ConfigurationID { get; set; }
        public int AuditActionID { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Expedia.CarInterface.CarServiceTest.TestDataGenenator.TestConfigData.supplierUpdateType UpdateType { get; set; }

        public ConfigurationSettingDataTbl(int configurationID = 0
            , string env = null
            , string sName = null
            , string sValue = null
            , string jCode = null
            , string cCode = null
            , string mUnitCode = null
            , string cID = null
            , string sID = null
            , DateTime? createDate = null
            , string createdBy = null
            , DateTime? updateDate = null
            , string lastUpdatedBy = null
            , int auditActionID = 0
            )
        {
            ConfigurationID = configurationID;
            AuditActionID = auditActionID;
            environment = env;
            settingName = sName;
            value = sValue;
            //PoSConfiguration
            jurisdictionCode = jCode;
            companyCode = cCode;
            managementUnitCode = mUnitCode;
            //ClientConfiguration
            clientID = cID;
            //SupplierConfiguration
            supplierID = sID;
            CreateDate = (createDate == null) ? DateTime.Now : Convert.ToDateTime(createDate);
            CreatedBy = createdBy;
            UpdateDate = (updateDate == null) ? DateTime.Now : Convert.ToDateTime(updateDate);
            LastUpdatedBy = lastUpdatedBy;
        }
        public ConfigurationSettingDataTbl() { }
    }

    /// <summary>
    /// for POSConfiguration, ClientConfiguration, SupplierConfiguration
    /// </summary>
    public class ConfigurationDBHelper
    {
        public string ConnectionString { get; set; }
        public ConfigSettingType ConfigurationType { get; set; }
        public ConfigurationDBHelper(CommonEnum.ServieProvider serviceProvider, ConfigSettingType configType)
        {
            this.ConnectionString = CarSCSCommonHelper.DBConnectionStringGet(ConfigSettingHelper.ServiceNameGet(serviceProvider));
            this.ConfigurationType = configType;
        }
        public ConfigurationDBHelper(CommonEnum.ServiceName service, ConfigSettingType configType)
        {
            this.ConnectionString = CarSCSCommonHelper.DBConnectionStringGet(service);
            this.ConfigurationType = configType;
        }
        public ConfigurationDBHelper(string connectionString, ConfigSettingType configType)
        {
            this.ConnectionString = connectionString;
            this.ConfigurationType = configType;
        }

        public string ConfigurationIDNameGet(bool isLogGet)
        {
            return CarSCSCommonHelper.ConfigurationIDColNameGet(this.ConfigurationType, isLogGet);
        }

        /// <summary>
        /// Get the value of Cost.TransactionFeesPercent 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public int TransactionFeesPercentByEnvNameGet(string env, int supplierID)
        {
            string sql = string.Format(@"select SettingValue,EnvironmentName from SupplierConfiguration 
where SettingName = 'Cost.TransactionFeesPercent' and (EnvironmentName is null or EnvironmentName = '{0}'
and (SupplierID is null or SupplierID = {1}))", env, supplierID);
            List<ConfigurationSettingDataTbl> tbls = CSDTblsGet(sql, this.ConnectionString);
            string transactionFeeValue = null;
            string tempValue = null;     
            foreach (ConfigurationSettingDataTbl tbl in tbls)
            {
                if (tbl.environment == env)
                {
                    transactionFeeValue = tbl.value;
                    break;
                }
                else
                {
                    tempValue = tbl.value;
                }                               
            }
            if (transactionFeeValue == null)
            {
                transactionFeeValue = tempValue;
            }
            return Convert.ToInt16(transactionFeeValue);
        }

        /// <summary>
        ///  Get the value of Cost.TransactionFeesPercent 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public int TransactionFeesPercentGet(string env, int supplierID)
        {
            string transactionFeeValue = "0";
            string sql = string.Format("select SettingValue,EnvironmentName from SupplierConfiguration " +
                " where SettingName = 'Cost.TransactionFeesPercent' and (EnvironmentName is null or EnvironmentName = '{0}') and SupplierID = {1}", env, supplierID);
            List<ConfigurationSettingDataTbl> tbls = CSDTblsGet(sql, this.ConnectionString);
            foreach (ConfigurationSettingDataTbl tbl in tbls)
            {
                if (tbl.environment == env)
                {
                    transactionFeeValue = tbl.value;
                    break;
                }
                else if (null == tbl.environment)
                {
                    transactionFeeValue = tbl.value;
                }
            }
            return Convert.ToInt16(transactionFeeValue);
        }

        /// <summary>
        /// get setting value for POSConfiguration
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="env"></param>
        /// <param name="jurisdictionCode"></param>
        /// <param name="companyCode"></param>
        /// <param name="managementUnitCode"></param>
        /// <returns></returns>
        public string SettingValuePOSGet(string settingName, string env = null, string jurisdictionCode = null, string companyCode = null, string managementUnitCode = null)
        {
            ConfigurationSettingDataTbl config = new ConfigurationSettingDataTbl(env: env, sName: settingName, jCode: jurisdictionCode, cCode: companyCode, mUnitCode: managementUnitCode);
            return SettingValueGet(config);
        }

        /// <summary>
        /// get setting value for SupplierConfiguration
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="env"></param>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public string SettingValueSupplierGet(string settingName, string env = null, string supplierID = null)
        {
            ConfigurationSettingDataTbl config = new ConfigurationSettingDataTbl(env: env, sName: settingName, sID: supplierID);
            return SettingValueGet(config);
        }

        /// <summary>
        /// Get specific column value based on POS, e.g: CarWorldspanSCS..PoSToWorldspanDefaultSegmentMap
        /// </summary>
        /// <param name="jurisdictionCode"></param>
        /// <param name="companyCode"></param>
        /// <param name="managementUnitCode"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string POSValueGet(PointOfSaleKey posKey, string columnName)
        {
            ConfigurationSettingDataTbl config = new ConfigurationSettingDataTbl(jCode: posKey.JurisdictionCountryCode, cCode: posKey.CompanyCode, mUnitCode: posKey.ManagementUnitCode);
            return ColumnValueGet(config, columnName);
        }

        /// <summary>
        /// get setting value by configutaion entity
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public string SettingValueGet(ConfigurationSettingDataTbl config)
        {
            List<string> vals = SettingValuesGet(TBLQueryGet(config, this.ConfigurationType, false), this.ConnectionString);
            if (vals.Count > 0)
            {
                return vals[0];
            }
            return string.Empty;
        }

        /// <summary>
        /// Get value for one specific column
        /// </summary>
        /// <param name="config"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string ColumnValueGet(ConfigurationSettingDataTbl config, string columnName)
        {
            List<string> vals = ColumnValuesGet(TBLQueryGet(config, this.ConfigurationType, false), this.ConnectionString, columnName);
            if (vals.Count > 0)
            {
                return vals[0];
            }
            return string.Empty;
        }

        /// <summary>
        /// get configutaion entity list 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="isLogGet"></param>
        /// <param name="topCount"></param>
        /// <param name="sortdirection"></param>
        /// <param name="defaultNullValue"></param>
        /// <returns></returns>
        public List<ConfigurationSettingDataTbl> CSDTblsGet(ConfigurationSettingDataTbl config
            , bool isLogGet = false
            , int topCount = 0
            , CommonEnum.SortDirection sortdirection = CommonEnum.SortDirection.None
            , string defaultNullValue = null)
        {            
            List<ConfigurationSettingDataTbl> tbls = CSDTblsGet(
                TBLQueryGet(config, this.ConfigurationType, isLogGet, topCount, sortdirection)
                , this.ConnectionString
                , ConfigurationIDNameGet(isLogGet)
                , defaultNullValue);
            return tbls;
        }

        /// <summary>
        /// get configutaion entity list 
        /// </summary>
        /// <param name="sConfig"></param>
        /// <param name="isLogGet"></param>
        /// <param name="topCount"></param>
        /// <param name="sortdirection"></param>
        /// <param name="defaultNullValue"></param>
        /// <returns></returns>
        public List<ConfigurationSettingDataTbl> CSDTblsGet(SupplierConfiguration sConfig
            , bool isLogGet = false
            , int topCount = 0
            , CommonEnum.SortDirection sortdirection = CommonEnum.SortDirection.None
            , string defaultNullValue = null)
        {
            ConfigurationSettingDataTbl config = ConvertToConfigurationSettingDataTbl(sConfig);
            return CSDTblsGet(config, isLogGet, topCount, sortdirection, defaultNullValue);
        }

        /// <summary>
        /// get all configutaion entity list 
        /// </summary>
        /// <param name="isLogGet"></param>
        /// <param name="defaultNullValue"></param>
        /// <returns></returns>
        public List<ConfigurationSettingDataTbl> CSDTblsGetAll(bool isLogGet = false, string defaultNullValue = null)
        {
            string tblName = (isLogGet) ? ConfigSettingHelper.ConfigLogTableNameGet(this.ConfigurationType) : ConfigSettingHelper.ConfigTableNameGet(this.ConfigurationType);
            string query = string.Format("select * from {0}", tblName);
            return CSDTblsGet(query, this.ConnectionString, ConfigurationIDNameGet(isLogGet), defaultNullValue);
        }

        /// <summary>
        /// update SupplierConfiguration
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="settingValue"></param>
        /// <param name="env"></param>
        /// <param name="supplierID"></param>
        public void SupplierConfigUpdate(string settingName, string settingValue, string env = null, string supplierID = null)
        {
            ConfigurationSettingDataTbl config = new ConfigurationSettingDataTbl(env: env, sName: settingName, sID: supplierID);            
            string where = TBLQueryGetWhere(config, ConfigSettingType.Supplier);
            if (string.IsNullOrEmpty(where))
            {
                Assert.Fail("no where condition for SupplierConfigUpdate");
            }
            string sql = string.Format("update SupplierConfiguration set SettingValue = '{0}' {1}", settingValue, where);

            CarSCSCommonHelper.DBExecuteNonQuery(this.ConnectionString, sql);
        }               

        /// <summary>
        /// convert SupplierConfiguration entity to ConfigurationSettingDataTbl entity
        /// </summary>
        /// <param name="sConfig"></param>
        /// <returns></returns>
        public static ConfigurationSettingDataTbl ConvertToConfigurationSettingDataTbl(SupplierConfiguration sConfig)
        {
            ConfigurationSettingDataTbl config = new ConfigurationSettingDataTbl(configurationID: 0, env: sConfig.environment, sName: sConfig.settingName, sValue: sConfig.value
                , sID: sConfig.supplierID, lastUpdatedBy: sConfig.updatedBy);
            config.UpdateType = (supplierUpdateType)sConfig.updateType;
            return config;
        }               

        /// <summary>
        /// get where script 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="configType"></param>
        /// <returns></returns>
        public static string TBLQueryGetWhere(ConfigurationSettingDataTbl config, ConfigSettingType configType)
        {
            string where = string.Empty;
            switch (configType)
            {
                case ConfigSettingType.POS:
                    where = string.Format("where 1=1 {0} {1}  {2} {3} {4}"
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(config.jurisdictionCode, "and JurisdictionCode")
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(config.companyCode, "and CompanyCode")
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(config.managementUnitCode, "and ManagementUnitCode")
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(config.settingName, "and SettingName")
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(config.environment, "and EnvironmentName"));
                    break;
                case ConfigSettingType.PoSToWorldspanDefaultSegmentMap:
                    where = string.Format("where 1=1 {0} {1}  {2}"
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(config.jurisdictionCode, "and JurisdictionCode")
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(config.companyCode, "and CompanyCode")
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(config.managementUnitCode, "and ManagementUnitCode")
                        );
                    break;
                case ConfigSettingType.Client:
                    where = string.Format("where 1=1 {0} {1} {2}"
                       , CarSCSCommonHelper.SubConditionQueryGet<string>(config.clientID, "and ClientID") //clientID='config.clientID' 
                       , CarSCSCommonHelper.SubConditionQueryGet<string>(config.settingName, "and SettingName")
                       , CarSCSCommonHelper.SubConditionQueryGet<string>(config.environment, "and EnvironmentName"));
                    break;
                case ConfigSettingType.Supplier:
                    string updatedBy = "";
                    if (config.LastUpdatedBy != null)
                    {
                        if (config.UpdateType == supplierUpdateType.Add)
                        {
                            updatedBy = CarSCSCommonHelper.SubConditionQueryGet<string>(config.LastUpdatedBy, "and CreatedBy");
                        }
                        if (config.UpdateType == supplierUpdateType.Update)
                        {
                            updatedBy = CarSCSCommonHelper.SubConditionQueryGet<string>(config.LastUpdatedBy, "and LastUpdatedBy");
                        }

                    }
                    where = string.Format("where 1=1 {0} {1} {2} {3}"
                      , CarSCSCommonHelper.SubConditionQueryGet<string>(config.supplierID, "and SupplierID") //SupplierID='config.supplierID' 
                      , CarSCSCommonHelper.SubConditionQueryGet<string>(config.settingName, "and SettingName")
                      , CarSCSCommonHelper.SubConditionQueryGet<string>(config.environment, "and EnvironmentName")
                      , updatedBy);

                    break;
            }
            return where;
        }
        
        /// <summary>
        /// get query script
        /// </summary>
        /// <param name="config"></param>
        /// <param name="configType"></param>
        /// <param name="isLogGet"></param>
        /// <param name="topCount"></param>
        /// <param name="sortdirection"></param>
        /// <returns></returns>
        public static string TBLQueryGet(ConfigurationSettingDataTbl config
            , ConfigSettingType configType
            , bool isLogGet
            , int topCount = 0
            , CommonEnum.SortDirection sortdirection = CommonEnum.SortDirection.None)
        {
            string topStr = string.Empty;
            if (topCount > 0)
            {
                topStr = "top " + topCount;
            }
            string orderBy = string.Empty;
            if (sortdirection == CommonEnum.SortDirection.ASC)
            {
                orderBy = "order by 1 asc";
            }
            else if (sortdirection == CommonEnum.SortDirection.DESC)
            {
                orderBy = "order by 1 desc";
            }

            string tblName = (isLogGet) ? ConfigSettingHelper.ConfigLogTableNameGet(configType) : ConfigSettingHelper.ConfigTableNameGet(configType);
            string where = TBLQueryGetWhere(config, configType);
            return string.Format("select {0} * from {1} {2} {3} ", topStr, tblName, where, orderBy);
        }

        /// <summary>
        /// get column value by query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<string> ColumnValuesGet(string query, string connectionString, string columnName)
        {
            List<string> values = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = query;

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                string val;
                while (reader.Read())
                {
                     val = reader[columnName].ToString();
                     values.Add(val);
                }
                conn.Close();
            }
            return values;
        }

        /// <summary>
        /// get setting value by query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<string> SettingValuesGet(string query, string connectionString, string columnName = null)
        {            
            return ColumnValuesGet(query, connectionString, "SettingValue");
        }

        /// <summary>
        /// get ConfigurationSettingDataTbl entity list by query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <param name="configIDColName"></param>
        /// <param name="defaultNullValue"></param>
        /// <returns></returns>
        public static List<ConfigurationSettingDataTbl> CSDTblsGet(string query, string connectionString
            , string configIDColName = "PoSConfigurationID", string defaultNullValue=null)
        {
            List<ConfigurationSettingDataTbl> tbls = new List<ConfigurationSettingDataTbl>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = query;
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                ConfigurationSettingDataTbl tbl;               
                while (reader.Read())
                {
                    tbl = new ConfigurationSettingDataTbl();
                    tbl.environment = CarSCSCommonHelper.ColumnNameCheck<string>(reader, "environmentName", defaultNullValue);
                    tbl.settingName = CarSCSCommonHelper.ColumnNameCheck<string>(reader, "settingName", defaultNullValue);
                    tbl.value = CarSCSCommonHelper.ColumnNameCheck<string>(reader, "SettingValue", defaultNullValue);
                    tbl.jurisdictionCode = CarSCSCommonHelper.ColumnNameCheck<string>(reader, "jurisdictionCode", defaultNullValue);
                    tbl.companyCode = CarSCSCommonHelper.ColumnNameCheck<string>(reader, "companyCode", defaultNullValue);
                    tbl.managementUnitCode = CarSCSCommonHelper.ColumnNameCheck<string>(reader, "managementUnitCode", defaultNullValue);
                    tbl.supplierID = CarSCSCommonHelper.ColumnNameCheck<string>(reader, "supplierID", defaultNullValue);
                    tbl.clientID = CarSCSCommonHelper.ColumnNameCheck<string>(reader, "clientID", defaultNullValue);
                    tbl.CreateDate = CarSCSCommonHelper.ColumnNameCheck<DateTime>(reader,"CreateDate", DateTime.MinValue);
                    tbl.CreatedBy = CarSCSCommonHelper.ColumnNameCheck<string>(reader, "CreatedBy", defaultNullValue);
                    tbl.UpdateDate = CarSCSCommonHelper.ColumnNameCheck<DateTime>(reader,"UpdateDate", DateTime.MinValue);
                    tbl.LastUpdatedBy = CarSCSCommonHelper.ColumnNameCheck<string>(reader, "LastUpdatedBy", defaultNullValue);
                    tbl.ConfigurationID = CarSCSCommonHelper.ColumnNameCheck<int>(reader, configIDColName, 0);
                    tbl.AuditActionID = CarSCSCommonHelper.ColumnNameCheck<int>(reader, "AuditActionID", 0);
                    tbls.Add(tbl);
                }
                conn.Close();
            }
            return tbls;
        } 
    }   
    
}
