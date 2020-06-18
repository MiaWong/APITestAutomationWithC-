using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class SupplyDomain
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["SupplyDomain"].ConnectionString;
        private static string connectionStringMNSCS = ConfigurationManager.ConnectionStrings["CarMicronNexusSCS"].ConnectionString;
        public static uint GetSupplierIDFromVendorCode(string vendorCode)
        {
            uint supplierID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT domainvalue AS VendorSupplierID FROM externalsupplyservicedomainvaluemap " +
                    "WHERE domaintypeid=150 AND ExternalDomainValue= '{0}'", vendorCode);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    supplierID = uint.Parse(reader[0].ToString());
                }
                conn.Close();
            }

            if (supplierID == 0)
            {
            using (SqlConnection conn = new SqlConnection(connectionStringMNSCS))
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();
                    cmd.CommandText = string.Format("SELECT domainvalue AS VendorSupplierID FROM externalsupplyservicedomainvaluemap " +
                        "WHERE ExternalDomainValue= '{0}'", vendorCode);
                    cmd.CommandTimeout = 0;

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        supplierID = uint.Parse(reader[0].ToString());
                    }
                    conn.Close();
                }
            }


            return supplierID;
        }

        public static String GetVendorCodeBySupplierID(uint supplierid)
        {

            String vendorCode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT ExternalDomainValue AS VendorCode FROM externalsupplyservicedomainvaluemap WHERE domaintypeid=150 AND domainvalue='{0}'", supplierid);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    vendorCode = reader[0].ToString();
                }
                conn.Close();
            }
            return vendorCode;


        }
                
        //get supplierName basic on supplierID
        public static String GetSupplierName(int supplierId)
        {
            String supplierName = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("Select SupplierName from Supplier where SupplierID = {0} ", supplierId); 
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    supplierName = reader[0].ToString();
                }
                conn.Close();
            }
            return supplierName;
        }

        public static uint GetASCSServiceID()
        {
            uint serverID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT SupplyConnectivityServiceID  FROM SupplyConnectivityService " +
                    "WHERE SupplyConnectivityServiceName like '%{0}%'", "Amadeus");
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    serverID = uint.Parse(reader[0].ToString());
                }
                conn.Close();
            }
            return serverID;
        }
    }
}
