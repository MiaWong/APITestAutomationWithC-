using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Expedia.CarInterface.CarServiceTest.DBOperation.DBUtil;
using CommonEnum = Expedia.CarInterface.CarServiceTest.Util.CarCommonEnumManager;
using Expedia.CarInterface.CarServiceTest.Verification.Common;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{  
    public class ErrorMapHelper
    {
        public string ConnectionString { get; set; }
        public ErrorMapHelper(CommonEnum.ServieProvider serviceProvider)
        {
            this.ConnectionString = CarSCSCommonHelper.DBConnectionStringGet(Util.ConfigSettingHelper.ServiceNameGet(serviceProvider));
        }
        public ErrorMapHelper(CommonEnum.ServiceName service)
        {
            this.ConnectionString = CarSCSCommonHelper.DBConnectionStringGet(service);
        }
        public ErrorMapHelper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// check if a error map exists
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="errorCode"></param>
        /// <param name="textRegex"></param>
        /// <returns></returns>
        public bool isErrorMapppingExist(string messageType, string errorCode, string textRegex)
        {
            ErrorMapConfigTbl searchTbl = TBLEntityGet(messageType, errorCode, textRegex);
            List<ErrorMapConfigTbl> tbls = ErrorMapTblsGet(TBLQueryGet(searchTbl), this.ConnectionString);
            if (tbls.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// get error maps 
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="errorCode"></param>
        /// <param name="textRegex"></param>
        /// <param name="queryAll"></param>
        /// <returns></returns>
        public List<ErrorMapConfigTbl> ErrorMapConfigGet(string messageType = null, string errorCode = null, string textRegex = null, bool queryAll = false)
        {
            string query = "select * from ErrorMap";
            if (queryAll == false)
            {
                ErrorMapConfigTbl searchTbl = TBLEntityGet(messageType, errorCode, textRegex);
                query = TBLQueryGet(searchTbl);
            }
            return ErrorMapTblsGet(query, this.ConnectionString);
        }

        /// <summary>
        /// get error map logs
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="errorCode"></param>
        /// <param name="textRegex"></param>
        /// <param name="topCount"></param>
        /// <param name="sortdirection"></param>
        /// <returns></returns>
        public List<ErrorMapConfigLogTbl> ErrorMapConfigLogGet(string messageType = null, string errorCode = null, string textRegex = null
            , int topCount = 0
            , CommonEnum.SortDirection sortdirection = CommonEnum.SortDirection.DESC)
        {
            ErrorMapConfigTbl searchTbl = TBLEntityGet(messageType, errorCode, textRegex);
            string query = TBLQueryGet(searchTbl, true, topCount, sortdirection);
            List<ErrorMapConfigLogTbl> logTbl = ErrorMapLogTblGet(query, this.ConnectionString);           
            return logTbl;
        }

        /// <summary>
        /// error map script get
        /// </summary>
        /// <param name="searchTbl"></param>
        /// <param name="isLogGet"></param>
        /// <param name="topCount"></param>
        /// <param name="sortdirection"></param>
        /// <returns></returns>
        public static string TBLQueryGet(ErrorMapConfigTbl searchTbl, bool isLogGet=false
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
            
            return string.Format("select {0} * from {1} where 1=1 {2} {3} {4} {5}"
                , topStr
                , (isLogGet ? "ErrorMapLog" : "ErrorMap")
                , CarSCSCommonHelper.SubConditionQueryGet<string>(searchTbl.messageType, "and MessageType")
                , CarSCSCommonHelper.SubConditionQueryGet<string>(searchTbl.errorCode, "and errorCode")
                , CarSCSCommonHelper.SubConditionQueryGet<string>(searchTbl.textRegex, "and TextRegEx")
                , orderBy);
        }

        /// <summary>
        /// error map entity get by values
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="errorCode"></param>
        /// <param name="textRegex"></param>
        /// <returns></returns>
        public static ErrorMapConfigTbl TBLEntityGet(string messageType, string errorCode, string textRegex)
        {
            ErrorMapConfigTbl searchTbl = new ErrorMapConfigTbl();
            searchTbl.messageType = messageType;
            searchTbl.errorCode = errorCode;
            searchTbl.textRegex = textRegex;
            return searchTbl;
        }

        /// <summary>
        /// error map entity get by DB returns
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ErrorMapConfigTbl ErrorMapTblGet(SqlDataReader reader)
        {
            ErrorMapConfigTbl tbl = new ErrorMapConfigTbl();
            tbl.messageType = reader["MessageType"].ToString(); 
            tbl.errorCode = reader["ErrorCode"].ToString();
            tbl.textRegex = reader["TextRegEx"].ToString();
            tbl.errorNode = reader["ErrorNode"].ToString();
            tbl.message = reader["Message"].ToString();
            tbl.interruptProcessing = reader["InterruptProcessingBool"].ToString();
            tbl.logError = reader["LogErrorBool"].ToString();
            tbl.CreateDate = Convert.ToDateTime(reader["CreateDate"].ToString());
            tbl.CreatedBy = reader["CreatedBy"].ToString();
            tbl.UpdateDate = CarSCSCommonHelper.ColumnNameCheck<DateTime>(reader, "UpdateDate", DateTime.MinValue);
            tbl.updatedBy = CarSCSCommonHelper.ColumnNameCheck<string>(reader, "LastUPdatedBy", null);
            tbl.errorMapID = Convert.ToInt32(reader["ErrorMapID"].ToString());
            return tbl;
        }

        /// <summary>
        /// error map entity list get by db query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<ErrorMapConfigTbl> ErrorMapTblsGet(string query, string connectionString)
        {
            List<ErrorMapConfigTbl> tbls = new List<ErrorMapConfigTbl>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = query;
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {                    
                    tbls.Add(ErrorMapTblGet(reader));
                }
                conn.Close();
            }
            return tbls;
        }

        /// <summary>
        /// error map log entity list get by db query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<ErrorMapConfigLogTbl> ErrorMapLogTblGet(string query, string connectionString)
        {
            List<ErrorMapConfigLogTbl> tbls = new List<ErrorMapConfigLogTbl>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = query;
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                ErrorMapConfigLogTbl tbl;
                while (reader.Read())
                {
                    tbl = new ErrorMapConfigLogTbl();
                    tbl.errorMapConfig = ErrorMapTblGet(reader);
                    tbl.errorMapID = Convert.ToInt32(reader["ErrorMapLogID"].ToString());
                    tbl.auditAcationID = Convert.ToInt32(reader["AuditActionID"].ToString());
                    tbls.Add(tbl);
                }
                conn.Close();
            }
            return tbls;
        }
    }

}
