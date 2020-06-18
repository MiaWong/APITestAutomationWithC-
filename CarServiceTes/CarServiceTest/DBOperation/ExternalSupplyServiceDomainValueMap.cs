using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CommonEnum = Expedia.CarInterface.CarServiceTest.Util.CarCommonEnumManager;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{       
    public class ExternalSupplyServiceDomainValueMaptbl
    {
		public string SupplierID { get; set; }		
		public string MessageSystemID{ get; set; }		
		public string DomainType{ get; set; }		
		public string DomainValue{ get; set; }		
		public string ExternalDomainValue{ get; set; }		
		public System.DateTime CreateDate{ get; set; }		
		public string CreatedBy{ get; set; }		
		public System.DateTime UpdateDate{ get; set; }		
		public string LastUpdatedBy{ get; set; }

        public ExternalSupplyServiceDomainValueMaptbl(string supplierID = null
            , string messageSystemID = null
            , string domainType = null
            , string domainValue = null
            , string externalDomainValue = null
            , DateTime? createDate = null
            , string createdBy = null
            , DateTime? updateDate = null
            , string lastUpdatedBy = null
            )
        {
            SupplierID = supplierID;
            MessageSystemID = messageSystemID;
            DomainType = domainType;
            DomainValue = domainValue;
            ExternalDomainValue = externalDomainValue;
            CreateDate = (createDate == null) ? DateTime.Now : Convert.ToDateTime(createDate);
            CreatedBy = createdBy;
            UpdateDate = (updateDate == null) ? DateTime.Now : Convert.ToDateTime(updateDate);
            LastUpdatedBy = lastUpdatedBy;
        }
    }

    public class ExternalSupplyServiceDomainValueMapHelper
    {
        public string ConnectionString { get; set; }
        public ExternalSupplyServiceDomainValueMapHelper(CommonEnum.ServieProvider serviceProvider)
        {
            this.ConnectionString = CarSCSCommonHelper.DBConnectionStringGet(Util.ConfigSettingHelper.ServiceNameGet(serviceProvider));
        }
        public ExternalSupplyServiceDomainValueMapHelper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        ///  Query ExternalSupplyServiceDomainValueMap table to get mapping info
        /// </summary>
        public string ESSDVMtblsGetDomainValue(string domainType, string externalDomainValue, string additionDomainValue = null, 
            string messageSystemID = null, string supplierID = null)
        {
            ExternalSupplyServiceDomainValueMaptbl searchTbl = new ExternalSupplyServiceDomainValueMaptbl(
                domainType: domainType, externalDomainValue: externalDomainValue, domainValue: additionDomainValue, 
                messageSystemID: messageSystemID, supplierID: supplierID);
            return ESSDVMtblsGetDomainValue(TBLQueryGet(searchTbl));
        }

        /// <summary>
        /// in CarWorldspanSCS, column name is DomainTypeID, not DomainType
        /// </summary>
        /// <param name="domainTypeID"></param>
        /// <param name="externalDomainValue"></param>
        /// <param name="additionDomainValue"></param>
        /// <returns></returns>
        public string ESSDVMtblsGetDomainValue(int domainTypeID, string externalDomainValue, string additionDomainValue = null)
        {
            ExternalSupplyServiceDomainValueMaptbl searchTbl = new ExternalSupplyServiceDomainValueMaptbl(
                domainType: domainTypeID.ToString(), externalDomainValue: externalDomainValue, domainValue: additionDomainValue);
            string[] columns = new string[1] { "DomainType" };
            return ESSDVMtblsGetDomainValue(TBLQueryGet(searchTbl, columns));
        }

        /// <summary>
        /// Get the whole ExternalSupplyServiceDomainValueMap
        /// </summary>
        /// <returns></returns>
        public List<ExternalSupplyServiceDomainValueMaptbl> ESSDVMtblsGetByDomainTypeSupplierID(string domainType = null, string supplierID = null)
        {
            string domainTypeStr = domainType != null ? String.Format("and DomainType='{0}'", domainType) : "";
            string supplierIDStr = supplierID != null ? String.Format("and SupplierID='{0}'", supplierID) : "";
            string query = string.Format("select * from ExternalSupplyServiceDomainValueMap where 1=1 {0} {1}", domainTypeStr, supplierIDStr);
            return ESSDVMtblsGet(query, this.ConnectionString);
        }

        /// <summary>
        /// get by query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string ESSDVMtblsGetDomainValue(string query)
        {
            List<ExternalSupplyServiceDomainValueMaptbl> tbls = ESSDVMtblsGet(query, this.ConnectionString);
            string ret = "";
          //  "select * from ExternalSupplyServiceDomainValueMap where 1=1 and DomainType='CarVendorLocation' and ExternalDomainValue='CDG'  and SupplierID='43' "
            if (query.Contains("DomainType='CarVendorLocation'") || query.Contains("DomainType='RatePeriod'"))
            {
                ret = tbls.Count == 0 ? null : tbls[0].DomainValue;
            }
            else
            { 
               foreach (ExternalSupplyServiceDomainValueMaptbl tbl in tbls)
               {
                ret += tbl.DomainValue;
               }
            }
            return ret;
        }


        public List<ExternalSupplyServiceDomainValueMaptbl> ESSDVMtblsGetExternalSupplyServiceDomainValue(string domainType, String locationCode, String supplierId)
        {
            String query = "select * from ExternalSupplyServiceDomainValueMap where DomainType=\'" + domainType + "\' and SupplierID=\'" + supplierId + "\' and ExternalDomainValue like " + "\'"
            + locationCode + "T%\'" + " or ExternalDomainValue like \'" + locationCode + "O%\'" + " or ExternalDomainValue like \'" + locationCode + "A%\'";
            List<ExternalSupplyServiceDomainValueMaptbl> tbls = ESSDVMtblsGet(query, this.ConnectionString);


            return tbls;
        }

        /// <summary>
        ///  Query ExternalSupplyServiceDomainValueMap table to get ExternalDomainValue 
        /// </summary>
        /// <param name="domainType"></param>
        /// <param name="domainValue"></param>
        /// <returns></returns>
        public string ESSDVMtblsGetExternalDomainValue(string domainType, string domainValue, string messageSystemID = null)
        {
            ExternalSupplyServiceDomainValueMaptbl searchTbl = new ExternalSupplyServiceDomainValueMaptbl(
                domainType: domainType, domainValue: domainValue, messageSystemID: messageSystemID);
            string query = TBLQueryGet(searchTbl);
            return ESSDVMtblsGetExternalDomainValue(query);
        }

        /// <summary>
        /// in CarWorldspanSCS, column name is DomainTypeID, not DomainType
        /// </summary>
        /// <param name="domainTypeID"></param>
        /// <param name="domainValue"></param>
        /// <returns></returns>
        public string ESSDVMtblsGetExternalDomainValue(int domainTypeID, string domainValue)
        {
            ExternalSupplyServiceDomainValueMaptbl searchTbl = new ExternalSupplyServiceDomainValueMaptbl(
                domainType: domainTypeID.ToString(), domainValue: domainValue);
            string[] columns = new string[1] { "DomainType" };
            string query = TBLQueryGet(searchTbl, columns);
            return ESSDVMtblsGetExternalDomainValue(query);
        }
        /// <summary>
        /// get by query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string ESSDVMtblsGetExternalDomainValue(string query)
        {
            List<ExternalSupplyServiceDomainValueMaptbl> tbls = ESSDVMtblsGet(query, this.ConnectionString);
            List<string> externalDomainValues = new List<string>();
            foreach (ExternalSupplyServiceDomainValueMaptbl tbl in tbls)
            {
                externalDomainValues.Add(tbl.ExternalDomainValue);
            }
            return string.Join(",", externalDomainValues.ToArray());
        }

        /// <summary>
        /// ExternalSupplyServiceDomainValueMap table query get
        /// </summary>
        /// <param name="searchTbl"></param>
        /// <returns></returns>
        public static string TBLQueryGet(ExternalSupplyServiceDomainValueMaptbl searchTbl, string[] columnNames=null)
        {
            string domainTypeStr = "and DomainType";
            string eDomainValueStr = "and ExternalDomainValue";
            string domainValueStr =  "and DomainValue";
            string supplierIDStr = "and SupplierID";
            string messageSystemIDStr = "and MessageSystemID";
            if (columnNames != null)
            {
                if (columnNames.Length > 0) domainTypeStr = "and " + columnNames[0] ;
                if (columnNames.Length > 1) eDomainValueStr = "and " + columnNames[1];
                if (columnNames.Length > 2) domainValueStr = "and " + columnNames[2];
                if (columnNames.Length > 3) supplierIDStr = "and " + columnNames[3] ;
                if (columnNames.Length > 4) messageSystemIDStr = "and " + columnNames[4];
            }
            return string.Format("select * from ExternalSupplyServiceDomainValueMap where 1=1 {0} {1} {2} {3} {4}"
                    , CarSCSCommonHelper.SubConditionQueryGet<string>(searchTbl.DomainType, domainTypeStr, noReturnIfNoValue: true)
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(searchTbl.ExternalDomainValue, eDomainValueStr, noReturnIfNoValue: true)
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(searchTbl.DomainValue, domainValueStr, noReturnIfNoValue: true)
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(searchTbl.SupplierID, supplierIDStr, noReturnIfNoValue: true)
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(searchTbl.MessageSystemID, messageSystemIDStr, noReturnIfNoValue: true));
        }

        /// <summary>
        /// ExternalSupplyServiceDomainValueMap entity list get by query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<ExternalSupplyServiceDomainValueMaptbl> ESSDVMtblsGet(
            string query, string connectionString)
        {
            List<ExternalSupplyServiceDomainValueMaptbl> tbls = new List<ExternalSupplyServiceDomainValueMaptbl>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = query;

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                ExternalSupplyServiceDomainValueMaptbl tbl;
                string domainTypeColname = "DomainType";
                if (!CarSCSCommonHelper.ColumnExists(reader, domainTypeColname))
                {
                    domainTypeColname = "DomainTypeID";
                }
                while (reader.Read())
                {                
                    tbl = new ExternalSupplyServiceDomainValueMaptbl();
                    tbl.SupplierID = reader["SupplierID"].ToString();
                    tbl.MessageSystemID = reader["MessageSystemID"].ToString();
                    tbl.DomainType = reader[domainTypeColname].ToString();
                    tbl.DomainValue = reader["DomainValue"].ToString();
                    tbl.ExternalDomainValue = reader["ExternalDomainValue"].ToString();
                    tbl.CreateDate = Convert.ToDateTime(reader["CreateDate"].ToString());
                    tbl.CreatedBy = reader["CreatedBy"].ToString();
                    tbl.UpdateDate = Convert.ToDateTime(reader["UpdateDate"].ToString());
                    tbl.LastUpdatedBy = reader["LastUpdatedBy"].ToString();
                    tbls.Add(tbl);
                }
                conn.Close();
            }
            return tbls;
        }

        /// <summary>
        ///  Query ExternalSupplyServiceDomainValueMap table by DomainType and external DomainValue 
        /// </summary>
        /// <param name="domainType"></param>
        /// <param name="domainValue"></param>
        /// <returns></returns>
        public List<ExternalSupplyServiceDomainValueMaptbl> ESSDVMtblsGetAllFiledByDomainTypeAndExternalDomainValue(string domainType, string externalDomainValue, string supplierID)
        {
            ExternalSupplyServiceDomainValueMaptbl searchTbl = new ExternalSupplyServiceDomainValueMaptbl(domainType: domainType, externalDomainValue: externalDomainValue, supplierID:supplierID);
            string query = TBLQueryGet(searchTbl);
            return ESSDVMtblsGet(query, this.ConnectionString);
        }

        public List<ExternalSupplyServiceDomainValueMaptbl> ESSDVMtblsGetAllFiledByDomainTypeAndDomainValue(string domainType, string domainValue, string supplierID)
        {
            ExternalSupplyServiceDomainValueMaptbl searchTbl = new ExternalSupplyServiceDomainValueMaptbl(domainType: domainType, domainValue: domainValue, supplierID: supplierID);
            string query = TBLQueryGet(searchTbl);
            return ESSDVMtblsGet(query, this.ConnectionString);
        }
    }
}
