using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expedia.CarInterface.CarServiceTest.DBOperation.DBUtil
{
    public class SupplierConfigurationTbl
    {
        public string SupplierConfigurationID { get; set; }
        public string EnvironmentName { get; set; }
        public string SupplierID { get; set; }
        public string SetttingName { get; set; }
        public string SettingValue { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string LastUpdatedBy { get; set; }

        public SupplierConfigurationTbl() { }
        public SupplierConfigurationTbl(string envName, string supplierID, string settingName, string settingValue, DateTime createDate, string createdBy, DateTime updateDate, string lastUpdatedBy)
        {
            EnvironmentName = envName;
            SupplierID = supplierID;
            SettingValue = settingValue;
            CreateDate = createDate;
            CreatedBy = createdBy;
            UpdateDate = updateDate;
            LastUpdatedBy = lastUpdatedBy;
        }
    }
    
    public class SupplierConfigurationLogTbl
    {
        public string EnvironmentName { get; set; }
        public string SupplierID { get; set; }
        public string SetttingName { get; set; }
        public string SettingValue { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public int AuditActionID { get; set; }

        public SupplierConfigurationLogTbl() { }

        public SupplierConfigurationLogTbl(string envName, string supplierID, string settingName, string settingValue, DateTime createDate, string createBy, int auditActionID)
        {
            EnvironmentName = envName;
            SupplierID = supplierID;
            SettingValue = settingValue;
            CreateDate = createDate;
            CreatedBy = createBy;
            AuditActionID = auditActionID;
        }
    }

    public class ErrorMapConfigTbl
    {
        public int errorMapID { get; set; }
        public string errorCode { get; set; }
        public string textRegex { get; set; }
        public string errorNode { get; set; }
        public string messageType { get; set; }
        public string message { get; set; }
        public string interruptProcessing { get; set; }
        public string logError { get; set; }
        public string updatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdateDate { get; set; }

        public int updateType { get; set; }
        public string deleteValue { get; set; }
    }

    public class ErrorMapConfigLogTbl
    {
        public int auditAcationID { get; set; }
        public int errorMapID {get;set;}
        public ErrorMapConfigTbl errorMapConfig { get; set; }

        public ErrorMapConfigLogTbl()
        {
            errorMapConfig = new ErrorMapConfigTbl();
        }
    }
}
