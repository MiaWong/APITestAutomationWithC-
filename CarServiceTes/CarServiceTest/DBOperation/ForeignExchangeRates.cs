using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class ForeignExchangeRates
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["ForeignExchangeRates"].ConnectionString;
        public static Decimal getExchangeRate(String currencyCode,string pickupDate)
        {
            Decimal exchangeRate = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select  CurrencyCode, ExchangeRate  from dbo.ExchangeRate " + 
                            "where  ExchangeRateTypeCode = 'CUR'  and CurrencyCode = '{0}' and EffectiveDateBegin <= '{1}' " +
                            "and EffectiveDateEnd   >  '{1}'", currencyCode, pickupDate);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    exchangeRate = Convert.ToDecimal(reader["ExchangeRate"]);
                }
                conn.Close();
            }
            return exchangeRate;
        }
    }
}
