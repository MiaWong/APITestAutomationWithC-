using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.PlaceTypes.V4;
using Expedia.CarInterface.CarServiceTest.DBOperation.DBUtil;
using Expedia.CarInterface.CarServiceTest.Util;
using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.CarTypes.V5;
using Expedia.CarInterface.CarServiceTest.ExceptionFacade;
using System.Text.RegularExpressions;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class CarsInventory
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["CarsInventory"].ConnectionString;
        public static uint GetSupplierIDFromCarItemID(uint carItemID)
        {
            uint supplierID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("Select SupplierID from CarItem where CarItemID = {0}", carItemID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    supplierID = uint.Parse(reader[0].ToString());
                }
                conn.Close();
            }
            return supplierID;
        }

        public static string GetVendorCodeFromCarVendor(uint supplierID)
        {
            string vendorCode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("Select CarVendorCode from CarVendor where SupplierID = {0}", supplierID);
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

        public static List<string> GetVendorCodeFromSupplierIDs(string supplierIDsStr)
        {
            List<string> vendorCode = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("Select CarVendorCode from CarVendor where SupplierID in ({0})", supplierIDsStr);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    vendorCode.Add(reader[0].ToString());
                }
                conn.Close();
            }
            return vendorCode;
        }

        /**
     * Get point of sale record.
     * @param tpid
     * @return PointOfSaleKeyType
     * @throws ClassNotFoundException
     * @throws SQLException
     */
        public static List<PointOfSaleKey> getPointOfSaleRecord(int tpid)
        {

            List<PointOfSaleKey> result = new List<PointOfSaleKey>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                //  cmd.CommandText = string.Format("SELECT CountryCode as jurisdictionCountryCode, CompanyID as CompanyCode, OrganizationID as ManagementUnitCode FROM TPIDToPoSAttributeMap WHERE TravelProductID = {0}", tpid);
                cmd.CommandText = string.Format("SELECT jurisdictionCode as CountryCode, CompanyCode as CompanyID, ManagementUnitCode as OrganizationID FROM TPIDToPoSAttributeMap WHERE TravelProductID  = {0}", tpid);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PointOfSaleKey pointOfSaleKey = new PointOfSaleKey();
                    pointOfSaleKey.JurisdictionCountryCode = reader[0].ToString();
                    pointOfSaleKey.CompanyCode = reader[1].ToString();
                    pointOfSaleKey.ManagementUnitCode = reader[2].ToString();
                    result.Add(pointOfSaleKey);
                }
                conn.Close();
            }
            return result;
        }

        public static int GetCarBusinessModelIDFromCarItem(uint carItemID)
        {
            int carBusinessModelID = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select CarBusinessModelID from CarItem where CarItemID = {0}", carItemID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carBusinessModelID = Convert.ToUInt16(reader[0].ToString());
                }
                conn.Close();
            }
            return carBusinessModelID;
        }

        public static string GetCarStandaloneBoolFromCarItem(uint carItemID)
        {
            string carStandaloneBool = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select StandaloneBool from CarItem where CarItemID = {0}", carItemID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carStandaloneBool = reader[0].ToString();
                }
                conn.Close();
            }
            return carStandaloneBool;
        }

        public static DataTable GetCarItemIDForCarSS(CarItemDBQueryInputsFromConfigs carItemDBQueryInputsFromConfigs)
        {
            string database = GetDatabaseFromConnectionString(connectionString);
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT distinct ci.CarItemID FROM {0}..CarProductCatalogChildCarItem cpccci " +
                    " join {1}..CarItem ci on ci.CarItemID=cpccci.CarItemID and ci.ActiveBool=1 " +
                    "and ci.ActiveBool=1 and cpccci.ActiveBool=1 and ci.StandaloneBool = {2} " +
                    "join  {3}..CarProductCatalogChild cpcc on cpccci.CarProductCatalogChildID=cpcc.CarProductCatalogChildID " +
                    "and cpccci.ActiveBool=1 and cpcc.ActiveBool=1 and cpcc.CarProductCatalogID= {4} " +
                    "and cpcc.FlightOption = {5} and cpcc.HotelOption in ({6}) and cpcc.OneWayBool = {7} " +
                    "and cpcc.OnAirBool = {8} and cpcc.PickupCountryCode in ({9}) order by ci.CarItemID desc ",
                    database, database, carItemDBQueryInputsFromConfigs.standaloneBool, database, carItemDBQueryInputsFromConfigs.carProductCatalogID,
                    carItemDBQueryInputsFromConfigs.flightOption, carItemDBQueryInputsFromConfigs.hotelOption, carItemDBQueryInputsFromConfigs.oneWayBool,
                    carItemDBQueryInputsFromConfigs.onAirportBool, carItemDBQueryInputsFromConfigs.pickupCountryCode);
                Console.WriteLine("cmd.CommandText: " + cmd.CommandText);
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }
        
        public static DataTable GetCarItemIDForCarSS(long carProductCatalogID, string pickupCountryCode, int oneWayBool, int onAirportBool,
            int standaloneBool, int flightOption, String hotelOption)
        {
            string database = GetDatabaseFromConnectionString(connectionString);
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT distinct ci.CarItemID FROM {0}..CarProductCatalogChildCarItem cpccci " +
                    " join {1}..CarItem ci on ci.CarItemID=cpccci.CarItemID and ci.ActiveBool=1 " +
                    "and ci.ActiveBool=1 and cpccci.ActiveBool=1 and ci.StandaloneBool = {2} " +
                    "join  {3}..CarProductCatalogChild cpcc on cpccci.CarProductCatalogChildID=cpcc.CarProductCatalogChildID " +
                    "and cpccci.ActiveBool=1 and cpcc.ActiveBool=1 and cpcc.CarProductCatalogID= {4} " +
                    "and cpcc.FlightOption = {5} and cpcc.HotelOption in ({6}) and cpcc.OneWayBool = {7} " +
                    "and cpcc.OnAirBool = {8} and cpcc.PickupCountryCode in ({9}) order by ci.CarItemID desc ",
                    database, database, standaloneBool, database, carProductCatalogID,
                   flightOption, hotelOption, oneWayBool,
                    onAirportBool, pickupCountryCode);
                Console.WriteLine("cmd.CommandText: " + cmd.CommandText);
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        //Mehtod GetCarItemIDForCarSS is to get all the CarItemID list from DB based on config excpet vendorCode, this method is to get CarItemID based on SupplierID
        public static string GetCarItemIDByCarItemIDListAndSupplierID(string carItemIDString, uint supplierID)
        {
            //select ci.CarItemID from CarItem ci join
            //CarItemSupplySubsetRank cissr on
            //ci.CarItemID = cissr.CarItemID
            //and ci.CarItemID in (39642)
            //and SupplierID = 41
            string carItemID = "";
            string database = GetDatabaseFromConnectionString(connectionString);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select ci.CarItemID from CarItem ci join CarItemSupplySubsetRank cissr on" +
                    " ci.CarItemID = cissr.CarItemID and ci.CarItemID in ({0}) and SupplierID = {1}", carItemIDString, supplierID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carItemID = reader["CarItemID"].ToString();
                }
                conn.Close();
            }
            return carItemID;
        }

        
        //For offairport car search, no CarItemID list will be needed, 
        //just get the CarItemID by vendorSupplierID in OffairportLocationKey under Expedia.CarInterface.CarServiceIntgTest.TestValues
        public static uint GetCarItemIDForCarSS_Offairport(long carProductCatalogID, string pickupCountryCode, int oneWayBool, int onAirportBool,
            int standaloneBool, int flightOption, String hotelOption, uint vendorSupplierID)
        {
            uint carItemID = 0;
            string database = GetDatabaseFromConnectionString(connectionString);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT ci.CarItemID FROM {0}..CarProductCatalogChildCarItem cpccci " +
                    " join {1}..CarItem ci on ci.CarItemID=cpccci.CarItemID and ci.ActiveBool=1 " +
                    "and ci.ActiveBool=1 and cpccci.ActiveBool=1 and ci.StandaloneBool = {2} " +
                    "join  {3}..CarProductCatalogChild cpcc on cpccci.CarProductCatalogChildID=cpcc.CarProductCatalogChildID " +
                    "and cpccci.ActiveBool=1 and cpcc.ActiveBool=1 and cpcc.CarProductCatalogID= {4} " +
                    "and cpcc.FlightOption = {5} and cpcc.HotelOption in ({6}) and cpcc.OneWayBool = {7} " +
                    "and cpcc.OnAirBool = {8} and cpcc.PickupCountryCode in ({9}) and SupplierID = {10} ",
                    database, database, standaloneBool, database, carProductCatalogID,
                    flightOption, hotelOption, oneWayBool,
                    onAirportBool, pickupCountryCode, vendorSupplierID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carItemID = Convert.ToUInt32(reader["CarItemID"].ToString());
                }
                conn.Close();
            }
            return carItemID;
        }

        public static String GetCountryCode(String airportCode)
        {
            string countryCode = string.Empty;
            string database = GetDatabaseFromConnectionString(connectionString);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select CountryCode from {0}..Airport where airportCode = '{1}' ", database, airportCode);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return countryCode = reader["CountryCode"].ToString();
                }
                conn.Close();
            }
            return null;
        }

        public static string GetDatabaseFromConnectionString(string connectionString)
        {
            string[] split = connectionString.Split(new Char[] { ';' });
            string[] split2 = split[1].Split(new Char[] { '=' });
            string database = split2[1];
            return database;
        }

        public static DataTable getChildCarItemIDs(String carItemIDString)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select ChildCarItemID as CarItemID from {0}..CarItemParentChild where ParentCarItemID in ({1}) " +
                    "and ChildCarItemID not in (1,2) ", GetDatabaseFromConnectionString(connectionString), carItemIDString);
                cmd.CommandTimeout = 0;
                Console.WriteLine("cmd.CommandText: " + cmd.CommandText);

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        #region for Amadeus
        public static List<String> GetSupplySubSetIDS_ForACSC()
        {
            //Get ASCS service ID
            uint ASCS_ServerID = SupplyDomain.GetASCSServiceID();
            List<String> subSetIDlist = new List<String>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("select distinct SupplySubsetID from SupplySubsetToSupplyConnectivityServiceMap"
                    + " where SupplyConnectivityServiceID = {0} ", ASCS_ServerID);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    subSetIDlist.Add(reader[0].ToString());
                }
                conn.Close();
            }
            return subSetIDlist;
        }

        public static List<uint> GetCarItemWithSupplierSubsetID(List<string> subsetIDList)
        {

            string subsetIDString = string.Empty;
            int i = 0;
            foreach (string subsetID in subsetIDList)
            {
                subsetIDString += subsetID;
                if (++i != subsetIDList.Count)
                    subsetIDString += ",";
            }
            List<uint> caritemList = new List<uint>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("select CaritemID from caritem where CarItemID "
                    + " in( select CarItemID from CarItemSupplySubsetRank where SupplySubsetID in({0})) ", subsetIDString);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    caritemList.Add(uint.Parse(reader[0].ToString()));
                }
                conn.Close();
                return caritemList;
            }
        }
        #endregion

        public static uint GetSupplyIDByVendorCode(string VendorCode)
        {
            uint suplierID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("Select SupplierID from CarVendor where CarVendorCode = '{0}'", VendorCode);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    suplierID = uint.Parse((reader[0].ToString()));
                }
                conn.Close();
            }
            return suplierID;
        }

        public static DataTable GetSupplySubSetIDs(String carItemIDString)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format(" select cissr.SupplySubsetID,ci.CarBusinessModelID, ci.SupplierID " +
                "from {0}..CarItemSupplySubsetRank cissr join {0}..caritem ci on cissr.CarItemID=ci.CarItemID " +
                " and cissr.CarItemID in ({1}) order by ci.SupplierID asc ", GetDatabaseFromConnectionString(connectionString), carItemIDString);

                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        //Get SI and VO format from table CarConfigurationFormat based on SupplySubSetID
        public static DataTable GetSIVOFormatFromSupplySubSetID(uint supplySubSetID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format(" select CarSupplementalInfoFormat, CarVoucherNumberFormat from CarConfigurationFormat " +
                "ccf join SupplySubsetToCarConfigurationFormatMap scf on ccf.CarConfigurationFormatID = scf.CarConfigurationFormatID " +
                " and scf.SupplySubsetID = {0} ", supplySubSetID);

                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        public static string GetRateCodeFormatFromSupplySubSetID(uint supplySubSetID)
        {
            string rateCodeFormat = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format(" select CarRateCodeFormat from CarConfigurationFormat " +
                "ccf join SupplySubsetToCarConfigurationFormatMap scf on ccf.CarConfigurationFormatID = scf.CarConfigurationFormatID " +
                " and scf.SupplySubsetID = {0} ", supplySubSetID);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    rateCodeFormat = reader["CarRateCodeFormat"].ToString();
                }
                conn.Close();
            }
            return rateCodeFormat;
        }

        public static string GetCorpDiscFormatFromSupplySubSetID(uint supplySubSetID)
        {
            string corpDiscFormat = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format(" select CarCorpDiscFormat from CarConfigurationFormat " +
                "ccf join SupplySubsetToCarConfigurationFormatMap scf on ccf.CarConfigurationFormatID = scf.CarConfigurationFormatID " +
                " and scf.SupplySubsetID = {0} ", supplySubSetID);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    corpDiscFormat = reader["CarCorpDiscFormat"].ToString();
                }
                conn.Close();
            }
            return corpDiscFormat;
        }

        public static string GetTourCodeFormatFromSupplySubSetID(uint supplySubSetID)
        {
            string tourCodeFormat = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format(" select CarTourIDFormat from CarConfigurationFormat " +
                "ccf join SupplySubsetToCarConfigurationFormatMap scf on ccf.CarConfigurationFormatID = scf.CarConfigurationFormatID " +
                " and scf.SupplySubsetID = {0} ", supplySubSetID);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tourCodeFormat = reader["CarTourIDFormat"].ToString();
                }
                conn.Close();
            }
            return tourCodeFormat;
        }

        public static string GetAccountingVendorIDFromCarItemID(uint carItemID)
        {
            string accountingVendorID = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format(" select AccountingVendorID from CarItem where CarItemID = {0} ", carItemID);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    accountingVendorID = reader["AccountingVendorID"].ToString();
                }
                conn.Close();
            }
            return accountingVendorID;
        }

        public static uint GetSupplySubSetIDFromCarItemID(uint CarItemID)
        {
            uint supplySubsetID = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format(" select cissr.SupplySubsetID " +
                "from {0}..CarItemSupplySubsetRank cissr join {0}..caritem ci on cissr.CarItemID=ci.CarItemID " +
                " and cissr.CarItemID = {1} ", GetDatabaseFromConnectionString(connectionString), CarItemID);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    supplySubsetID = Convert.ToUInt32(reader["SupplySubsetID"].ToString());
                }
                conn.Close();
            }
            return supplySubsetID;
        }

        //Get the serviceID for Cost from SupplySubsetToSupplyConnectivityServiceMap based on SupplySubsetID
        //select SupplyConnectivityServiceID from SupplySubsetToSupplyConnectivityServiceMap 
        //where SupplySubsetID = 9270 and SupplyRoutingCategoryID = 1 (Cost)
        //ServiceID 1: WSCS 2. ESCS 3. MNSCS
        public static int GetServiceIDForSupplySubsetID_Cost(uint supplySubsetID)
        {
            int serviceID = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("select SupplyConnectivityServiceID from SupplySubsetToSupplyConnectivityServiceMap " +
                "where SupplySubsetID = {0} and SupplyRoutingCategoryID = 1 ", supplySubsetID);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    serviceID = Convert.ToInt32(reader["SupplyConnectivityServiceID"].ToString());
                }
                conn.Close();
            }
            return serviceID;
        }

        //Get the serviceID for Avail from SupplySubsetToSupplyConnectivityServiceMap based on SupplySubsetID
        //select SupplyConnectivityServiceID from SupplySubsetToSupplyConnectivityServiceMap 
        //where SupplySubsetID = 9270 and SupplyRoutingCategoryID = 1 (Avail)
        //ServiceID 1: WSCS 2. ESCS 3. MNSCS
        public static int GetServiceIDForSupplySubsetID_Avail(uint supplySubsetID)
        {
            int serviceID = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("select SupplyConnectivityServiceID from SupplySubsetToSupplyConnectivityServiceMap " +
                "where SupplySubsetID = {0} and SupplyRoutingCategoryID = 2 ", supplySubsetID);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    serviceID = Convert.ToInt32(reader["SupplyConnectivityServiceID"].ToString());
                }
                conn.Close();
            }
            return serviceID;
        }


        public static uint GetCarAgreementIDFromSupplySubSetID(uint SupplySubSetID)
        {
            uint carAgreementID = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format(" select CarAgreementID from SupplySubsetToCarAgreementMap where SupplySubsetID = {0} ", SupplySubSetID);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carAgreementID = Convert.ToUInt32(reader["CarAgreementID"].ToString());
                }
                conn.Close();
            }
            return carAgreementID;
        }

        //Select GDSPVoucherEnabledSupplierIDs from CarProductCatalogConfiguration where CarProductCatalogID = 2
        public static List<uint> GetGDSPVoucherEnabledSupplierIDsFromProductCatalogID(uint carProductCatalogID)
        {
            List<uint> GDSPVoucherEnabledSupplierIDs = new List<uint>();
            string GDSPVoucherEnabledSupplierIDString = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = String.Format("Select GDSPVoucherEnabledSupplierIDs from CarProductCatalogConfiguration where CarProductCatalogID = {0} ", carProductCatalogID);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GDSPVoucherEnabledSupplierIDString = reader["GDSPVoucherEnabledSupplierIDs"].ToString();
                }
                conn.Close();
            }
            if (GDSPVoucherEnabledSupplierIDString != "")
            {
                string[] split = GDSPVoucherEnabledSupplierIDString.Split(new Char[] { ',' });
                foreach (string splitString in split)
                {
                    GDSPVoucherEnabledSupplierIDs.Add(Convert.ToUInt32(splitString));
                }
            }
            return GDSPVoucherEnabledSupplierIDs;
        }

        //Select the CarVendorLocationCode, Latitude and Longitude from table CarVendorLocation to query all the CarVendorLocation information based on AirportCode
        public static DataTable GetCarVendorLocationInfoFromAirportCode(string airportCode)
        {
            string database = GetDatabaseFromConnectionString(connectionString);
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                //Ustory 111626 DB Work for migration of VendorCode to use SupplierID   -- modified by v-pxie 2012-7-16
                string supportVendorCode = CarConfigurationManager.AppSettingFromXml("SurportVendrCode") == null ? "false" : CarConfigurationManager.AppSettingFromXml("SurportVendrCode");
                if (supportVendorCode.Equals("true"))
                {
                    //Add statusCode=5 - edit by Qiuhua 2013-12-04
                    cmd.CommandText = string.Format("select CarVendorLocationCode, CarVendorCode, Latitude, Longitude, CarVendorLocationID from CarVendorLocation "
                + "where AirportCode = '{0}' and (StatusCode = 'A' or StatusCode = '5')", airportCode);
                }
                else
                {
                    cmd.CommandText = string.Format("select CarVendorLocationCode, SupplierID, Latitude, Longitude, CarVendorLocationID from CarVendorLocation "
                + "where AirportCode = '{0}' and (StatusCode = 'A' or StatusCode = '5')", airportCode);// and  StatusCode = 'A'
                }

                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        //select StreetAddress1,CityName,StateProvinceName,ISOCountryCode from CarVendorLocation where AirportCode = 'PAR' and CarVendorLocationCode = 'X019'and SupplierID = 41
        public static DataTable GetCarVendorLocationsFromLocationCodeAndSupplierID(string locationCode, string locationCategoryCode, string supplierRawText, uint supplierID)
        {
            string database = GetDatabaseFromConnectionString(connectionString);
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select StreetAddress1,CityName,StateProvinceName,ISOCountryCode from CarVendorLocation where AirportCode = '{0}' "
                + "and CarVendorLocationCode = '{1}'and SupplierID = {2}", locationCode, locationCategoryCode + supplierRawText, supplierID);

                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        //get carVendorLocations From supplierId and latitude latitude offset.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierID"></param>
        /// <param name="latitude"></param>
        /// <param name="latOffset">value 0 will not search by latitude and offset.</param>
        /// <returns></returns>
        public static List<CarVendorlocation> GetCarVendorLocationsFromTiSupplierIDAndRangeOfLatitude(int supplierID, double latitude, double latOffset)
        {
            string database = GetDatabaseFromConnectionString(connectionString);
            List<CarVendorlocation> locations = new List<CarVendorlocation>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                string latitudeCondition = "";
                if (latOffset != 0)
                {
                    latitudeCondition = string.Format("and Latitude < {0} and Latitude > {1}",latitude+latOffset, latitude-latOffset);
                }

                cmd.CommandText = string.Format("select CarVendorLocationID,AirportCode,CarVendorLocationCode,"+
                    "CarVendorLocationName,StreetAddress1,CityName,StateProvinceCode,StateProvinceName,ISOCountryCode,"+
                    "Latitude,Longitude,StatusCode,UpdateDate,LastUpdatedBy,LocationTypeID,SupplierID,CarShuttleCategoryID,"+
                    "PostalCode,PhoneNumber,FaxNumber,DeliveryBool,CollectionBool,OutOfOfficeHoursBool from CarVendorLocation"+
                    " where SupplierID = {0} {1}", supplierID, latitudeCondition);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CarVendorlocation loc = new CarVendorlocation();
                    loc.CarVendorLocationID = Convert.ToInt32(reader[0].ToString());
                    loc.AirportCode = reader[1].ToString();
                    loc.CarVendorLocationCode = reader[2].ToString();
                    loc.CarVendorLocationName = reader[3].ToString();
                    loc.StreeAddress1 = reader[4].ToString();
                    loc.CityName = reader[5].ToString();
                    loc.StateProvinceCode = reader[6].ToString();
                    loc.StateProvinceName = reader[7].ToString();
                    loc.ISOCountryCode = reader[8].ToString();
                    loc.Latitude = Double.Parse(reader[9].ToString());
                    loc.Longitude = Double.Parse(reader[10].ToString());
                    loc.StatusCode = reader[11].ToString();
                    loc.UpdateDate = reader[12].ToString();
                    loc.LastUpdatedBy = reader[13].ToString();
                    loc.LocationTypeID = reader[14].ToString();
                    loc.SupplierID = Convert.ToInt32(reader[15]);
                   // loc.CarShuttleCategoryID = reader[16].ToString();
                    loc.PostalCode = reader[17].ToString();//,,
                    loc.PhoneNumber = reader[18].ToString();
                    loc.FaxNumber = reader[19].ToString();
                   // loc.DeliveryBool = reader[20].ToString();
                   // loc.CollectionBool = reader[21].ToString();
                  //  loc.OutOfOfficeHoursBool = reader[22].ToString();

                    locations.Add(loc);
                }
                conn.Close();
            }
            return locations;
        }

        //get carVendorLocations From supplierId and latitude latitude offset.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierID"></param>
        /// <param name="latitude"></param>
        /// <param name="latOffset">value 0 will not search by latitude and offset.</param>
        /// <returns></returns>
        public static List<CarVendorlocation> GetCarVendorLocationsFromSupplierIDAndLocCodeLength(int supplierID, int LocCodeLength)
        {
            string database = GetDatabaseFromConnectionString(connectionString);
            List<CarVendorlocation> locations = new List<CarVendorlocation>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                string latitudeCondition = "";
                if (LocCodeLength != 0)
                {
                    latitudeCondition = string.Format("and len(CarVendorLocationCode) < {0}", LocCodeLength);
                }

                cmd.CommandText = string.Format("select CarVendorLocationID,AirportCode,CarVendorLocationCode," +
                    "CarVendorLocationName,StreetAddress1,CityName,StateProvinceCode,StateProvinceName,ISOCountryCode," +
                    "Latitude,Longitude,StatusCode,UpdateDate,LastUpdatedBy,LocationTypeID,SupplierID,CarShuttleCategoryID," +
                    "PostalCode,PhoneNumber,FaxNumber,DeliveryBool,CollectionBool,OutOfOfficeHoursBool from CarVendorLocation" +
                    " where SupplierID = {0} {1}", supplierID, latitudeCondition);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CarVendorlocation loc = new CarVendorlocation();
                    loc.CarVendorLocationID = Convert.ToInt32(reader[0].ToString());
                    loc.AirportCode = reader[1].ToString();
                    loc.CarVendorLocationCode = reader[2].ToString();
                    loc.CarVendorLocationName = reader[3].ToString();
                    loc.StreeAddress1 = reader[4].ToString();
                    loc.CityName = reader[5].ToString();
                    loc.StateProvinceCode = reader[6].ToString();
                    loc.StateProvinceName = reader[7].ToString();
                    loc.ISOCountryCode = reader[8].ToString();
                    loc.Latitude = Double.Parse(reader[9].ToString());
                    loc.Longitude = Double.Parse(reader[10].ToString());
                    loc.StatusCode = reader[11].ToString();
                    loc.UpdateDate = reader[12].ToString();
                    loc.LastUpdatedBy = reader[13].ToString();
                    loc.LocationTypeID = reader[14].ToString();
                    loc.SupplierID = Convert.ToInt32(reader[15]);
                    // loc.CarShuttleCategoryID = reader[16].ToString();
                    loc.PostalCode = reader[17].ToString();//,,
                    loc.PhoneNumber = reader[18].ToString();
                    loc.FaxNumber = reader[19].ToString();
                    // loc.DeliveryBool = reader[20].ToString();
                    // loc.CollectionBool = reader[21].ToString();
                    //  loc.OutOfOfficeHoursBool = reader[22].ToString();

                    locations.Add(loc);
                }
                conn.Close();
            }
            return locations;
        }

        //Get the SupplierID with specified AirportCdoe and Lat/Long/Radius from table CarVendorLocation in CarsInventory DB.
        public static DataTable GetSupplierIDWithAirportCodeLatLongFromCarVendorLocation(string airportCode, string lat, string lon, string vendorLocationCode)
        {
            string database = GetDatabaseFromConnectionString(connectionString);
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                //Ustory 111626 DB Work for migration of VendorCode to use SupplierID   -- modified by v-pxie 2012-7-16
                string supportVendorCode = ConfigurationManager.AppSettings["SurportVendrCode"] == null ? "false" : ConfigurationManager.AppSettings["SurportVendrCode"];
                if (supportVendorCode.Equals("true"))
                {
                    cmd.CommandText = string.Format("select CarVendorCode from CarVendorLocation "
                    + "where AirportCode = '{0}' and Latitude = '{1}' and Longitude = '{2}' and CarVendorLocationCode = '{3}' and StatusCode = 'A'", airportCode, lat, lon, vendorLocationCode);
                }
                else
                {
                    cmd.CommandText = string.Format("select SupplierID from CarVendorLocation "
                    + "where AirportCode = '{0}' and Latitude = '{1}' and Longitude = '{2}' and CarVendorLocationCode = '{3}' and StatusCode = 'A'", airportCode, lat, lon, vendorLocationCode);
                }

                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        //select VendorCollectsFlag from carRentalAmountCalculation where ChargeTypeID=4 and OneWayBool =1
        //and SupplierID = 40 and CarBusinessModelID = 3
        public static string GetVendorCollectsFlag_GDSPOneWay_DropOffCharge(uint vendorSupplierID)
        {
            string vendorCollectsFlag = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                //Get the pointofsaleset based on POS attribute
                cmd.CommandText = string.Format("select VendorCollectsFlag from carRentalAmountCalculation where ChargeTypeID=4 and OneWayBool =1 "
                + "and SupplierID = {0} and CarBusinessModelID = 3", vendorSupplierID);
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    vendorCollectsFlag = reader[0].ToString();
                }

                conn.Close();
            }
            return vendorCollectsFlag;
        }

        //Get default rule based on car business model and POS attributes
        public static String GetDefaultRuleFromBusinessModelAndPOS(int carBusinessModelID, String jurisdictionCode, String companyCode,
            String managementUnitCode)
        {
            String ruleId = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                //Get the pointofsaleset based on POS attribute
                cmd.CommandText = string.Format("select distinct pointofsalesetid from PointOfSaleSetPointOfSale where JurisdictionCode='{0}' "
                + "and CompanyCode = '{1}' and ManagementUnitCode = '{2}'", jurisdictionCode, companyCode, managementUnitCode);
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                String pointOfSaleSetIdString = "";
                while (reader.Read())
                {
                    pointOfSaleSetIdString += reader.GetInt32(0).ToString() + ",";
                }
                pointOfSaleSetIdString = pointOfSaleSetIdString.Substring(0, pointOfSaleSetIdString.Length - 1);
                reader.Close();

                //Get the default rule based on carBusinessModelID and pointOfSaleSet
                cmd.CommandText = string.Format("select cr.CarRuleID from CarRule cr  join carrulematrix crm on cr.CarRuleTypeID = 3 and cr.CarBusinessModelID = {0} "
                + " and cr.CarRuleID = crm.CarRuleID and crm.PointOfSaleSetID in ({1})", carBusinessModelID, pointOfSaleSetIdString);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ruleId = reader.GetInt32(0).ToString();
                }

                conn.Close();
            }
            return ruleId;
        }

        //Get GDSP car commission based on CarItemID and Airport code
        public static decimal GetGDSPCommission(uint carItemID, String airportCode)
        {
            decimal commission = 0;
            bool success;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select CommissionPct from CarCommission where CarItemID = {0} and AirportCode = '{1}' "
                , carItemID, airportCode);
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    commission = reader.GetDecimal(0);
                    success = true;
                }
                else
                {
                    success = false;
                }
                reader.Close();

                conn.Close();
            }
            if (success)
            {
                return commission;
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandText = string.Format("select CommissionPct from CarCommission where CarItemID = {0} and AirportCode = \'\' "
                    , carItemID);
                    cmd.CommandTimeout = 0;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        commission = reader.GetDecimal(0);
                    }
                    reader.Close();

                    conn.Close();
                }
                return commission;
            }
        }


        //Select the Latitude and Longitude from table Airport according to AirportCode
        public static DataTable GetLonLatFromAirportCode(string airportCode)
        {
            string database = GetDatabaseFromConnectionString(connectionString);
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select Latitude,Longitude from Airport where AirportCode = '{0}' ", airportCode);
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        public static int getVoucherPreBasedOnTPID(int tpid)
        {
            int tpidMapped = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                //CarTravelProductMappingGet 30029
                cmd.CommandText = "exec CarTravelProductMappingGet " + tpid;
                cmd.CommandTimeout = 0;

                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    tpidMapped = Convert.ToInt32(dr["CarTravelProductID"]);
                }
            }
            return tpidMapped;
        }

        public static int getVoucherNumber()
        {
            int voucherNumber = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                //CarUniqueNbrForVoucherGetAndSet
                cmd.CommandText = "exec CarUniqueNbrForVoucherGetAndSet";
                cmd.CommandTimeout = 0;

                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    voucherNumber = Convert.ToInt32(dr["LastReturnedUniqueNbr"]);
                }
            }
            return voucherNumber;
        }


        public static String getCarCategoryCode(String categoryid)
        {
            String carCategoryCode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT CarCategoryExpeAbbr FROM CarCategory WHERE CarCategoryID = {0}", categoryid);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carCategoryCode = reader[0].ToString();
                }
                conn.Close();
            }
            return carCategoryCode;
        }

        public static String getCarCategoryName(int categoryid)
        {
            String carCategoryName = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT CarCategoryName FROM CarCategory WHERE CarCategoryID = {0}", categoryid);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carCategoryName = reader[0].ToString();
                }
                conn.Close();
            }
            return carCategoryName;
        }

        public static String getCarTypeCode(String cartypeid)
        {
            String carTypeCode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT CarTypeExpeAbbr FROM CarType WHERE CarTypeID = {0}", cartypeid);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carTypeCode = reader[0].ToString();
                }
                conn.Close();
            }
            return carTypeCode;

        }

        public static String getCarTransmissionDriveCode(String carTransmissionDriveID)
        {
            String carTransmissionDriveCode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT CarTransmissionDriveExpeAbbr FROM CarTransmissionDrive WHERE CarTransmissionDriveID = {0}", carTransmissionDriveID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carTransmissionDriveCode = reader[0].ToString();
                }
                conn.Close();
            }
            return carTransmissionDriveCode;
        }

        public static String getCarFuelAirConditionCode(String carFuelAirConditionID)
        {
            String carCarFuelAirConditionCode = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT CarFuelAirConditionExpeAbbr FROM CarFuelAirCondition WHERE CarFuelAirConditionID = {0}", carFuelAirConditionID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carCarFuelAirConditionCode = reader[0].ToString();
                }
                conn.Close();
            }
            return carCarFuelAirConditionCode;
        }

        public static uint getCarCategoryID(string carCategoryExpeAbbr)
        {
            uint carCategoryID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT CarCategoryID FROM CarCategory WHERE CarCategoryExpeAbbr = '{0}'", carCategoryExpeAbbr);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carCategoryID = Convert.ToUInt32(reader[0].ToString());
                }
                conn.Close();
            }
            return carCategoryID;
        }

        public static uint getCarTypeID(string carTypeExpeAbbr)
        {
            uint carTypeID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT CarTypeID FROM CarType WHERE  CarTypeExpeAbbr= '{0}'", carTypeExpeAbbr);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carTypeID = Convert.ToUInt32(reader[0].ToString());
                }
                conn.Close();
            }
            return carTypeID;

        }

        public static uint getCarTransmissionDriveID(string carTransmissionDriveExpeAbbr)
        {
            uint carTransmissionDriveID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT CarTransmissionDriveID FROM CarTransmissionDrive WHERE  CarTransmissionDriveExpeAbbr= '{0}'", carTransmissionDriveExpeAbbr);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carTransmissionDriveID = Convert.ToUInt32(reader[0].ToString());
                }
                conn.Close();
            }
            return carTransmissionDriveID;
        }

        public static uint getCarFuelAirConditionID(string carFuelAirConditionExpeAbbr)
        {
            uint carFuelAirConditionID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT CarFuelAirConditionID  FROM CarFuelAirCondition WHERE CarFuelAirConditionExpeAbbr = '{0}'", carFuelAirConditionExpeAbbr);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carFuelAirConditionID = Convert.ToUInt32(reader[0].ToString());
                }
                conn.Close();
            }
            return carFuelAirConditionID;
        }

        //get ClassificationIDs according to specified parameters 
        public static List<int> getClassificationIdForThisCar(int tpid, int groupId, String CarTypeID, String CarCategoryID,
                    String CarTransmissionDriveID, String CarFuelAirConditionID)
        {

            List<int> classificationIDs = new List<int>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                //query CarClassificationID by tpid and groupId 
                cmd.CommandText = string.Format("exec CarClassificationMapLstByGroupType {0}, {1}, null", tpid, groupId);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();


                List<int> classificationIdList = new List<int>();
                while (reader.Read())
                {
                    if (reader["CarTypeID"].Equals(CarTypeID) && reader["CarCategoryID"].Equals(CarCategoryID)
                            && reader["CarTransmissionDriveID"].Equals(CarTransmissionDriveID) && reader["CarFuelAirConditionID"].Equals(CarFuelAirConditionID)
                            && reader["IncludeBool"].Equals("1"))
                    {
                        //set CarClassificationID to list which satisfied conditions
                        classificationIdList.Add(Convert.ToInt32(reader["CarClassificationID"]));
                    }
                }

                //iterate through CarClassificationID list
                for (int i = 0; i < classificationIdList.Count; i++)
                {
                    cmd.CommandText = string.Format("exec CarClassificationToCarMapSrch {0}", classificationIdList[i]);
                    cmd.CommandTimeout = 0;

                    SqlDataReader carMapReader = cmd.ExecuteReader();
                    while (carMapReader.Read())
                    {
                        if (Convert.ToInt32(carMapReader["CarClassificationID"]) == classificationIdList[i])
                        {
                            if (carMapReader["CarTypeID"].Equals(CarTypeID) && carMapReader["CarCategoryID"].Equals(CarCategoryID)
                                    && carMapReader["CarTransmissionDriveID"].Equals(CarTransmissionDriveID) && carMapReader["CarFuelAirConditionID"].Equals(CarFuelAirConditionID)
                                    && carMapReader["IncludeBool"].Equals("1"))
                            {
                                classificationIDs.Add(Convert.ToInt32(carMapReader["CarClassificationID"]));
                            }
                        }
                    }
                }
                conn.Close();
            }
            return classificationIDs;
        }

        public static String GetVendorCode(String description)
        {
            String vendorCode = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select CarVendorCode from CarVendor where Description = '{0}' ", description);
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

       

        //get list of setIDs
        public static List<int> getAttributeListForRule(string queryText)
        {
            List<int> attributeList = new List<int>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = queryText;
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    attributeList.Add(Convert.ToInt32(reader[0]));
                }
                conn.Close();
            }
            return (attributeList.Count == 0 ? null : attributeList);
        }

        //Get car attribute set
        public static Dictionary<String, List<int>> getProductAttributeSetBySprocs(Dictionary<String, Object> carProductAttribute)
        {
            Dictionary<String, List<int>> carAttributeSet = new Dictionary<String, List<int>>();


            //PointOfSaleSetID
            string queryPointOfSaleSetIdName = string.Format("exec PointOfSaleSetPointOfSaleSrch null, '{0}','{1}','{2}'", carProductAttribute["JurisdictionCode"],
                                                                carProductAttribute["CompanyCode"], carProductAttribute["ManagementUnitCode"]);
            carAttributeSet.Add("PointOfSaleSetID", getAttributeListForRule(queryPointOfSaleSetIdName));

            //PurchaseTypeSetID
            string queryPurchaseTypeId = string.Format("exec PurchaseTypeSetPurchaseTypeSrch null, '{0}'", carProductAttribute["PurchaseType"]);
            carAttributeSet.Add("PurchaseTypeSetID", getAttributeListForRule(queryPurchaseTypeId));

            //PickupRegionSetID
            string queryPickupRegionSet = string.Format("exec PickupRegionSetPickupRegionSrch null, '{0}' ",
                        (carProductAttribute["PickupRegionId"].Equals("NULL") ? "-1" : carProductAttribute["PickupRegionId"]));
            carAttributeSet.Add("PickupRegionSetID", getAttributeListForRule(queryPickupRegionSet));

            //PickUpCountrySetID
            string queryCountryCode = string.Format("exec PickUpCountrySetPickUpCountrySrch null, '{0}' ", carProductAttribute["CountryCode"]);
            carAttributeSet.Add("PickUpCountrySetID", getAttributeListForRule(queryCountryCode));

            //PickupAirportSetID
            string queryAirport = string.Format("exec PickupAirportSetPickupAirportSrch null, '{0}'", carProductAttribute["AirportCode"]);
            carAttributeSet.Add("PickupAirportSetID", getAttributeListForRule(queryAirport));

            //CarVendorSetID
            string queryCarVendor = string.Format("exec CarVendorSetCarVendorSrch null, '{0}'", carProductAttribute["VendorCode"]);
            carAttributeSet.Add("CarVendorSetID", getAttributeListForRule(queryCarVendor));

            //CarClassificationSetID
            List<int> classificationIDList = (List<int>)carProductAttribute["ClassificationId"];
            if (classificationIDList.Count > 0)
            {
                List<int> resultList = new List<int>();
                for (int i = 0; i < classificationIDList.Count; i++)
                {
                    int classificationId = (classificationIDList[i].Equals("NULL") ? -1 : classificationIDList[i]);
                    string queryClassificationId = string.Format("exec CarClassificationSetCarClassificationSrch null, '{0}' ", classificationId);
                    resultList.AddRange(getAttributeListForRule(queryClassificationId));
                }
                carAttributeSet.Add("CarClassificationSetID", resultList);
            }
            else
            {
                carAttributeSet.Add("CarClassificationSetID", null);
            }
            return carAttributeSet;
        }

        //get CarRule Data
        public static DataSet getCarRuleData()
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("exec CarRuleMatrixCache");
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(ds, "CarRuleCache");
                conn.Close();
            }
            return ds;
        }

        //get TPIDToPOS value from TPIDToPoSAttributeMap
        public static String getTpidToPosMapValue(int tpid)
        {
            String posValue = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select JurisdictionCode, CompanyCode, ManagementUnitCode from TPIDToPoSAttributeMap where TravelProductID = {0}", tpid);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    posValue = reader[0].ToString() + "/" + reader[1].ToString() + "/" + reader[2].ToString();
                }

                conn.Close();
            }
            return posValue;
        }

        // Get default rule based on car business model and POS attributes
        public static DataTable getDefaultRuleMarkup(DataTable carRuleID, int carMarginRateTypeid)
        {
            DataTable dt = new DataTable();
            String carRuleIDValue = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                for (int i = 0; i < carRuleID.Rows.Count; i++)
                {
                    if (i == 0)
                        carRuleIDValue = "'" + carRuleID.Rows[i][0].ToString() + "'";
                    else
                        carRuleIDValue = carRuleIDValue + "," + "'" + carRuleID.Rows[i][0].ToString() + "'";
                }

                cmd.CommandText = string.Format("select distinct MarginPct  from CarRuleMargin where CarMarginRateTypeID = {0} " +
                                "and  CarRuleID in ({1}) and CarMarginTypeID = 1", carMarginRateTypeid, carRuleIDValue);
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        public static bool hasNagativeRuleMarkup(List<uint> carRuleLIst)
        {
            DataTable dt = new DataTable();
            String carRuleIDValue = string.Join(",", carRuleLIst.ToArray()); 
            int j = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                //cmd.CommandText = string.Format("select  * from CarRuleMargin where CarRuleID in ({0}) and MarginPct<0", carRuleIDValue);
               // cmd.CommandText = string.Format("select  * from CarRule where CarRuleID in ({0}) and (CarRuleTypeID = 1 or CarRuleTypeID=2)", carRuleIDValue);
                cmd.CommandText = string.Format("select  * from CarRuleMatrix where CarRuleID in ({0}) and PurchaseTypeSetID is not null", carRuleIDValue);
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    j++;
                }
                //SqlDataAdapter ad = new SqlDataAdapter(cmd);
                //ad.Fill(dt);
                //conn.Close();
            }
           // return dt.Rows.Count >0;
            if (j > 0)
                return true;
            else
                return false;
   
        }
        //Get pointofsalesetid string according to jurisdictionCode, companyCode, managementUnitCode
        public static DataTable getPointOfSaleSetID(String jurisdictionCode, String companyCode, String managementUnitCode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select distinct pointofsalesetid  from PointOfSaleSetPointOfSale where JurisdictionCode = '{0}' " +
                    " and CompanyCode = '{1}' and ManagementUnitCode = '{2}'", jurisdictionCode, companyCode, managementUnitCode);
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        //get RuleID according to carBusinessModelID and carMarginRateTypeid
        public static DataTable getRuleID(DataTable pointOfSaleSetIdString, int carBusinessModelID)
        {
            DataTable dt = new DataTable();
            String pointOfSaleSetIdStringValue = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                for (int i = 0; i < pointOfSaleSetIdString.Rows.Count; i++)
                {
                    if (i == 0)
                        pointOfSaleSetIdStringValue = "'" + pointOfSaleSetIdString.Rows[i]["pointofsalesetid"].ToString() + "'";
                    else
                        pointOfSaleSetIdStringValue = pointOfSaleSetIdStringValue + "," + "'" + pointOfSaleSetIdString.Rows[i][0].ToString() + "'";
                }

                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select cr.CarRuleID  from CarRule cr join carrulematrix crm on cr.CarRuleTypeID = 3 and cr.CarBusinessModelID = {0} " +
                    " and cr.CarRuleID = crm.CarRuleID and crm.PointOfSaleSetID in ({1}) order by cr.CreateDate Desc", carBusinessModelID, pointOfSaleSetIdStringValue);
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();


            }
            return dt;
        }

        //get WeightedSelectivityRank for specified car attribute
        public static Dictionary<String, Object> getAttributeWeight(String jurisdictionCode, String companyCode, String managementCode,
                                                                                 int carRuleTypeId, int carMarginTypeId)
        {
            Dictionary<String, Object> attributeWeight = new Dictionary<String, Object>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("exec CarRuleSelectivityOrderSrch '{0}', '{1}', '{2}',{3}, {4}",
                            jurisdictionCode, companyCode, managementCode, carRuleTypeId, carMarginTypeId);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //store WeightedSelectivityRank with AttributeName for key
                    String key = reader["AttributeName"].ToString();
                    Object weightValue = reader["WeightedSelectivityRank"];
                    attributeWeight.Add(key, weightValue);
                }
                conn.Close();
            }
            return attributeWeight;
        }

        //get create date for rules
        public static Dictionary<String, DateTime> getCreatedDateForRules(List<String> ruleList)
        {
            Dictionary<String, DateTime> ruleDateMap = new Dictionary<String, DateTime>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                foreach (String rule in ruleList)
                {
                    cmd.CommandText = string.Format("Select CreateDate from CarRule where CarRuleID = {0}", Convert.ToUInt16(rule));
                    cmd.CommandTimeout = 0;

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        DateTime dateString = DateTime.Parse(reader[0].ToString());
                        ruleDateMap.Add(rule, dateString);
                    }

                }
                conn.Close();
            }
            return ruleDateMap;
        }

        // add by v-pxie
        public static uint GetAccountingVendorIDByCarItemID(uint carItemID)
        {
            uint AccountingVendorID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT AccountingVendorID FROM CarItem WHERE CarItemID = {0}", carItemID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {
                        AccountingVendorID = uint.Parse(reader[0].ToString());
                    }
                    catch (Exception)
                    {

                        Console.WriteLine("AccountingVendorID is null");
                    }
                }
                conn.Close();
            }

            return AccountingVendorID;
        }


        public static uint GetPubCompareEnableByCPCatID(uint carProductCatalogID)
        {
            bool enabled = false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select PublishedPriceCompareEnabledBool " +
                    " FROM CarProductCatalogConfiguration where CarProductCatalogID = {0}",
                        carProductCatalogID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    enabled = Convert.ToBoolean(reader[0].ToString());
                }
                conn.Close();
            }
            if (enabled == true)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        // delete from carsinventory_stt01..CarVendorLocation where carvendorlocationname like '%admintooltest%'
        public static void deleteLocationByName(string locationName)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("delete from CarVendorLocation where carvendorlocationname like '%{0}%'", locationName);
                Console.WriteLine(cmd.CommandText);
                cmd.CommandTimeout = 0;

                cmd.ExecuteNonQuery();

                conn.Close();
            }

        }

        public static DataSet GetCarCostAndAvailGetDataTable(GetCarCostAndAvailGetInputs inputs)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                //3353,'CDG',"2012-08-09 12:08:00" ,"2012-08-20 12:08:00",7,3,1,1,'AL','T701',true
                //pCarAgreementID, pAirportCode, pPickUpDate, pDropOffDate, pCarCategoryID, 
                //pCarTypeID, pCarTransmissionDriveID, pCarFuelAirConditionID, pCarVendorCode, pCarVendorLocationCode, pOnAirportSearchBool
                cmd.CommandText = string.Format("exec dbo.CarCostAndAvailGet {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    inputs.pCarAgreementID, "'" + inputs.pAirportCode + "'", "'" + inputs.pPickUpDate.ToString("yyyy-MM-dd HH:mm:ss") + "'",
                    "'" + inputs.pDropOffDate.ToString("yyyy-MM-dd HH:mm:ss") + "'",
                    inputs.pCarCategoryID, inputs.pCarTypeID, inputs.pCarTransmissionDriveID, inputs.pCarFuelAirConditionID,
                    "'" + inputs.pCarVendorCode + "'", "'" + inputs.pCarVendorLocationCode + "'", inputs.pOnAirportSearchBool);
                Console.WriteLine(cmd.CommandText);
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(ds);

                conn.Close();
            }

            //Throw the exception if no rows queried from sproc CarCostAndAvailGet, 
            if (ds.Tables.Count < 1 || ds.Tables[0].Rows.Count < 1) throw new Exception(String.Format("No data queried from sproc CarCostAndAvailGet, parameters: pCarAgreemntID - {0}, "
                 + "pAirportCode - {1}, pPickUpDate - {2}, pDropOffDate - {3}, pCarCategoryID - {4}, pCarTypeID - {5}, pCarTransmissionDriveID - {6}, pCarFuelAirConditionID - {7}"
                 + " , pCarVendorCode - {8}, pCarVendorLocationCode - {9}, pOnAirportSearchBool - {10}", inputs.pCarAgreementID, inputs.pAirportCode, inputs.pPickUpDate, inputs.pDropOffDate, inputs.pCarCategoryID,
                 inputs.pCarTypeID, inputs.pCarTransmissionDriveID, inputs.pCarFuelAirConditionID, inputs.pCarVendorCode, inputs.pCarVendorLocationCode, inputs.pOnAirportSearchBool));
            return ds;
        }

        public static DataSet GetCarTaxRateSrchDataTable(string pAirportCode, string pCarVendorCode)
        {

            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                //'CDG', 'AL'
                //exec dbo.CarTaxRateSrch @pAirportCode, @pCarVendorCode  
                cmd.CommandText = string.Format("exec dbo.CarTaxRateSrch {0},{1}", "'" + pAirportCode + "'", "'" + pCarVendorCode + "'");
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(ds, "CarCostAndAvailTable");
                conn.Close();
            }
            return ds;
        }

        public static DataTable GetCommissionDataTable(uint carItemID, string airPortCode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select CarCommissionLogID, AirportCode from CarCommission where CarItemID = {0} and AirportCode = '{1}' "
                , carItemID, airPortCode);
                cmd.CommandTimeout = 0;
                //SqlDataReader reader = cmd.ExecuteReader();

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);

                conn.Close();
            }
            return dt;
        }

        //For TP95, Select SupplierID base on the CarItemIDs got from CarSS search request.
        public static List<uint> GetSupplierIDFromCarSSCarItemID(string carItemID)
        {
            List<uint> supplierID = new List<uint>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("Select distinct SupplierID from CarItem where CarItemID in ({0})", carItemID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    supplierID.Add(uint.Parse(reader[0].ToString()));
                }
                conn.Close();
            }
            return supplierID;
        }

        public static String GetIATAOverrideBasedOnSupplySubSetID(uint supplySubsetID)
        {
            String IATAOverride = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select IATAOverrideBooking from SupplySubSetToWorldSpanSupplierItemMap where SupplySubsetID = {0} ", supplySubsetID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    IATAOverride = reader[0].ToString();
                }
                conn.Close();
            }
            return IATAOverride;
        }

        public static String GetIATAAgencyCodeBasedOnSupplySubSetID(uint supplySubsetID)
        {
            String IATAAgencyCode = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select IATAAgencyCode from SupplySubSetToWorldSpanSupplierItemMap where SupplySubsetID = {0} ", supplySubsetID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    IATAAgencyCode = reader[0].ToString();
                }
                conn.Close();
            }
            return IATAAgencyCode;
        }

        public static String GetCDForShoppingBasedOnSupplySubSetID(uint supplySubsetID)
        {
            String CDForShopping = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select CorporateDiscountCodeRequiredInShopping from SupplySubSetToWorldSpanSupplierItemMap where SupplySubsetID = {0} ", supplySubsetID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CDForShopping = reader[0].ToString();
                }
                conn.Close();
            }
            return CDForShopping;
        }

        public static String GetCDBasedOnSupplySubSetID(uint supplySubsetID)
        {
            String CD = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select CorporateDiscountCode from SupplySubSetToWorldSpanSupplierItemMap where SupplySubsetID = {0} ", supplySubsetID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CD = reader[0].ToString();
                }
                conn.Close();
            }
            return CD;
        }

        public static String GetRateCodeBasedOnSupplySubSetID(uint supplySubsetID)
        {
            String RateCode = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select RateCode from SupplySubSetToWorldSpanSupplierItemMap where SupplySubsetID = {0} ", supplySubsetID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RateCode = reader[0].ToString();
                }
                conn.Close();
            }
            return RateCode;
        }

        public static String GetITCodeBasedOnSupplySubSetID(uint supplySubsetID)
        {
            String ITcode = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select ITNumber from SupplySubSetToWorldSpanSupplierItemMap where SupplySubsetID = {0} ", supplySubsetID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ITcode = reader[0].ToString();
                }
                conn.Close();
            }
            return ITcode;
        }

        //public static Dictionary<uint, string> GetITCodeBasedOnSupplySubSetID(List<uint> subsetIDList)
        //{
        //    string subsetIDString = string.Join(",", subsetIDList.ToArray());
        //    Dictionary<uint, string> supplySubsetIDsAndITNumber = new Dictionary<uint, string>();

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        SqlCommand cmd = conn.CreateCommand();
        //        conn.Open();

        //        cmd.CommandText = string.Format("select distinct SupplySubsetID, ITNumber from SupplySubSetToWorldSpanSupplierItemMap where SupplySubsetID in({0}) ", subsetIDString);
        //        cmd.CommandTimeout = 0;

        //        SqlDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            supplySubsetIDsAndITNumber.Add(Convert.ToUInt32(reader[0].ToString()), reader[1].ToString());
        //        }
        //        conn.Close();
        //    }
        //    return supplySubsetIDsAndITNumber;
        //}

        public static List<SupplySubSetToWorldSpanSupplierItemMapInfo> GetSupplySubSetToWorldSpanSupplierItemMapInfo(List<uint> subsetIDList)
        {
            string subsetIDString = string.Join(",", subsetIDList.ToArray());
            List<SupplySubSetToWorldSpanSupplierItemMapInfo> supplySubSetIDMap = new List<SupplySubSetToWorldSpanSupplierItemMapInfo>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select distinct SupplySubsetID, ITNumber, CorporateDiscountCodeRequiredInShopping,RateCode,IATAAgencyCode,CorporateDiscountCode from SupplySubSetToWorldSpanSupplierItemMap where SupplySubsetID in({0}) ", subsetIDString);
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    SupplySubSetToWorldSpanSupplierItemMapInfo map = new SupplySubSetToWorldSpanSupplierItemMapInfo();
                    map.supplySubsetID = Convert.ToUInt32(reader[0].ToString());
                    map.itNumber = reader[1].ToString();
                    map.corporateDiscountCodeRequiredInShopping = Convert.ToBoolean(reader[2].ToString());
                    map.rateCode = reader[3].ToString();
                    map.iataAgencyCode = reader[4].ToString();
                    map.corporateDiscountCode =  reader[5].ToString();
                    supplySubSetIDMap.Add(map);
                }
                conn.Close();
            }
            return supplySubSetIDMap;
        }

        public static String GetCDForBookingBasedOnSupplySubSetID(uint supplySubsetID)
        {
            String CDForBooking = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select CorporateDiscountCodeRequiredInBooking from SupplySubSetToWorldSpanSupplierItemMap where SupplySubsetID = {0} ", supplySubsetID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CDForBooking = reader[0].ToString();
                }
                conn.Close();
            }
            return CDForBooking;
        }

        public static uint GetCarProductCatalogIDByTPID(int tpid)
        {
            uint CarProductCatalogID = 0;

            List<PointOfSaleKey> pointOfSaleKeyList = getPointOfSaleRecord(tpid);
            PointOfSaleKey pointOfSaleKey = pointOfSaleKeyList[0];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select CarProductCatalogID from PoSToCarProductCatalogMap where JurisdictionCode = '{0}' and CompanyCode = '{1}' and ManagementUnitCode = '{2}' ",
                    pointOfSaleKey.JurisdictionCountryCode, pointOfSaleKey.CompanyCode, pointOfSaleKey.ManagementUnitCode);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CarProductCatalogID = uint.Parse(reader[0].ToString());
                }
                conn.Close();
            }
            return CarProductCatalogID;
        }

        public static uint GetCarProductCatalogIDByPOS(string jurisdictionCode, string companyCode, string managementUnitCode)
        {
            uint CarProductCatalogID = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select CarProductCatalogID from PoSToCarProductCatalogMap where JurisdictionCode = '{0}' and CompanyCode = '{1}' and ManagementUnitCode = '{2}' ",
                    jurisdictionCode, companyCode, managementUnitCode);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CarProductCatalogID = uint.Parse(reader[0].ToString());
                }
                conn.Close();
            }
            return CarProductCatalogID;
        }

        //Get IATAAgencyCode from DB according to CarItemID
        public static uint GetIATAAgencyCodeByCarItem(uint carItemID)
        {
            uint iataAgencyCode = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format(" select IATAAgencyCode from SupplySubSetToWorldSpanSupplierItemMap where  SupplySubsetID in" +
                        "(select caritmSupRank.SupplySubsetID from CarItemSupplySubsetRank caritmSupRank join caritem ci on caritmSupRank.CarItemID = ci.CarItemID and ci.CarItemID = {0})", carItemID);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    iataAgencyCode = Convert.ToUInt16(reader[0].ToString());
                }
                conn.Close();
            }
            return iataAgencyCode;
        }

        //Get IATAOverrideBooking from DB according to CarItemID
        public static uint GetIATAOverrideBookingByCarItem(uint carItemID)
        {
            uint iataOverrideBooking = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format(" select IATAOverrideBooking from SupplySubSetToWorldSpanSupplierItemMap where  SupplySubsetID in" +
                        "(select caritmSupRank.SupplySubsetID from CarItemSupplySubsetRank caritmSupRank join caritem ci on caritmSupRank.CarItemID = ci.CarItemID and ci.CarItemID = {0})", carItemID);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    iataOverrideBooking = Convert.ToUInt16(reader[0].ToString());
                }
                conn.Close();
            }
            return iataOverrideBooking;
        }

        //Update IATAAgencyCode column in SupplySubSetToWorldSpanSupplierItemMap
        public static void UpdateIATAAgencyCode(String IATAAgencyCode, uint carItemID)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("update SupplySubSetToWorldSpanSupplierItemMap set IATAAgencyCode = {0} where SupplySubsetID in" +
                        "(select caritmSupRank.SupplySubsetID from CarItemSupplySubsetRank caritmSupRank join caritem ci on caritmSupRank.CarItemID = ci.CarItemID and ci.CarItemID = {1})"
                        , IATAAgencyCode, carItemID);

                cmd.CommandTimeout = 0;

                cmd.ExecuteReader();

                conn.Close();
            }
        }

        //Update IATAOverrideBooking column in SupplySubSetToWorldSpanSupplierItemMap
        public static void UpdateIATAOverrideBooking(String IATAOverride, uint carItemID)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("update SupplySubSetToWorldSpanSupplierItemMap set IATAOverrideBooking = {0} where SupplySubsetID in" +
                        "(select caritmSupRank.SupplySubsetID from CarItemSupplySubsetRank caritmSupRank join caritem ci on caritmSupRank.CarItemID = ci.CarItemID and ci.CarItemID = {1})"
                        , IATAOverride, carItemID);

                cmd.CommandTimeout = 0;

                cmd.ExecuteReader();

                conn.Close();
            }
        }

        public static List<String> getLocationCodeWithDeliveryCollection(String airportCode, String isSupportDelivery, String isSupportCollection)
        {

            List<String> result = new List<String>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                //  cmd.CommandText = string.Format("SELECT CountryCode as jurisdictionCountryCode, CompanyID as CompanyCode, OrganizationID as ManagementUnitCode FROM TPIDToPoSAttributeMap WHERE TravelProductID = {0}", tpid);

                String text = "SELECT distinct CarVendorLocationCode from CarVendorLocation where AirportCode = '{0}'";
                if (!isSupportDelivery.Trim().Equals(""))
                    text = text + " and DeliveryBool = '{1}'";
                if (!isSupportCollection.Trim().Equals(""))
                    text = text + " and CollectionBool = '{2}'";

                cmd.CommandText = string.Format(text, airportCode, isSupportDelivery.Trim(), isSupportCollection.Trim());
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader[0].ToString());
                }
                conn.Close();
            }
            return result;
        }

        public static string GetCarVendorLocationCodeBySupplyIDAndAirportCode(uint supplierID, string airportCode)
        {
            string carVendorlocation = string.Empty;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                String sqlstring = "SELECT  CarVendorLocationCode from CarVendorLocation "
                    + " where AirportCode = '{0}'and SupplierId = {1} and StatusCode = 'A'";

                cmd.CommandText = string.Format(sqlstring, airportCode, supplierID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carVendorlocation = reader[0].ToString();
                }
                conn.Close();
            }
            return carVendorlocation;
        }

        public static DataTable getLocationInfoBySupplyIDAndAirportCodeAndVendorLocationCode(int supplierID, string airportCode, string vendorLocationCode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select StreetAddress1, Latitude, Longitude from CarVendorLocation where SupplierID={0} and CarVendorLocationCode='{1}' and AirportCode='{2}'"
                , supplierID, vendorLocationCode, airportCode);

                Console.WriteLine(cmd.CommandText);

                cmd.CommandTimeout = 0;
                //SqlDataReader reader = cmd.ExecuteReader();

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);

                conn.Close();
            }
            return dt;
        }

        public static string getMediaInfoByCarMediaID(int carMediaID)
        {
            string str = string.Empty;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select MediaFileName from CarMedia where CarMediaID={0}", carMediaID);
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    str = reader[0].ToString();
                }
                conn.Close();
            }
            return str;
        }


        public static bool existMediaInfoForRequestedCar(int supplierID, uint carCategoryID, uint carTypeID, string locationCode)
        {
            bool isExist = true;
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select * from CarMedia where CarMediaID in (select CarMediaID from CarMediaConfiguration  where CarCategoryID={0} and CarTypeID={1} and SupplierID={2} and CountryCode=(select CountryCode from Airport where AirportCode='{3}'))"
                , carCategoryID, carTypeID, supplierID, locationCode);
                cmd.CommandTimeout = 0;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);

                conn.Close();
            }
            if (null == dt)
            {
                isExist = false;
            }
            return isExist;
        }

        public static bool existLocationInfoForRequestedCar(int supplierID, string airportCode, string carVendorLocationCode)
        {
            bool isExist = true;
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select * from CarVendorLocation where SupplierID={0} and CarVendorLocationCode='{1}' and AirportCode='{2}'"
                , supplierID, carVendorLocationCode, airportCode);
                cmd.CommandTimeout = 0;
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);

                conn.Close();
            }
            if (null == dt)
            {
                isExist = false;
            }
            return isExist;
        }

        //This method is used to executive the sproc of finding CarVendorLocation based on given Lat/Long/Radius
        //e.g: exec CarVendorLocationLstByLatLong ZD,47.461566307675597,47.751211492324403,-122.544663288493836,-122.115815946827664
        public static DataTable ExcuteSprocFunction(string sprocName, List<string> parameterList)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string command = "exec " + sprocName + " ";
                int i = 0;
                foreach (string parameter in parameterList)
                {
                    command += parameter;
                    if (i++ != parameterList.Count - 1)
                    {
                        command += ",";
                    }
                }

                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = command;
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        public static List<CarTaxRateSrchResult> getTaxsForMerchantCar(string carVendorCode, string airportCode, string vendorLocationCode)
        {
            List<CarTaxRateSrchResult> result = new List<CarTaxRateSrchResult>();
            List<string> paramerList = new List<string>();
            paramerList.Add(airportCode);
            paramerList.Add(carVendorCode);
            DataSet taxFeesWithNoLocationCode = GetCarTaxRateSrchDataTable(airportCode, carVendorCode);
            if (taxFeesWithNoLocationCode.Tables.Count < 1) throw new Exception(String.Format("No result form sproc CarTaxRateSrch, parameters: airportCode - {0}, carVendorCode - {1}", airportCode, carVendorCode));
            foreach (DataRow dataRow in taxFeesWithNoLocationCode.Tables[0].Rows)
            {
                if (dataRow["CarVendorLocationCode"].ToString().Trim().Equals(vendorLocationCode)
                    && Convert.ToDouble(dataRow["CarTaxRatePct"].ToString()) > 0)
                {
                    result.Add(new CarTaxRateSrchResult(dataRow["AirportCode"].ToString().Trim(), dataRow["TaxCurrencyCode"].ToString().Trim(), dataRow["CarVendorLocationCode"].ToString().Trim(),
                        dataRow["CarVendorCode"].ToString().Trim(), dataRow["CarTaxFeeTypeName"].ToString().Trim(), dataRow["CarTaxRatePct"].ToString().Trim(), dataRow["CarTaxAmt"].ToString().Trim(), dataRow["CarTaxDurationCode"].ToString().Trim()));
                }
            }
            return result;
        }

        public static List<CarTaxRateSrchResult> getFeesForMerchantCar(string carVendorCode, string airportCode, string vendorLocationCode)
        {
            List<CarTaxRateSrchResult> result = new List<CarTaxRateSrchResult>();
            List<string> paramerList = new List<string>();
            paramerList.Add(airportCode);
            paramerList.Add(carVendorCode);
            DataSet taxFeesWithNoLocationCode = GetCarTaxRateSrchDataTable(airportCode, carVendorCode);
            if (taxFeesWithNoLocationCode.Tables.Count < 1) throw new Exception(String.Format("No result form sproc CarTaxRateSrch, parameters: airportCode - {0}, carVendorCode - {1}", airportCode, carVendorCode));
            foreach (DataRow dataRow in taxFeesWithNoLocationCode.Tables[0].Rows)
            {
                if (dataRow["CarVendorLocationCode"].ToString().Trim().Equals(vendorLocationCode)
                    && Convert.ToDouble(dataRow["CarTaxAmt"].ToString()) > 0)
                {
                    result.Add(new CarTaxRateSrchResult(dataRow["AirportCode"].ToString().Trim(), dataRow["TaxCurrencyCode"].ToString().Trim(), dataRow["CarVendorLocationCode"].ToString().Trim(),
                       dataRow["CarVendorCode"].ToString().Trim(), dataRow["CarTaxFeeTypeName"].ToString().Trim(), dataRow["CarTaxRatePct"].ToString().Trim(), dataRow["CarTaxAmt"].ToString().Trim(), dataRow["CarTaxDurationCode"].ToString().Trim()));
                }
            }
            return result;
        }

        public static double GetVariableCostPct(uint TPID)
        {
            //SELECT VariableCostPct FROM dbo.CarOtherCost WHERE TravelProductID = 3
            double variableCostPct = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                String sqlstring = String.Format("SELECT VariableCostPct FROM dbo.CarOtherCost WHERE TravelProductID = {0}", TPID);

                cmd.CommandText = sqlstring;
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    variableCostPct = Convert.ToDouble(reader[0].ToString());
                }
                conn.Close();
            }
            return variableCostPct;
        }

        public static uint GetCarProductCatalogIDByCarItemID(uint carItemID)
        {
            uint carproductCatalogID = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                String sqlstring = "select Distinct carProductCatalogID from CarProductCatalogChild"
                    + " where CarProductCatalogChildID in "
                    + " (select CarProductCatalogChildID from CarProductCatalogChildCarItem cpci"
                    + " join CarItem ci on ci.CarItemID = cpci.caritemID"
                    + " where ci.CarItemID = {0})";

                cmd.CommandText = string.Format(sqlstring, carItemID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carproductCatalogID = uint.Parse(reader[0].ToString());
                }
                conn.Close();
            }
            return carproductCatalogID;
        }

        public static List<uint> GetCarProductCatalogIDByCarItemID(string carItemID)
        {
            List<uint> carproductCatalogIDList = new List<uint>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                String sqlstring = "select Distinct carProductCatalogID from CarProductCatalogChild"
                    + " where CarProductCatalogChildID in "
                    + " (select CarProductCatalogChildID from CarProductCatalogChildCarItem cpci"
                    + " join CarItem ci on ci.CarItemID = cpci.caritemID"
                    + " where ci.CarItemID = {0})";

                cmd.CommandText = string.Format(sqlstring, carItemID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carproductCatalogIDList.Add(Convert.ToUInt32(reader[0].ToString()));
                }
                conn.Close();
            }
            return carproductCatalogIDList;
        }

        //if countrycode is null return all airportcode.
        public static List<string> GetAirportCodeByCountryCode(string countryCode)
        {
            List<string> AirportCodes = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                string condition = "";
                if(!string.IsNullOrEmpty(countryCode))
                  condition =   "where CountryCode = " + countryCode;
                cmd.CommandText = String.Format("select AirportCode from Airport {0}", condition);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    AirportCodes.Add(reader["AirportCode"].ToString());
                }
                conn.Close();
            }
            return AirportCodes;
        }

        public static List<TPIDToPoSAttributeMap> getTPIDToPoSAttributeMapByPOS(string jurisdictionCode, string companyCode, string managementUnitCode)
        {
            List<TPIDToPoSAttributeMap> objTPIDToPoSAttributeMapList = new List<TPIDToPoSAttributeMap>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("select TravelProductID, PartnerID from TPIDToPoSAttributeMap where JurisdictionCode = '{0}' and CompanyCode = '{1}' and ManagementUnitCode = '{2}' order by PartnerID", jurisdictionCode, companyCode, managementUnitCode);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objTPIDToPoSAttributeMapList.Add(new TPIDToPoSAttributeMap(Convert.ToInt32(reader["TravelProductID"]), Convert.ToUInt16(reader["PartnerID"]), jurisdictionCode, companyCode, managementUnitCode));
                }

                conn.Close();
            }
            return objTPIDToPoSAttributeMapList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<CarVendorLocation_providerData> getCarVendorlocation_providerData(string supplierID, string providerID = "7")
        {
            List<CarVendorLocation_providerData> result = new List<CarVendorLocation_providerData>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from CarVendorLocation_providerData where ProviderID='{0}' and Supplier='{1}'",
                    providerID, supplierID);                
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    result.Add(new CarVendorLocation_providerData(reader["ProviderID"], reader["Supplier"], reader["IATACode"], reader["LocationCode"],
                        reader["LocationName"], reader["StreetAddress1"], reader["CityName"]
                        , reader["StateProvinceCode"], reader["StateProvinceName"], reader["PostalCode"], reader["ISOCountryCode"],
                        reader["PhoneNumber"], reader["FaxNumber"], reader["Latitude"], reader["Longitude"], reader["LocationType"]
                        , reader["ShuttleCategory"], reader["DeliveryBool"], reader["CollectionBool"], reader["OutOfOfficeHoursBool"], reader["CreateDate"], reader["CreatedBy"]));
                }
                conn.Close();
            }

            return result;
        }

        public static CarVendorlocation getCarVendorlocation(string supplierID, string locationCode, string carLocationCategoryCode, string SupplierRawText, out bool existLocationRecord)
        {
            CarVendorlocation result = null;
            existLocationRecord = false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from CarVendorLocation where AirportCode = '{0}' and SupplierID = '{3}' and CarVendorLocationCode = '{1}{2}'", locationCode, carLocationCategoryCode, SupplierRawText, supplierID);
                //cmd.CommandText = string.Format("select  cvl.*, csc.CarShuttleCategoryCode from CarVendorLocation cvl " +
                //                                "left join CarShuttleCategory csc on cvl.CarShuttleCategoryID = csc.CarShuttleCategoryID " +
                //                                "where cvl.AirportCode = '{0}' and cvl.CarVendorLocationCode = '{1}{2}'", locationCode, carLocationCategoryCode, SupplierRawText);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //(1) StreetAddress1, (2) Cityname, (3) Stateprovince code or Stateprovincename or postal code, (4) ISO County Code.
                    //Meichun: 2013 - 12 - 6 - for the new design, no need to check StateProvinceCode/StateProvinceName/PostalCode exist or not
                    if (reader["StreetAddress1"].ToString().Trim() != null
                        && reader["CityName"].ToString().Trim() != null
                        && reader["ISOCountryCode"].ToString().Trim() != null
                        //&& ((reader["StateProvinceCode"].ToString().Trim() != null && reader["StateProvinceCode"].ToString().Trim().Length > 0)
                        //|| (reader["StateProvinceName"].ToString().Trim() != null && reader["StateProvinceName"].ToString().Trim().Length > 0)
                        //|| (reader["PostalCode"].ToString().Trim() != null && reader["PostalCode"].ToString().Trim().Length > 0))
                        )
                    {
                        existLocationRecord = true;
                    }
                    result = new CarVendorlocation(Convert.ToInt32(reader["CarVendorLocationID"].ToString().Trim()), reader["AirportCode"].ToString().Trim(),
                        reader["CarVendorLocationCode"].ToString().Trim(), reader["CarVendorLocationName"].ToString().Trim(), reader["StreetAddress1"].ToString().Trim(), reader["StateProvinceCode"].ToString().Trim(), reader["StateProvinceName"].ToString().Trim(),
                        reader["CityName"].ToString().Trim(), reader["ISOCountryCode"].ToString().Trim(), Convert.ToInt32(reader["SupplierID"]),
                        reader["LocationTypeID"],
                        reader["PostalCode"], reader["FaxNumber"]
                    , reader["Latitude"], reader["Longitude"], reader["CarShuttleCategoryID"].ToString()
                        , reader["DeliveryBool"].ToString(), reader["CollectionBool"].ToString(), reader["OutOfOfficeHoursBool"].ToString(),
                        reader["PhoneNumber"].ToString());
                    result.LastUpdatedBy = Convert.ToString(reader["LastUpdatedBy"]);
                    result.UpdateDate = Convert.ToString(reader["UpdateDate"]);//Latitude  Longitude
                }
                conn.Close();
            }

            return result;
        }

        // Read CarShuttleCategoryCode from CarVendorLocation table 
        public static string GetCarShuttleCategoryCode(CarInventoryKey inventorykey)
        {
             // select CarShuttleCategoryCode  from CarshuttleCategory
            //where CarShuttleCategoryID =(select  CarShuttleCategoryID from CarVendorLocation
            //where AirportCode = 'SEA' and CarVendorLocationCode = 'T001'and SupplierID = 14)
            string shuttleCategoryCode = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    String sqlstring = "select CarShuttleCategoryCode  from CarshuttleCategory"
                        + " where CarShuttleCategoryID =(select  CarShuttleCategoryID from CarVendorLocation"
                        + " where AirportCode = '{0}' and CarVendorLocationCode = '{1}'and SupplierID = {2} )";

                    cmd.CommandText = string.Format(sqlstring, inventorykey.CarCatalogKey.CarPickupLocationKey.LocationCode,
                        inventorykey.CarCatalogKey.CarPickupLocationKey.CarLocationCategoryCode 
                        + inventorykey.CarCatalogKey.CarPickupLocationKey.SupplierRawText, 
                        inventorykey.CarCatalogKey.VendorSupplierID);
                    cmd.CommandTimeout = 0;
                    //Print.PrintMessageToConsole("DB cmd String: [" + cmd.CommandText + "]");
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        shuttleCategoryCode = reader[0].ToString();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new CarException(CarErrorMessage.readDBTableError, ex);
            }
            return shuttleCategoryCode;
        }

        /// <summary>
        ///  off airport set Location code , per vendor-airport code 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCarVendorLocationCodeListWithAirportCode(uint supplierID, string airportCode)
        {
            List<String> locationList = new List<String>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                String sqlstring = "SELECT  CarVendorLocationCode from CarVendorLocation "
                    + " where AirportCode = '{0}'and SupplierId = {1} and StatusCode = 'A'";

                cmd.CommandText = string.Format(sqlstring, airportCode, supplierID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    locationList.Add(reader["CarVendorLocationCode"].ToString());
                }
                conn.Close();
            }
            return locationList;
        }

        /// <summary>
        /// count , per vendor-AirportCode-CarVendorLocationCode code 
        /// </summary>
        /// <returns></returns>
        public static int getListCountWithAirportCodeAndCarVendorLocationCode(uint supplierID, string airportCode, string CarVendorLocationCode)
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                String sqlstring = "SELECT count(*) CarVendorLocationCode from CarVendorLocation "
                    + " where AirportCode = '{0}'and SupplierId = {1} and CarVendorLocationCode = '{2}'";

                cmd.CommandText = string.Format(sqlstring, airportCode, supplierID, CarVendorLocationCode);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    count = Convert.ToInt16(reader[0]);
                }
                conn.Close();
            }
            return count;
        }

        public static string getVendorLocationID(uint supplierID, string airportCode, string CarVendorLocationCode)
        {
            string carVendorLocationId = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                StringBuilder sqlstring = new StringBuilder("SELECT CarVendorLocationId from CarVendorLocation where 1=1 ");
                if(!string.IsNullOrEmpty(airportCode))
                    sqlstring.AppendFormat(" and AirportCode = '{0}'", airportCode);
                if(supplierID > 0)
                    sqlstring.AppendFormat(" and SupplierId = {0}", supplierID);
                if (!string.IsNullOrEmpty(CarVendorLocationCode))
                    sqlstring.AppendFormat(" and CarVendorLocationCode = '{0}'", CarVendorLocationCode);

                cmd.CommandText = sqlstring.ToString();
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    carVendorLocationId = reader["CarVendorLocationId"].ToString();
                }
                conn.Close();
            }
            return carVendorLocationId;
        }

        /// <summary>
        ///  GetCarVendorLocation with CarVendorLocationID 
        /// </summary>
        /// <returns></returns>
        public static CarVendorlocation GetCarVendorLocationWithCarVendorLocationID(string CarVendorLocationID)
        {
            CarVendorlocation location = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                String sqlstring = "SELECT * from CarVendorLocation where CarVendorLocationID = '{0}'";

                cmd.CommandText = string.Format(sqlstring, CarVendorLocationID);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    location = new CarVendorlocation(Convert.ToInt32(reader["CarVendorLocationID"].ToString().Trim()), reader["AirportCode"].ToString().Trim(),
                        reader["CarVendorLocationCode"].ToString().Trim(), reader["CarVendorLocationName"].ToString().Trim(), reader["StreetAddress1"].ToString().Trim(), 
                        reader["StateProvinceCode"].ToString().Trim(), reader["StateProvinceName"].ToString().Trim(), reader["CityName"].ToString().Trim(), 
                        reader["ISOCountryCode"].ToString().Trim(), Convert.ToInt32(reader["SupplierID"]), reader["LocationTypeID"],reader["PostalCode"], 
                        reader["FaxNumber"], reader["Latitude"], reader["Longitude"], reader["CarShuttleCategoryID"].ToString()
                        , reader["DeliveryBool"].ToString(), reader["CollectionBool"].ToString(), reader["OutOfOfficeHoursBool"].ToString(),
                        reader["PhoneNumber"].ToString());
                    location.LastUpdatedBy = Convert.ToString(reader["LastUpdatedBy"]);
                    location.UpdateDate = Convert.ToString(reader["UpdateDate"]);
                }
                conn.Close();
            }
            return location;
        }

        /// <summary>
        ///  GetCarVendorLocations with CarVendorLocationID 
        /// </summary>
        /// <returns></returns>
        public static List<CarVendorlocation> GetCarVendorlocations(string locationCode)
        {
            CarVendorlocation location = null;
            List<CarVendorlocation> locations = new List<CarVendorlocation>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                var sqlstring = "Select * from CarVendorLocation where AirportCode = '{0}'";

                cmd.CommandText = string.Format(sqlstring, locationCode);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    location = new CarVendorlocation(Convert.ToInt32(reader["CarVendorLocationID"].ToString().Trim()), reader["AirportCode"].ToString().Trim(),
                        reader["CarVendorLocationCode"].ToString().Trim(), reader["CarVendorLocationName"].ToString().Trim(), reader["StreetAddress1"].ToString().Trim(),
                        reader["StateProvinceCode"].ToString().Trim(), reader["StateProvinceName"].ToString().Trim(), reader["CityName"].ToString().Trim(),
                        reader["ISOCountryCode"].ToString().Trim(), Convert.ToInt32(reader["SupplierID"]), reader["LocationTypeID"], reader["PostalCode"],
                        reader["FaxNumber"], reader["Latitude"], reader["Longitude"]);
                    location.LastUpdatedBy = Convert.ToString(reader["LastUpdatedBy"]);
                    location.UpdateDate = Convert.ToString(reader["UpdateDate"]);

                    locations.Add(location);
                }
                conn.Close();
            }
            return locations;
        }

        public static bool CheckAirportCodeInCarsInventoryAirportTable(string airportCode)
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
                    existBool = ((int)reader[0])>0?true:false;
                    break;
                }
                conn.Close();
            }

            return existBool;
        }

        // support Loyalty number
        public static bool GetConfigurationFormatOverrideBool(uint supplySubSetID)
        {
            //string database = GetDatabaseFromConnectionString(connectionString);
            //DataTable dt = new DataTable();
            bool overrideBool =false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select ccf.CarRateCodeFormat,ccf.CarCorpDiscFormat,"+
                "ccf.CarSupplementalInfoFormat,ccf.CarVoucherNumberFormat,ccf.CarTourIDFormat,"+
                "sstwm.IATAOverrideBooking from SupplySubSetToWorldSpanSupplierItemMap as sstwm "+
                "join SupplySubsetToCarConfigurationFormatMap as sscf on sstwm.SupplySubsetID = sscf.SupplySubsetID"+
                " and sscf.SupplySubsetID = {0} join CarConfigurationFormat as ccf "+
                " on ccf.CarConfigurationFormatID=sscf.CarConfigurationFormatID", supplySubSetID);
                Console.WriteLine("cmd.CommandText: " + cmd.CommandText);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if(reader["CarRateCodeFormat"].ToString()!=""||
                        reader["CarCorpDiscFormat"].ToString() != "" ||
                        reader["CarSupplementalInfoFormat"].ToString() != "" ||
                        reader["CarVoucherNumberFormat"].ToString() != "" ||
                        reader["CarTourIDFormat"].ToString() != "" ||
                        reader["IATAOverrideBooking"].ToString() != "")
                    {
                        overrideBool = true;
                    }
                    Console.WriteLine(string.Format("CarRateCodeFormat = {0},CarCorpDiscFormat={1},CarSupplementalInfoFormat={2},"+
                        "CarVoucherNumberFormat={3},CarTourIDFormat={4},IATAOverrideBooking={5}",
                        reader["CarRateCodeFormat"].ToString(),
                        reader["CarCorpDiscFormat"].ToString(),
                        reader["CarSupplementalInfoFormat"].ToString(),
                        reader["CarVoucherNumberFormat"].ToString(),
                        reader["CarTourIDFormat"].ToString(),
                        reader["IATAOverrideBooking"].ToString()));
                    //break;
                }
                conn.Close();
            }
            return overrideBool;
        }

        public static string GetCarBehaviorAttributValue(uint supplierID,uint supplySubsetID,uint CarBehaviorAttID)
        {
            string attributValue = "";
           
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select cba.AttributeValue from CarBehaviorAttributeValue as cba " +
                        " where SupplierID = {0} and SupplySubsetID in ({1},0) and CarBehaviorAttributeID = {2}",
                        supplierID, supplySubsetID, CarBehaviorAttID);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    attributValue = reader["AttributeValue"].ToString();
                }
                conn.Close();
            }
            return attributValue;
        }

        public static DataTable GetCarSupplierIDAndSupplySubsetIDByBusiness(uint businessID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string cmdStr = " select distinct ci.supplierID, cis.SupplySubsetID " +
                    " from dbo.CarProductCatalogChildCarItem cpccci" +
                    " join dbo.CarProductCatalogChild cpcc on cpcc.CarProductCatalogChildID = cpccci.CarProductCatalogChildID" +
                    " join dbo.CarItemParentChild cipc on cipc.ParentCarItemID = cpccci.CarItemID" +
                    " join dbo.CarItem ci on ci.CarItemID = cipc.ChildCarItemID" +
                    " join dbo.CarItemSupplySubsetRank cis on cis.CarItemID = cipc.ChildcarItemID" +
                    " join dbo.SupplySubSetToWorldSpanSupplierItemMap ssw on ssw.SupplySubsetID = cis.SupplySubsetID" +
                    " where 1=1 and cpccci.ActiveBool=1 and ci.CarBusinessModelID = {0} " +//GDSP or Agency
                    " and (ci.SupplierID < 999 or ci.SupplierID = 1040)" + //-- only WSPN suppliers
                    " union " +//-- NOT PACKAGE 
                    " select distinct ci.supplierID, cis.SupplySubsetID" +
                    " from dbo.CarProductCatalogChildCarItem cpccci" +
                    " join dbo.CarProductCatalogChild cpcc on cpcc.CarProductCatalogChildID = cpccci.CarProductCatalogChildID" +
                    " join dbo.CarItem ci on ci.CarItemID = cpccci.CarItemID" +
                    " join dbo.CarItemSupplySubsetRank cis on cis.CarItemID = ci.CarItemID" +
                    " join dbo.SupplySubSetToWorldSpanSupplierItemMap ssw on ssw.SupplySubsetID = cis.SupplySubsetID" +
                    " where 1=1" +
                    " and cpccci.ActiveBool=1" +
                    " and ci.CarBusinessModelID = {1} " +
                    " and (ci.SupplierID < 999 or ci.SupplierID = 1040) ";//-- only WSPN suppliers
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format(cmdStr, businessID, businessID);
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        public static DataTable LoopCarBehavirAttributevalue(uint supplierID, uint supplySubsetID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select cba.AttributeValue from CarBehaviorAttributeValue as cba " +
                        " where SupplierID = {0} and SupplySubsetID = {1} and CarBehaviorAttributeID in (6,12,18)",
                        supplierID, supplySubsetID);

                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
            }
            return dt;
        }

        #region -- Media Search --
        public static DataTable CarMediaConfigurationListGet()
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandText = string.Format("exec CarMediaConfigurationLst#01");
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(ds);
                conn.Close();
            }
            /*
             * return the second table which includes the following columns :
             * CarVendorCode, CountryCode, AirportCode, CarCategoryID, CarTypeID, CarTransmissionDriveID, CarFuelAirConditionID, 
             * ConfigurationRank, CarMediaID,ThumbNailFullMediaFileName, SmallFullMediaFileName, ThumbNailProcessedBool, SmallProcessedBool, 
             * NumberOfDoorsMin, NumberOfDoorsMax, NumberOfPassengersAdult, NumberOfPassengersChild, NumberOfSuitcasesLarge, NumberOfSuitcasesSmall
             */
            return ds.Tables.Count > 1 ? ds.Tables[1] : ds.Tables[0];
        }

        public static void GetMakeModelValue(uint carMediaID, string langID, out string makeModel, out string carFeatureInfo)
        {
            makeModel = string.Empty;
            carFeatureInfo = string.Empty;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select * from CarMediaDataLoc where CarMediaID = {0} and LangID = {1}",
                        carMediaID, langID);

                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    makeModel = reader["MakeModel"].ToString();
                    carFeatureInfo = reader["CarFeatureInfo"].ToString();
                }
                conn.Close();
            }
        }
        #endregion

        public static bool GetPrepaidBoolFromCarItem(uint carItemID)
        {
            bool prepaidBool = false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                String sqlstring = string.Format(" select PrepaidBool from CarItem where CarItemID  = '{0}'", carItemID) ;

                cmd.CommandText = sqlstring;
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    prepaidBool = Convert.ToBoolean(reader["PrepaidBool"].ToString());
                   
                    break;
                }
                conn.Close();
            }

            return prepaidBool;
        }

        public static Dictionary<uint, string> GetPrepaidBoolFromCarItem(string carItemIDList)
        {
            Dictionary<uint, string> carItemIDAndPrePaidValues = new Dictionary<uint, string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                String sqlstring = string.Format(" select CarItemID, PrepaidBool, CarBusinessModelID from CarItem where CarItemID  in ({0})", carItemIDList);
                Console.WriteLine(sqlstring);
                cmd.CommandText = sqlstring;
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    uint carItemID = Convert.ToUInt32((int)reader["CarItemID"]);
                    bool prePaidBool = Convert.ToBoolean(reader["PrepaidBool"].ToString()); //(reader["PrepaidBool"].ToString() == "1" ? true : false);
                    string merchentOfRecord = (int)reader["CarBusinessModelID"] == 1 ? "Supplier" : "Expedia";
                    carItemIDAndPrePaidValues.Add(carItemID, prePaidBool + "-" + merchentOfRecord);
                 
                }
                conn.Close();
            }

            return carItemIDAndPrePaidValues;
        }

        public static int getMaxCarVendorLocationID()
        {
            int maxCarVendorLocationID = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                string sqlstring = "select maxCarVendorLocationID = MAX(CarVendorLocationID) + 1 from CarVendorLocation";
                cmd.CommandText = sqlstring;
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    maxCarVendorLocationID = (int)reader["maxCarVendorLocationID"];
                }
                conn.Close();
            }

            return maxCarVendorLocationID;
        }

        public static CarLocationKey getCarVendorLocationDetailFromID(uint CarVendorLocationID)
        {
            CarLocationKey location = null;
            Regex reg = new Regex(@"[A-Za-z]{1}[0-9]{1,3}");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                string sqlstring = String.Format("select AirportCode,CarVendorLocationCode from CarVendorLocation where CarVendorLocationID = {0}", CarVendorLocationID);
                cmd.CommandText = sqlstring;
                cmd.CommandTimeout = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if(location == null)
                        location = new CarLocationKey();

                    location.LocationCode = (string)reader["AirportCode"];
                    location.CarVendorLocationID = CarVendorLocationID;
                    string CarVendorLocationCode = (string)reader["CarVendorLocationCode"];
                    if (reg.IsMatch(CarVendorLocationCode))
                    {
                        location.CarLocationCategoryCode = CarVendorLocationCode.Substring(0, 1);
                        location.SupplierRawText = CarVendorLocationCode.Substring(1);
                    }
                }
                conn.Close();
            }

            return location;
        }


        #region CarItemSupplySubsetRand
        public static CarItemSupplySubsetRank getCarItemSupplySubsetRank(uint carItemID)
        {
            CarItemSupplySubsetRank carItemSupplySubsetRank = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select CarItemSupplySubsetRankID, CarItemID, SupplySubsetID, CarSupplyItemRank from CarItemSupplySubsetRank where CarItemID={0}", carItemID);
                
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    carItemSupplySubsetRank = new CarItemSupplySubsetRank();
                    carItemSupplySubsetRank.CarItemID = carItemID;
                    carItemSupplySubsetRank.CarItemSupplySubsetRankID = Convert.ToUInt16(reader["CarItemSupplySubsetRankID"]);
                    carItemSupplySubsetRank.SupplySubsetID = Convert.ToInt32(reader["SupplySubsetID"]);
                    carItemSupplySubsetRank.CarSupplyItemRank = Convert.ToUInt16(reader["CarSupplyItemRank"]);
                    break;
                }
                conn.Close();
            }
            catch (Exception e)
            {
                if (null != conn) conn.Close();
                
                throw new Exception( e.Message + e.StackTrace);
            }
            return carItemSupplySubsetRank;
        }

        public class CarItemSupplySubsetRank
        {
            public uint CarItemSupplySubsetRankID { get; set; }
            public uint CarItemID { get; set; }
            public int SupplySubsetID { get; set; }
            public uint CarSupplyItemRank { get; set; }
        }
        #endregion

    }
}