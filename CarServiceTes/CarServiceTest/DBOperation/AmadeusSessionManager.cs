using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class AmadeusSessionManager
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["AmadeusSessionManager"].ConnectionString;

        /// <summary>
        ///  Query Session table to get OfficeID column
        /// </summary>
        public static String getOfficeIDFromSessionTbl(string sessionId)
        {
            String officeID = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select OfficeID from AmadeusSessionManager_STT.dbo.Session where SessionNbr = '{0}'", sessionId);
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    officeID = reader["OfficeID"].ToString();
                }
                conn.Close();
            }
            return officeID;
        }
    }
}
