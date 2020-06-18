using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;


namespace Expedia.CarInterface.CarServiceTest.DBOperation.DBUtil
{
    public class CarTitaniumSCS_DB
    {

        public static string connectionString = ConfigurationManager.ConnectionStrings["CarTitaniumSCS"].ConnectionString;


        public static String getSupplierConfiguration(uint supplyId, String settingName)
        {
            String settingValue = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select SettingValue from SupplierConfiguration where SupplierID = {0} and SettingName = '{1}'", supplyId, settingName);
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



        public static List<SupplierConfigurationtbl> getSupplierConfigurationValue(string supplyId = null, string settingName = null, string settingValue = null)
        {
            List<SupplierConfigurationtbl> supplierConfigurationtbls = new List<SupplierConfigurationtbl>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = TBLQuerySupplierGet(supplierId: supplyId, settingName: settingName, settingValue: settingValue);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    SupplierConfigurationtbl tbl = new SupplierConfigurationtbl();
                    tbl.SupplierConfigurationID = reader["SupplierConfigurationID"].ToString();
                    tbl.SettingName = reader["SettingName"].ToString();
                    tbl.SettingValue = reader["SettingValue"].ToString();
                    tbl.SupplierID = reader["SupplierID"].ToString();
                    tbl.UpdateDate = Convert.ToDateTime(reader["UpdateDate"].ToString());
                    tbl.LastUpdatedBy = reader["LastUpdatedBy"].ToString();
                    tbl.EnvironmentName = reader["EnvironmentName"].ToString();
                    tbl.CreateDate = Convert.ToDateTime(reader["CreateDate"].ToString());
                    tbl.CreatedBy = reader["CreatedBy"].ToString();
                    supplierConfigurationtbls.Add(tbl);
  
                }
                conn.Close();
            }

            return supplierConfigurationtbls;
        }

        private static string TBLQuerySupplierGet(string supplierId = null, string settingName = null, string settingValue = null)
        {
            string supplierIdf = "and SupplierId";
            string settingNamef = "and SettingName";
            string settingValuef = "and SettingValue";


            return string.Format("select * from SupplierConfiguration where 1=1 {0} {1} {2} "
                    , CarSCSCommonHelper.SubConditionQueryGet<int>(supplierId, supplierIdf, noReturnIfNoValue: true)
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(settingName, settingNamef, noReturnIfNoValue: true)
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(settingValue, settingValuef, noReturnIfNoValue: true)
                       );
        }
       
        public static String getItemValue(uint supplySubSetId, String itemKey)
        {
            String ItemValue = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select ItemValue from SupplierItemMap where SupplySubsetID = {0} and Itemkey = '{1}'", supplySubSetId, itemKey);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ItemValue = reader[0].ToString();
                }
                conn.Close();
            }

            return ItemValue;
        }

    }

    public class SupplierConfigurationtbl
    {
        public string SupplierConfigurationID { get; set; }		
		public string EnvironmentName { get; set; }		
		public string SupplierID { get; set; }		
		public string SettingName { get; set; }
        public string SettingValue { get; set; }
		public System.DateTime CreateDate{ get; set; }		
		public string CreatedBy{ get; set; }		
		public System.DateTime UpdateDate{ get; set; }		
		public string LastUpdatedBy{ get; set; }

        public SupplierConfigurationtbl(string supplierConfigurationID = null
            , string environmentName = null
            , string supplierID = null
            , string settingName = null
            , string settingValue = null
            , DateTime? createDate = null
            , string createdBy = null
            , DateTime? updateDate = null
            , string lastUpdatedBy = null
            )
        {
            SupplierConfigurationID = supplierConfigurationID;
            EnvironmentName = environmentName;
            SupplierID = supplierID;
            SettingName = settingName;
            SettingValue = settingValue;
            CreateDate = (createDate == null) ? DateTime.Now : Convert.ToDateTime(createDate);
            CreatedBy = createdBy;
            UpdateDate = (updateDate == null) ? DateTime.Now : Convert.ToDateTime(updateDate);
            LastUpdatedBy = lastUpdatedBy;
        }

       
    }
}
