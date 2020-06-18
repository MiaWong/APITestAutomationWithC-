using Expedia.CarInterface.CarServiceTest.Verification.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CommonEnum = Expedia.CarInterface.CarServiceTest.Util.CarCommonEnumManager;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{  
    public class POSToAmadeusDefaultSegmentMapTbl
    {
        public string jurisdictionCode { get; set; }
        public string companyCode { get; set; }
        public string managementUnitCode { get; set; }
        public string OfficeID { get; set; }	
		public System.DateTime CreateDate{ get; set; }		
		public string CreatedBy{ get; set; }		
		public System.DateTime UpdateDate{ get; set; }		
		public string LastUpdatedBy{ get; set; }

        public POSToAmadeusDefaultSegmentMapTbl(string jCode = null
            , string cCode = null
            , string mUnitCode = null
            , string officeID = null
            , DateTime? createDate = null
            , string createdBy = null
            , DateTime? updateDate = null
            , string lastUpdatedBy = null
            )
        {
            jurisdictionCode = jCode;
            companyCode = cCode;
            managementUnitCode = mUnitCode;
            OfficeID = officeID;
            CreateDate = (createDate == null) ? DateTime.Now : Convert.ToDateTime(createDate);
            CreatedBy = createdBy;
            UpdateDate = (updateDate == null) ? DateTime.Now : Convert.ToDateTime(updateDate);
            LastUpdatedBy = lastUpdatedBy;
        }
    }

    public class POSToAmadeusDefaultSegmentMapTblHelper
    {
        public string ConnectionString { get; set; }
        public POSToAmadeusDefaultSegmentMapTblHelper(CommonEnum.ServieProvider serviceProvider)
        {
            this.ConnectionString = CarSCSCommonHelper.DBConnectionStringGet(Util.ConfigSettingHelper.ServiceNameGet(serviceProvider));
        }
        public POSToAmadeusDefaultSegmentMapTblHelper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// get POSConfiguration info by officeid
        /// </summary>
        /// <param name="officeID"></param>
        /// <param name="jurisdictionCode"></param>
        /// <param name="companyCode"></param>
        /// <param name="managementUnitCode"></param>
        public void POSByOfficeIDGet(string officeID, out string jurisdictionCode, out string companyCode, out string managementUnitCode)
        {
            jurisdictionCode = "";
            companyCode = "";
            managementUnitCode = "";
            POSToAmadeusDefaultSegmentMapTbl searchTbl = new POSToAmadeusDefaultSegmentMapTbl(officeID: officeID);
            List<POSToAmadeusDefaultSegmentMapTbl> tbls = POSTADSMtblsGet(TBLQueryGet(searchTbl), this.ConnectionString);
            if (tbls.Count > 0)
            {
                int lastIndex = tbls.Count - 1;
                jurisdictionCode = tbls[lastIndex].jurisdictionCode;
                companyCode = tbls[lastIndex].companyCode;
                managementUnitCode = tbls[lastIndex].managementUnitCode;
            }
        }

        /// <summary>
        /// check if POSConfiguration info exists
        /// </summary>
        /// <param name="jurisdictionCode"></param>
        /// <param name="companyCode"></param>
        /// <param name="managementUnitCode"></param>
        /// <returns></returns>
        public bool isEgenciaPOS(string jurisdictionCode, string companyCode, string managementUnitCode)
        {
            bool isEgenciaPOS = false;
            POSToAmadeusDefaultSegmentMapTbl searchTbl = new POSToAmadeusDefaultSegmentMapTbl(jCode:jurisdictionCode, cCode:companyCode, mUnitCode:managementUnitCode);
            List<POSToAmadeusDefaultSegmentMapTbl> tbls = POSTADSMtblsGet(TBLQueryGet(searchTbl), this.ConnectionString);
            if (tbls.Count > 0)
            {
                isEgenciaPOS = true;
            }
            return isEgenciaPOS;
        }

        /// <summary>
        /// db query get
        /// </summary>
        /// <param name="searchTbl"></param>
        /// <returns></returns>
        public static string TBLQueryGet(POSToAmadeusDefaultSegmentMapTbl searchTbl)
        {
            return string.Format("select * from PoSToAmadeusDefaultSegmentMap where 1=1 {0} {1} {2} {3}"
                    , CarSCSCommonHelper.SubConditionQueryGet<string>(searchTbl.OfficeID, "and OfficeID", noReturnIfNoValue: true)
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(searchTbl.jurisdictionCode, "and jurisdictionCode", noReturnIfNoValue: true)
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(searchTbl.companyCode, "and companyCode", noReturnIfNoValue: true)
                        , CarSCSCommonHelper.SubConditionQueryGet<string>(searchTbl.managementUnitCode, "and managementUnitCode", noReturnIfNoValue: true));
        }

        /// <summary>
        /// POSToAmadeusDefaultSegmentMapTbl entity list get by query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<POSToAmadeusDefaultSegmentMapTbl> POSTADSMtblsGet(string query, string connectionString)
        {
            List<POSToAmadeusDefaultSegmentMapTbl> tbls = new List<POSToAmadeusDefaultSegmentMapTbl>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = query;

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                POSToAmadeusDefaultSegmentMapTbl tbl;
                while (reader.Read())
                {
                    tbl = new POSToAmadeusDefaultSegmentMapTbl();
                    tbl.jurisdictionCode = reader["jurisdictionCode"].ToString();
                    tbl.companyCode = reader["companyCode"].ToString();
                    tbl.managementUnitCode = reader["managementUnitCode"].ToString();
                    tbl.OfficeID = reader["OfficeID"].ToString();
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
    }

}
