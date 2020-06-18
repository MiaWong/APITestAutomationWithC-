using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Expedia.CarInterface.CarServiceTest.DBOperation.DBUtil;


namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class ConfigurationMaster
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["ConfigurationMaster"].ConnectionString;

        public static uint GetCarProductCategoryIDFromTpidEapid(int tpid, uint eapid)
        {
            uint carProductCatalogID = 0;
            string sProductCatalogID = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select a.WebClientConfigValue from WebClientConfigProduct a " +
                    "join WebClientConfiguration b on a.WebClientConfigID = b.WebClientConfigID " +
                    "where a.TravelProductID = {0} and a.PartnerID = {1} and b.WebClientConfigCode = '2mpc'", tpid, eapid);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    sProductCatalogID = reader[0].ToString();
                    if (null != sProductCatalogID && sProductCatalogID.Length > 0 && int.Parse(sProductCatalogID) >= 0)
                    {
                        carProductCatalogID = uint.Parse(sProductCatalogID);
                    }
                }
                conn.Close();
            }
            return carProductCatalogID;
        }

        public static string GetCurrencyCodeFromTPID(int tpid)
        {
            string currencyCode = string.Empty;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select b.CurrencyCodeDefault from TravelProductDomain a join Country b " +
                    "on a.CountryCode = b.CountryCode where a.TravelProductID = {0} ", tpid);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    currencyCode = reader[0].ToString();
                }
                conn.Close();
            }
            return currencyCode;
        }

        public static uint GetLangIDFromTpid(int tpid)
        {
            uint langID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select LangID from LocaleProduct where TravelProductID = {0} ", tpid);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    langID = uint.Parse(reader[0].ToString());
                }
                conn.Close();
            }
            return langID;
        }

        /// <summary>
        /// Amadeus for language ID 
        /// </summary>
        public static uint GetLangIDFromTpid_TravelProductDomain(int tpid)
        {
            uint langID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format(" select langID from TravelProductDomain a join Country b "+
                   " on a.CountryCode = b.CountryCode where a.TravelProductID = {0} ", tpid);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    langID = uint.Parse(reader[0].ToString());
                }
                conn.Close();
            }
            return langID;
        }

        public static String getLanguageRecord(uint langid)
        {
            String LocaleCode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "exec LanguageWin32GetByLanguage " + langid;
                cmd.CommandTimeout = 0;

                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    LocaleCode = dr["LocaleCode"].ToString();
                }
            }
            return LocaleCode;
        }

        public static String getLanguageCode(string countryCode)
        {
            String LocaleCode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select LocaleCode from LanguageWin32 where CountryCode = '{0}'", countryCode);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    LocaleCode = reader[0].ToString();
                }
                conn.Close();
            }
            return LocaleCode;
        }

        // add by v-pxie 2012-5-3
        public static string GetCDcodeByCarItemID(uint carItemID)
        {
            String CDcode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "SELECT dbo.SupplySubSetToWorldSpanSupplierItemMap.CorporateDiscountCode " +
                        "FROM dbo.CarItemSupplySubsetRank INNER JOIN dbo.SupplySubSetToWorldSpanSupplierItemMap ON  " +
                        "dbo.CarItemSupplySubsetRank.SupplySubsetID = dbo.SupplySubSetToWorldSpanSupplierItemMap.SupplySubsetID " +
                        "WHERE (dbo.CarItemSupplySubsetRank.CarItemID = " + carItemID + ")";
                cmd.CommandTimeout = 0;

                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    CDcode = dr["CorporateDiscountCode"].ToString();
                }
            }

            return CDcode;
        }

        public static string GetVendorCodeListForCcCar(int tpid)
        {
            string cc_vendors=null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = " select WebClientConfigValue from webclientconfigproduct "
                    +"where webclientconfigid in "
                    +"(select webclientconfigid from webclientconfiguration "
                    +"where webclientconfigcode = '2cnv') "
                    + "and travelproductid = "+tpid+"and PartnerID=0" ;
                cmd.CommandTimeout = 0;

                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    cc_vendors = dr["WebClientConfigValue"].ToString();
                }
            }
            return cc_vendors;
        }

        public static string GetRCcodeByCarItemID(uint carItemID)
        {
            String RCcode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "SELECT dbo.SupplySubSetToWorldSpanSupplierItemMap.RateCode" +
                        "FROM dbo.CarItemSupplySubsetRank INNER JOIN " +
                        " dbo.SupplySubSetToWorldSpanSupplierItemMap ON  " +
                        " dbo.CarItemSupplySubsetRank.SupplySubsetID = dbo.SupplySubSetToWorldSpanSupplierItemMap.SupplySubsetID " +
                        "WHERE (dbo.CarItemSupplySubsetRank.CarItemID = " + carItemID + ")";
                cmd.CommandTimeout = 0;

                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    RCcode = dr["RateCode"].ToString();
                }
            }

            return RCcode;
        }

        public static string GetCountryCodeByCountryShortCode(string countryShortCode)
        {
            String countryCode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("select CountryCode from Country where CountryShortCode = '{0}'", countryShortCode);
                cmd.CommandTimeout = 0;

                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    countryCode = dr["CountryCode"].ToString();
                }
            }

            return countryCode;
        }

        public static string GetCountryCodeByDescription(string countryName)
        {
            String countryCode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("select CountryCode from Country where Description = '{0}'", countryName);
                cmd.CommandTimeout = 0;

                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    countryCode = dr["CountryCode"].ToString();
                }
            }

            return countryCode;
        }

        public static List<CreditCardType> GetCreditCardType()
        {
            CreditCardType creditCardType = new CreditCardType();
            List<CreditCardType> creditCardTypeList = new List<CreditCardType>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select CreditCardTypeID, Description from CreditCardType");
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    creditCardType = new CreditCardType(reader["CreditCardTypeID"], reader["Description"]);
                    creditCardTypeList.Add(creditCardType);
                }
                conn.Close();
            }
            return creditCardTypeList;
        }

        public static string GetCurrencyCodeByPOS(string JurisdictionCode, string companyCode, string managementUnitCode)
        {
            String countryCode = "";
            string databaseConfigurationMaster = CarsInventory.GetDatabaseFromConnectionString(connectionString);
            string databaseCarsInventory = CarsInventory.GetDatabaseFromConnectionString(ConfigurationManager.ConnectionStrings["CarsInventory"].ConnectionString);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("select b.CurrencyCodeDefault as CountryCode from {3}..TravelProductDomain a " + 
                    "join {3}..Country b on a.CountryCode = b.CountryCode " + 
                    "join {4}..TPIDToPoSAttributeMap c on c.TravelProductID = a.TravelProductID " +
                    "where c.CompanyCode = '{0}' and c.JurisdictionCode = '{1}' and c.ManagementUnitCode = '{2}'  and c.PartnerID = '0' ",
                    companyCode, JurisdictionCode, managementUnitCode, databaseConfigurationMaster, databaseCarsInventory);
                
                
                cmd.CommandTimeout = 0;

                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    countryCode = dr["CountryCode"].ToString();
                }
            }

            return countryCode;
        }


        public static bool CheckAirportCodeInConfigurationMasterAirportTable(string airportCode)
        {
            bool existBool = false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                String sqlstring = " select COUNT(AirportCode) from Airport where AirportCode = '{0}'";

                cmd.CommandText = string.Format(sqlstring, airportCode);
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    existBool = ((int)reader[0]) > 0 ? true : false;
                    break;
                }
                conn.Close();
            }

            return existBool;
        }

        public static string GetCountryCodeFromCountryShortCode(string countryshortcode)
        {
            string countryCode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("SELECT CountryCode from Country where countryshortcode = '{0}'", countryshortcode);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    countryCode = reader[0].ToString();
                }
                conn.Close();
            }
            return countryCode;
        }

        public static string GetCountryCodeFromPhoneCountryPrefixNbr(string PhoneCountryPrefixNbr)
        {
            string countryCode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                /**
                 * SELECT CountryCode FROM [ConfigurationMaster_STT01].[dbo].[PhoneCountry]
                 */
                cmd.CommandText = string.Format("SELECT CountryCode FROM PhoneCountry where PhoneCountryPrefixNbr = {0} ", PhoneCountryPrefixNbr);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    countryCode = reader[0].ToString();
                }
                conn.Close();
            }
            return countryCode;
        }

        public static int getDomainValueIdFromCEnumValueName(string cEnumValueName)
        {
            int domainValueID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select DomainValueID from DomainValue where DomainTypeID = 317 and CEnumValueName = '{0}'", cEnumValueName);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    domainValueID = Convert.ToInt16(reader[0]);
                }
                conn.Close();
            }
            return domainValueID;
        }
    }
}
