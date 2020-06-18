using System.Configuration;
using System.Data.SqlClient;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class CarSSDB
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["CarSS"].ConnectionString;
               
        public static void GetToXPathFromErrorXPathMapping(string messageNameFrom, string fromXPath
            , out string messageNameTo, out string toXPath)
        {
            messageNameTo = string.Empty;
            toXPath = string.Empty;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                // XPathFrom like '%{1}' for {$xpathBase} query
                cmd.CommandText = string.Format("select * from ErrorXPathMapping where MessageNameFrom = '{0}' and XPathFromRegex like '%{1}' "
                    , messageNameFrom, fromXPath.Replace("'", "''").Replace("[", "[[]"));

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    messageNameTo = reader["MessageNameTo"].ToString();
                    toXPath = reader["XPathToRegex"].ToString();
                }
                conn.Close();
            }
        }
    }
}
