using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CommonEnum = Expedia.CarInterface.CarServiceTest.Util.CarCommonEnumManager;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{       
    public class SupplierItemMaptbl
    {
		public string SupplierItemMapID { get; set; }		
		public string SupplySubsetID{ get; set; }		
		public string ItemKey{ get; set; }		
		public string ItemValue{ get; set; }		
		public System.DateTime CreateDate{ get; set; }		
		public string CreatedBy{ get; set; }		
		public System.DateTime UpdateDate{ get; set; }		
		public string LastUpdatedBy{ get; set; }

        public SupplierItemMaptbl(string  supplierItemMapID = null
            , string  supplySubsetID =null
            , string itemKey = null
            , string itemValue = null
           
            , DateTime? createDate = null
            , string createdBy = null
            , DateTime? updateDate = null
            , string lastUpdatedBy = null
            )
        {
            SupplierItemMapID = supplierItemMapID;
            SupplySubsetID = supplySubsetID;
            ItemKey = itemKey;
            ItemValue = itemValue;
            CreateDate = (createDate == null) ? DateTime.Now : Convert.ToDateTime(createDate);
            CreatedBy = createdBy;
            UpdateDate = (updateDate == null) ? DateTime.Now : Convert.ToDateTime(updateDate);
            LastUpdatedBy = lastUpdatedBy;
        }
    }

    public class SupplierItemMapHelper
    {
        public string ConnectionString { get; set; }
        public SupplierItemMapHelper(CommonEnum.ServieProvider serviceProvider)
        {
            this.ConnectionString = CarSCSCommonHelper.DBConnectionStringGet(Util.ConfigSettingHelper.ServiceNameGet(serviceProvider));
        }
        public SupplierItemMapHelper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        ///  Query SupplierItemMap table 
        /// </summary>
        public List<SupplierItemMaptbl> QuerySuplierItemMap(String supplySubsetID = null, string itemKey = null, string itemValue = null)
        {
            SupplierItemMaptbl searchTbl = new SupplierItemMaptbl(
                supplySubsetID: supplySubsetID, itemKey: itemKey, itemValue: itemValue
                );
            return SupIPtblsGet(TBLQueryGet(searchTbl), this.ConnectionString);
        }

        /// <summary>
        ///  Query SupplierItemMap table 
        /// </summary>
        public  void  UpdateSuplierItemMap(String supplySubsetID, string itemKey, string itemValue = null, string createdBy= "test", string updateBy = "test")
        {
            SupplierItemMaptbl searchTbl = new SupplierItemMaptbl(
                supplySubsetID: supplySubsetID, itemKey: itemKey, itemValue: itemValue, createdBy: createdBy, lastUpdatedBy: updateBy
                );
           TBLUpdate(searchTbl, this.ConnectionString);
        }

       
        /// <summary>
        /// SupplierItemMap table query get
        /// </summary>
        /// <param name="searchTbl"></param>
        /// <returns></returns>
        public static string TBLQueryGet(SupplierItemMaptbl searchTbl)
        {
            string supplySubsetID = "and SupplySubsetID";
            string itemKey = "and ItemKey";
            string itemValue = "and ItemValue";

            
            return string.Format("select * from SupplierItemMap where 1=1 {0} {1} {2} "
                    , CarSCSCommonHelper.SubConditionQueryGet<int>(searchTbl.SupplySubsetID, supplySubsetID, noReturnIfNoValue: true)
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(searchTbl.ItemKey, itemKey, noReturnIfNoValue: true)
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(searchTbl.ItemValue, itemValue, noReturnIfNoValue: true)
                       );
        }

        /// <summary>
        /// SupplierItemMap table query get
        /// </summary>
        /// <param name="searchTbl"></param>
        /// <returns></returns>
        public static void TBLUpdate(SupplierItemMaptbl searchTbl, string connectionString)
        {
             String query = string.Format("update SupplierItemMap set ItemValue='{0}' where supplySubsetID = 99999 and ItemKey = 'foo' "
                               , searchTbl.ItemValue);
             using (SqlConnection conn = new SqlConnection(connectionString))
             {
                 SqlCommand cmd = conn.CreateCommand();
                 conn.Open();

                 cmd.CommandText = query;

                 SqlDataReader reader = cmd.ExecuteReader();
                 conn.Close();
             }
        }


       
        /// <summary>
        /// SupplyItemMap entity list get by query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<SupplierItemMaptbl> SupIPtblsGet(
            string query, string connectionString)
        {
            List<SupplierItemMaptbl> supplierItemMaptbls = new List<SupplierItemMaptbl>();
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = query;

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    SupplierItemMaptbl tbl = new SupplierItemMaptbl();

                    tbl.SupplierItemMapID = reader["SupplierItemMapID"].ToString();
                    tbl.ItemKey = reader["ItemKey"].ToString();
                    tbl.ItemValue = reader["ItemValue"].ToString();
                    tbl.SupplySubsetID = reader["SupplySubsetID"].ToString();
                    tbl.LastUpdatedBy = reader["LastUpdatedBy"].ToString();
                    tbl.CreateDate = Convert.ToDateTime(reader["CreateDate"].ToString());
                    tbl.CreatedBy = reader["CreatedBy"].ToString();
                    tbl.UpdateDate = Convert.ToDateTime(reader["UpdateDate"].ToString());
                    tbl.LastUpdatedBy = reader["LastUpdatedBy"].ToString();

                    supplierItemMaptbls.Add(tbl);

                }
                conn.Close();
            }
            return supplierItemMaptbls;
        }

       
    }
    
}
