using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class CarProductCatalogID
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["ConfigurationMaster"].ConnectionString;
        public static long GetByTPID_EAPID(int tpid, uint eapid)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                string sql = "select TravelproductID, PartnerID, WebClientConfigvalue as CarProductCatalogID" +
                    " from WebClientConfiguration a" +
                    " JOIN WebClientConfigProduct b on b.WebClientConfigID = a.WebClientConfigID" +
                    " where a.WebClientConfigCode='2mpc' and TravelproductID=" + tpid +
                    " and PartnerID=" + eapid;
                cmd.CommandText = string.Format(sql);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    long result;
                    long.TryParse(reader["CarProductCatalogID"].ToString(), out result);
                    return result;
                }
                conn.Close();
            }
            return -1;
        }

    }
}
