using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using Expedia.CarInterface.CarServiceTest.Util;
using System.Reflection;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{    
    public class CarLog
    {

        private static string connectionString = ConfigurationManager.ConnectionStrings["CarLog"].ConnectionString;

        //The system DateTime on DB server maybe different with the system DateTime on the machine where automation is running, so adjust the log query start time using this method.
        public static DateTime GetAdjustedQueryStartTime(DateTime startTime)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                DateTime dateTimeNowDBServer = new DateTime();

                cmd.CommandText = string.Format("SELECT   Getdate()"); //This script will get the current time on DB server                   
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dateTimeNowDBServer = DateTime.Parse(reader[0].ToString());
                }
                reader.Close();

                //Get the current time on the machine automation is running
                DateTime dateTimeNow = DateTime.Now;

                //Get the difference between system DateTime on the machine automation is running and DB server
                TimeSpan adjustTime = dateTimeNowDBServer - dateTimeNow;
                //Adjust the query start time.
                startTime = startTime.Add(adjustTime);
                conn.Close();
            }
            return startTime;
        }

        public static DateTime GetCRSLogDBServerCurrentDateTime()
        {
            DateTime dateTimeNowDBServer = new DateTime();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                
                cmd.CommandText = string.Format("SELECT   Getdate()"); //This script will get the current time on DB server                   
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dateTimeNowDBServer = DateTime.Parse(reader[0].ToString());
                }
                reader.Close();
                conn.Close();
            }
            return dateTimeNowDBServer;
        }

        /*
         *equalGUID: if we want to query CRSLog which has the same OriginatingGUID as antoher CRSLog message, set this value
         *notEqualGUID: if we want to query CRSLog which has the different OriginatingGUID as antoher CRSLog message, set this value 
         *OriginatingGUID: this value will be logged the same for the same action, e.g - if we do a CarBS search, CSSR, CCSR and all the VAQs will have the same OriginatingGUID
         *Note: Different actions will have different OriginatingGUID, e.g - if one TC do a search then GetDetails, all the search messages will have one OriginatingGUID, all GetDetails messages will have another OriginatingGUID
         */
        public static List<CRSData> getCrsLogMessage(string messageType, DateTime startTime, int tuid, string description = null, bool autoAdjustTime = false,
            string equalGUID = null, string notEqualGUID = null, bool responseNeedNotNull = true, bool isRetry = true, string requestServerName = null)
        {
            List<CRSData> dataList = new List<CRSData>();
            //Thread.Sleep(2000);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //Adjust the query start time based on the difference between system DateTime on the machine automation is running and DB server.
                if (autoAdjustTime) startTime = GetAdjustedQueryStartTime(startTime);

                //If there is description in the parameters, set it as the query condition
                string transactionDescQueryString = (null != description) ? string.Format("and TransactionDesc like '%{0}%'", description) : "";

                //If OriginatingGUID is not null, set it as query condition
                string equalGUIDQueryString = (null != equalGUID) ? string.Format("and request.OriginatingGUID = '{0}'", equalGUID) : "";
                string notEqualGUIDQueryString = (null != notEqualGUID) ? string.Format("and request.OriginatingGUID <> '{0}'", notEqualGUID) : "";

                //Response not null
                string responseNotNullQueryString = responseNeedNotNull ? "and CRSResponse is not null" : "";

                //Request server name
                string reqServerNQueryString = string.IsNullOrEmpty(requestServerName) ? "" : string.Format("and RequestServerName ='{0}'", requestServerName);

                string CommandText = string.Format("select request.CRSRequest , response.CRSResponse, request.DateAdded, request.OriginatingGUID, request.CRSLogNumber, response.DateAdded as ResponseDateAdded  from dtCRSLogRequest request "
                                + "join dtCRSLogResponse response on request.CRSLogNumber = response.CRSLogNumber where request.TransactionType = '{0}'"
                    + "and request.DateAdded > '{1}' and request.TUID = {2} {3} {4} {5} {6} and CRSRequest is not null {7} order by request.CRSLogNumber desc",
                    messageType, startTime.ToString(), tuid, transactionDescQueryString, equalGUIDQueryString, notEqualGUIDQueryString, reqServerNQueryString, responseNotNullQueryString);
                Console.WriteLine("CommandText: " + CommandText);

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandTimeout = 0;
                conn.Open();
                cmd.CommandText = CommandText;
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    CRSData data = new CRSData();
                    data.CRSRequest = (byte[])reader["CRSRequest"];
                    data.CRSResponse = (null == reader["CRSResponse"] || reader["CRSResponse"].ToString().Length == 0) ? null : (byte[])reader["CRSResponse"];
                    data.OriginatingGUID = reader["OriginatingGUID"].ToString();
                    data.dateAdded = reader["DateAdded"].ToString();
                    data.CRSLogNumber = reader["CRSLogNumber"].ToString();
                    data.ResponseDateAdded = reader["ResponseDateAdded"].ToString();
                    dataList.Add(data);
                }

                conn.Close();
            }

            if (!isRetry)
            {
                return dataList;
            }
            else
            {
                var para = new object[] { messageType, startTime, tuid, description, false, equalGUID, notEqualGUID, responseNeedNotNull, isRetry, requestServerName };
                object[] paraNext = new object[] { messageType, startTime.AddSeconds(-1 * RetryLogic.Instance.RetryIncrementSecondsGet()), tuid, description, false, equalGUID, notEqualGUID, responseNeedNotNull, isRetry, requestServerName };
                return RetryLogic.Instance.CheckRetry<List<CRSData>>(dataList, MethodBase.GetCurrentMethod(), para, paraNext);
            }
        }

        public static List<CRSData> getCrsLogMessage(string messageType, DateTime startTime, string tuid = null, string description = null, bool autoAdjustTime = false,
            string equalGUID = null, string notEqualGUID = null, bool responseNeedNotNull = true, bool isRetry = true, string crs = null, bool noError = false)
        {
            List<CRSData> dataList = new List<CRSData>();
            //Thread.Sleep(2000);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //Adjust the query start time based on the difference between system DateTime on the machine automation is running and DB server.
                if (autoAdjustTime) startTime = GetAdjustedQueryStartTime(startTime);

                string equalTuidQueryString = (null != tuid) ? string.Format(" and request.TUID ={0}", tuid) : "";
                //If there is description in the parameters, set it as the query condition
                string transactionDescQueryString = (null != description) ? string.Format("and TransactionDesc like '%{0}%'", description) : "";
                string crsIDQuerytring = (null != crs) ? string.Format("and crsID like '%{0}%'", crs) : "";

                //If OriginatingGUID is not null, set it as query condition
                string equalGUIDQueryString = (null != equalGUID) ? string.Format("and request.OriginatingGUID = '{0}'", equalGUID) : "";
                string notEqualGUIDQueryString = (null != notEqualGUID) ? string.Format("and request.OriginatingGUID <> '{0}'", notEqualGUID) : "";

                string responseNotNullQueryString = responseNeedNotNull ? "and CRSResponse is not null" : "";

                string errorQueryString = noError ? "AND response.ErrorFlag = 0" : "";

                string CommandText = string.Format("select request.CRSRequest , response.CRSResponse, request.DateAdded, request.OriginatingGUID, request.CRSLogNumber, response.DateAdded as ResponseDateAdded  from dtCRSLogRequest request "
                                + "join dtCRSLogResponse response on request.CRSLogNumber = response.CRSLogNumber where request.TransactionType = '{0}'and request.DateAdded > '{1}'  {2} {3} {4} {5} {6} {7} {8} order by request.CRSLogNumber desc",
                                messageType, startTime.ToString(), equalTuidQueryString, crsIDQuerytring, transactionDescQueryString, equalGUIDQueryString, notEqualGUIDQueryString, errorQueryString, responseNotNullQueryString);

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandTimeout = 0;
                conn.Open();
                cmd.CommandText = CommandText;
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    CRSData data = new CRSData();
                    data.CRSRequest = (byte[])reader["CRSRequest"];
                    data.CRSResponse = (null == reader["CRSResponse"] || reader["CRSResponse"].ToString().Length == 0) ? null : (byte[])reader["CRSResponse"];
                    data.OriginatingGUID = reader["OriginatingGUID"].ToString();
                    data.dateAdded = reader["DateAdded"].ToString();
                    data.CRSLogNumber = reader["CRSLogNumber"].ToString();
                    data.ResponseDateAdded = reader["ResponseDateAdded"].ToString();
                    dataList.Add(data);
                }

                conn.Close();
            }

            if (!isRetry)
            {
                return dataList;
            }
            else
            {
                var para = new object[] { messageType, startTime, tuid, description, false, equalGUID, notEqualGUID, responseNeedNotNull, isRetry, crs, noError };
                var paraNext = new object[] { messageType, startTime.AddSeconds(-1 * RetryLogic.Instance.RetryIncrementSecondsGet()), tuid, description, false, equalGUID, notEqualGUID, responseNeedNotNull, isRetry, crs, noError };                
                return RetryLogic.Instance.CheckRetry<List<CRSData>>(dataList, MethodBase.GetCurrentMethod(), para, paraNext);
            }
        }

        public static List<CRSData> getCrsLogMessage_OnlyRequest(string messageType, DateTime startTime, string tuid = null, bool autoAdjustTime = false, bool isRetry = true)
        {
            List<CRSData> dataList = new List<CRSData>();
            //Thread.Sleep(2000);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //Adjust the query start time based on the difference between system DateTime on the machine automation is running and DB server.
                if (autoAdjustTime) startTime = GetAdjustedQueryStartTime(startTime);

                string equalTuidQueryString = (null != tuid) ? string.Format(" and TUID ={0}", tuid) : "";

                string CommandText = string.Format("select CRSRequest,OriginatingGUID, DateAdded,CRSLogNumber from dtCRSLogRequest where TransactionType = '{0}' and DateAdded > '{1}' {2}",
                    messageType, startTime, equalTuidQueryString);

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandTimeout = 0;
                conn.Open();
                cmd.CommandText = CommandText;
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    CRSData data = new CRSData();
                    data.CRSRequest = (byte[])reader["CRSRequest"];
                    data.CRSResponse = null;
                    data.OriginatingGUID = reader["OriginatingGUID"].ToString();
                    data.dateAdded = reader["DateAdded"].ToString();
                    data.CRSLogNumber = reader["CRSLogNumber"].ToString();
                    data.ResponseDateAdded = null;
                    dataList.Add(data);
                }

                conn.Close();
            }

            if (!isRetry)
            {
                return dataList;
            }
            else
            {
                var para = new object[] { messageType, startTime, tuid, false, isRetry };
                var paraNext = new object[] { messageType, startTime.AddSeconds(-1 * RetryLogic.Instance.RetryIncrementSecondsGet()), tuid, false, isRetry };
                return RetryLogic.Instance.CheckRetry<List<CRSData>>(dataList, MethodBase.GetCurrentMethod(), para, paraNext);
            }
        }
        public static List<CRSData> getCrsLogMessage(string messageType, DateTime startTime, DateTime endTime, string CRSDBConnectionStr = null, bool isRetry = true, string TransactionDesc = null)
        {
            List<CRSData> dataList = new List<CRSData>();
            //Thread.Sleep(2000);
            if (null == CRSDBConnectionStr || CRSDBConnectionStr.Length == 0)
            {
                CRSDBConnectionStr = connectionString;
            }
            using (SqlConnection conn = new SqlConnection(CRSDBConnectionStr))
            {
                string CommandText = string.Format("select request.CRSLogNumber, request.CRSRequest , response.CRSResponse, request.DateAdded, request.OriginatingGUID, travelServer.Name, request.WebServerCluster from dtCRSLogRequest request "
                                + "join dtCRSLogResponse response on request.CRSLogNumber = response.CRSLogNumber "
                                + "join dmTravelServer travelServer on travelServer.TravelServer = request.TravelServer "
                                + "where request.TransactionType = '{0}' and request.DateAdded > '{1}' and request.DateAdded < '{2}' and CRSRequest is not null and CRSResponse is not null ", messageType, startTime.ToString(), endTime.ToString());
                if (null != TransactionDesc)
                    CommandText += string.Format(" and TransactionDesc like '{0}%'", TransactionDesc);

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandTimeout = 0;
                conn.Open();
                cmd.CommandText = CommandText;
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    CRSData data = new CRSData();
                    data.CRSLogNumber = reader["CRSLogNumber"].ToString();
                    data.CRSRequest = (byte[])reader["CRSRequest"];
                    data.CRSResponse = (byte[])reader["CRSResponse"];
                    data.OriginatingGUID = reader["OriginatingGUID"].ToString();
                    data.dateAdded = reader["DateAdded"].ToString();
                    data.TravelServer = reader["Name"].ToString();
                    data.TPID = int.Parse(reader["WebServerCluster"].ToString());
                    dataList.Add(data);
                }

                conn.Close();
            }

            if (!isRetry)
            {
                return dataList;
            }
            else
            {
                var para = new object[] { messageType, startTime, endTime, CRSDBConnectionStr, isRetry};
                var paraNext = new object[] { messageType
                    , startTime.AddSeconds(-1 * RetryLogic.Instance.RetryIncrementSecondsGet())
                    , endTime.AddSeconds(RetryLogic.Instance.RetryIncrementSecondsGet())
                    , CRSDBConnectionStr, isRetry };
                return RetryLogic.Instance.CheckRetry<List<CRSData>>(dataList, MethodBase.GetCurrentMethod(), para, paraNext);
            }
        }

        public static List<CRSData> getCrsLogReq_ReqString(string messageType, DateTime startTime, string requestS, bool autoAdjustTime = true, bool isRetry = true)
        {
            List<CRSData> dataList = new List<CRSData>();
            //Thread.Sleep(2000);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //Adjust the query start time based on the difference between system DateTime on the machine automation is running and DB server.
                if (autoAdjustTime) startTime = GetAdjustedQueryStartTime(startTime);

                string CommandText = string.Format("select CRSRequest, DateAdded, OriginatingGUID, CRSLogNumber from dtCRSLogRequest where TransactionType = '{0}'"
                     + "and DateAdded > '{1}' and CRSRequest like   '%{2}%' order by CRSLogNumber asc",
                    messageType, startTime.ToString(), requestS);
                Console.WriteLine("CommandText: " + CommandText);

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandTimeout = 0;
                conn.Open();
                cmd.CommandText = CommandText;
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    CRSData data = new CRSData();
                    data.CRSRequest = (byte[])reader["CRSRequest"];                    
                    data.OriginatingGUID = reader["OriginatingGUID"].ToString();
                    data.dateAdded = reader["DateAdded"].ToString();
                    data.CRSLogNumber = reader["CRSLogNumber"].ToString();                    
                    dataList.Add(data);
                }

                conn.Close();
            }

            if (!isRetry)
            {
                return dataList;
            }
            else
            {
                var para = new object[] { messageType, startTime, requestS, false, isRetry};
                var paraNext = new object[] { messageType, startTime.AddSeconds(-1 * RetryLogic.Instance.RetryIncrementSecondsGet()), requestS, false, isRetry };
                return RetryLogic.Instance.CheckRetry<List<CRSData>>(dataList, MethodBase.GetCurrentMethod(), para, paraNext);
            }
        }

        //Get latest CrsLog time added to query message after that, example query script:
        //select top 1 (DateAdded) from dtCRSLogRequest where TUID = 61160005 and TransactionType = 'CSSR' order by CRSLogNumber desc
        public static string getLatestCrsLogTime(string messageType, uint tuid, bool isRetry = true)
        {
            string dateAdded = "";
            //Thread.Sleep(2000);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string CommandText = string.Format("select top 1 (DateAdded) from dtCRSLogRequest where TUID = {0} and TransactionType = '{1}'"
                 + "order by CRSLogNumber desc ", tuid, messageType);
                // cmd.CommandTimeout = 0;
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = CommandText;
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dateAdded = reader[0].ToString();
                    DateTime dateAdjust = DateTime.Parse(dateAdded);
                    //Adjust 1 sec because 2012-08-08 16:21:24.353 will be taken as 2012-08-08 16:21:24
                    dateAdjust = dateAdjust.AddSeconds(1);
                    dateAdded = dateAdjust.ToString();
                }
                conn.Close();
            }

            if (!isRetry)
            {
                return dateAdded;
            }
            else
            {
                var para = new object[] { messageType, tuid, isRetry};
                return RetryLogic.Instance.CheckRetry<string>(dateAdded, MethodBase.GetCurrentMethod(), para);
            }
        }

        public static List<CRSData> GetCRSLogMessageWithTimeDuration(string messageType, DateTime startTime, DateTime endTime, int tuid, string description = null, bool isRetry = true)
        {
            List<CRSData> dataList = new List<CRSData>();
            //Thread.Sleep(2000);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //If there is description in the parameters, set it as the query condition
                string transactionDescQueryString = (null != description) ? string.Format("and TransactionDesc like '%{0}%'", description) : "";

                string CommandText = string.Format("select request.CRSRequest , response.CRSResponse,request.TransactionDesc, request.DateAdded, request.OriginatingGUID, request.CRSLogNumber,"
                    + " response.DateAdded as ResponseDateAdded  from dtCRSLogRequest request "
                    + "join dtCRSLogResponse response on request.CRSLogNumber = response.CRSLogNumber where request.TransactionType = '{0}'"
                    + "and request.DateAdded > '{1}' and request.DateAdded < '{2}' and request.TUID = {3} {4} and CRSRequest is not null  order by request.CRSLogNumber desc",
                    messageType, startTime.ToString(), endTime.ToString(), tuid, transactionDescQueryString);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandTimeout = 0;
                conn.Open();
                cmd.CommandText = CommandText;
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    CRSData data = new CRSData();
                    data.CRSRequest = (byte[])reader["CRSRequest"];
                    data.CRSResponse = (byte[])reader["CRSResponse"];
                    data.OriginatingGUID = reader["OriginatingGUID"].ToString();
                    data.dateAdded = reader["DateAdded"].ToString();
                    data.TransactionDesc = reader["TransactionDesc"].ToString();
                    data.CRSLogNumber = reader["CRSLogNumber"].ToString();
                    dataList.Add(data);
                }

                conn.Close();
            }

            if (!isRetry)
            {
                return dataList;
            }
            else
            {
                var para = new object[] { messageType, startTime, endTime, tuid, description, isRetry };
                var paraNext = new object[] { messageType
                    , startTime.AddSeconds(-1 * RetryLogic.Instance.RetryIncrementSecondsGet())
                    , endTime.AddSeconds(RetryLogic.Instance.RetryIncrementSecondsGet())
                    , tuid, description, isRetry };
                return RetryLogic.Instance.CheckRetry<List<CRSData>>(dataList, MethodBase.GetCurrentMethod(), para, paraNext);
            }
        }
    }
}
