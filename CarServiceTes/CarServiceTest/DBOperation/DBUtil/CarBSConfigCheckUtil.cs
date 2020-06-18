using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Expedia.CarInterface.CarServiceTest.TestDataGenenator.TestConfigData;
using System.Configuration;
using Expedia.CarInterface.CarServiceTest.Util;
using System.Xml;

namespace Expedia.CarInterface.CarServiceTest.DBOperation.DBUtil
{
   
    public class ConfigSettingParameter
    {
        public string GetReservation_cancelledReservations { get; set; }
        public string Booking_augmentReservationWithDetails { get; set; }
        public string Reconstruct_useEnhancedBookingLogging { get; set; }
        public string Booking_enhancedBookingLogging { get; set; }
        public string Reconstruct_suppressCostAndPriceList { get; set; }
        public string Reconstruct_suppressDownstreamResponseErrors { get; set; }

        //For User Story798400 edit by Qiuhua
        public string PreparePurchase_detectPriceChange { get; set; } // Client config
        public string PreparePurchase_detectDownstreamReservePriceChange { get; set; } // Client config
        public string Booking_mergeDetailsInResponseReservation { get; set; } // Client config
        public string PriceChange_totalPriceTolerance { get; set; } // Pos config
        //For User Story 821816 edit by Qiuhua
        public string CurrencyConversion_useMoneyScale_enable { get; set; } //Client confgi
        public ConfigSettingParameter()
        {
            this.GetReservation_cancelledReservations = null;
            this.Booking_augmentReservationWithDetails = null;
            this.Reconstruct_useEnhancedBookingLogging = EnableKey.Enable_on;
            this.Booking_enhancedBookingLogging = EnableKey.Enable_off;
            this.Reconstruct_suppressCostAndPriceList = EnableKey.Enable_off;
            this.Reconstruct_suppressDownstreamResponseErrors = EnableKey.Enable_off;

            this.PreparePurchase_detectPriceChange = null;
            this.PreparePurchase_detectDownstreamReservePriceChange = null;
            this.Booking_mergeDetailsInResponseReservation = null;
            this.PriceChange_totalPriceTolerance = null;
            this.CurrencyConversion_useMoneyScale_enable = null;
        }

        public ConfigSettingParameter(string GetReservation_cancelledReservations,
            string Booking_augmentReservationWithDetails,string Reconstruct_useEnhancedBookingLogging,
            string Booking_enhancedBookingLogging, string Reconstruct_suppressCostAndPriceList,
            string Reconstruct_suppressDownstreamResponseErrors)
        {
            this.GetReservation_cancelledReservations = GetReservation_cancelledReservations;
            this.Booking_augmentReservationWithDetails = Booking_augmentReservationWithDetails;
            this.Reconstruct_useEnhancedBookingLogging = Reconstruct_useEnhancedBookingLogging;
            this.Booking_enhancedBookingLogging = Booking_enhancedBookingLogging;
            this.Reconstruct_suppressCostAndPriceList = Reconstruct_suppressCostAndPriceList;
            this.Reconstruct_suppressDownstreamResponseErrors = Reconstruct_suppressDownstreamResponseErrors;
        }
    }

    public class CarBSConfigCheckUtil
    {
        // EBL phase 2 : DB check client config or POS config setting value 
        public static void CheckDBConfigSettingValue(ConfigSettingParameter settingPara, string clientID, TestDataReserve testData, bool isUpdateIfNotMatch=false)
        {
            ///  ClientConfiguration table check with setting name and value
            if (settingPara.Booking_augmentReservationWithDetails != null)
            {
                CheckClientConfigFeatureEnable(clientID, ClientConfigurationSettingName.Booking_augmentReservationWithDetails_enable,
                    settingPara.Booking_augmentReservationWithDetails, tuid: testData.tuid, isUpdateIfNotMatch: isUpdateIfNotMatch);
            }
            if (settingPara.Reconstruct_useEnhancedBookingLogging != null)
            {
                CheckClientConfigFeatureEnable(clientID, ClientConfigurationSettingName.Reconstruct_useEnhancedBookingLogging_enable,
                    settingPara.Reconstruct_useEnhancedBookingLogging, tuid: testData.tuid, isUpdateIfNotMatch: isUpdateIfNotMatch);
            }
            if (settingPara.GetReservation_cancelledReservations != null)
            {
                CheckClientConfigFeatureEnable(clientID, ClientConfigurationSettingName.GetReservation_cancelledReservations_enable,
                    settingPara.GetReservation_cancelledReservations, tuid: testData.tuid, isUpdateIfNotMatch: isUpdateIfNotMatch);
            }



            /// PoSConfiguration table check with setting name and value
            if (settingPara.Booking_enhancedBookingLogging != null)
            {
                CheckPosConfigFeatureEnable(POSConfigurationSettingName.Booking_enhancedBookingLogging_enable,
                    settingPara.Booking_enhancedBookingLogging, testData, isUpdateIfNotMatch);
            }
            //if (settingPara.Reconstruct_suppressCostAndPriceList != null)
            //{
            //    CheckPosConfigFeatureEnable(POSConfigurationSettingName.Reconstruct_suppressCostAndPriceList_enable, 
            //        settingPara.Reconstruct_suppressCostAndPriceList,testData);
            //}
            //if (settingPara.Reconstruct_suppressDownstreamResponseErrors != null)
            //{
            //    CheckPosConfigFeatureEnable(POSConfigurationSettingName.Reconstruct_suppressDownstreamResponseErrors_enable,
            //        settingPara.Reconstruct_suppressDownstreamResponseErrors, testData);
            //}

            // Update for user story 397920 Migrate POS Config to Client Config  -- by v-moliu 11/18/2013
            if (settingPara.Reconstruct_suppressCostAndPriceList != null)
            {
                CheckClientConfigFeatureEnable(clientID, ClientConfigurationSettingName.Reconstruct_suppressCostAndPriceList_enable,
                    settingPara.Reconstruct_suppressCostAndPriceList, tuid: testData.tuid, isUpdateIfNotMatch: isUpdateIfNotMatch);
            }
            if (settingPara.Reconstruct_suppressDownstreamResponseErrors != null)
            {
                CheckClientConfigFeatureEnable(clientID, ClientConfigurationSettingName.Reconstruct_suppressDownstreamResponseErrors_enable,
                    settingPara.Reconstruct_suppressDownstreamResponseErrors, tuid: testData.tuid, isUpdateIfNotMatch: isUpdateIfNotMatch);
            }

            // Booking.augmentReservationWithDetails/enable == 1 
            // Booking.augmentReservationWithDetails.mergeDetailsInResponseReservation/enable = 1 
            //CheckClientConfigFeatureEnable(clientID, ClientConfigurationSettingName.Booking_augmentReservationWithDetails_enable,
            //       EnableKey.Enable_on, tuid: testData.tuid, isUpdateIfNotMatch: isUpdateIfNotMatch);
            //CheckClientConfigFeatureEnable(clientID, ClientConfigurationSettingName.Booking_mergeDetailsInResponseReservation,
            //       EnableKey.Enable_on, tuid: testData.tuid, isUpdateIfNotMatch: isUpdateIfNotMatch);

            // Edit by Qiuhua for use story 798400
            if (settingPara.PreparePurchase_detectPriceChange != null)
            {
                CheckClientConfigFeatureEnable(clientID, ClientConfigurationSettingName.PreparePurchase_detectPriceChange_enable,
                    settingPara.PreparePurchase_detectPriceChange, tuid: testData.tuid, isUpdateIfNotMatch: isUpdateIfNotMatch);
            }
            if (settingPara.PreparePurchase_detectDownstreamReservePriceChange != null)
            {
                CheckClientConfigFeatureEnable(clientID, ClientConfigurationSettingName.PreparePurchase_detectDownstreamReservePriceChange_enable,
                    settingPara.PreparePurchase_detectDownstreamReservePriceChange, tuid: testData.tuid, isUpdateIfNotMatch: isUpdateIfNotMatch);
            }
            if (settingPara.Booking_mergeDetailsInResponseReservation != null)
            {
                CheckClientConfigFeatureEnable(clientID, ClientConfigurationSettingName.Booking_mergeDetailsInResponseReservation,
                    settingPara.Booking_mergeDetailsInResponseReservation, tuid: testData.tuid, isUpdateIfNotMatch: isUpdateIfNotMatch);
            }
            if (settingPara.PriceChange_totalPriceTolerance != null)
            {
                CheckPosConfigFeatureEnable(POSConfigurationSettingName.PriceChange_totalPriceTolerance,
                    settingPara.PriceChange_totalPriceTolerance, testData, isUpdateIfNotMatch);
            }

            //Edit by Qiuhua for user story 821816
            if (settingPara.CurrencyConversion_useMoneyScale_enable != null)
            {
                CheckClientConfigFeatureEnable(clientID, ClientConfigurationSettingName.CurrencyConversion_useMoneyScale_enable,
                    settingPara.CurrencyConversion_useMoneyScale_enable, tuid: testData.tuid, isUpdateIfNotMatch: isUpdateIfNotMatch);
            }
        }

        // Common way to check the POS config setting value
        public static void CheckPosConfigFeatureEnable(string configSettingName, string expectedvalue, TestDataReserve testData, bool isUpdateIfNotMatch=false)
        {
            //create a configuration request data instance
            ConfigSettingRequestData configSettingRequestData = new ConfigSettingRequestData("CarBSUri", ConfigSettingType.POS);
            if (testData != null)
                configSettingRequestData.TUID = testData.tuid;
            string enableValue = null;
            // get the feature turned value from DB
            string envKey = ConfigurationManager.AppSettings["EnvironmentName"];
            ///1. mach as EnviromentName and POS's 3 values
            if (testData != null)
                enableValue = CarBSDB.GetServiceConfig(configSettingName, envKey, testData.JurisdictionCountryCode,
                    testData.CompanyCode, testData.ManagementUnitCode);
            if (enableValue != null)
            {
                // set configuration data by expectedValue where configName/envKey/JurisdictionCountryCode/CompanyCode/ManagementUnitCode have values
                configSettingRequestData.ConfigSettingDataAdd(new ConfigurationSettingData(env: envKey, sName: configSettingName, sValue: expectedvalue
                    , jCode: testData.JurisdictionCountryCode, cCode: testData.CompanyCode, mUnitCode: testData.ManagementUnitCode, cID: null, sID: null));
                //mach value 
                MachExpectedValueinDB(expectedvalue, enableValue, configSettingName, testData.tuid, isUpdateIfNotMatch, configSettingRequestData);
            }
            else
            {
                ///2. if 1 is null, mach only with EnviromentName
                enableValue = CarBSDB.GetServiceConfig(configSettingName, envKey);
                if (enableValue != null)
                {
                    // set configuration data by expectedValue where configName/envKey have values
                    configSettingRequestData.ConfigSettingDataAdd(new ConfigurationSettingData(env: envKey, sName: configSettingName, sValue: expectedvalue
                        , jCode: null, cCode: null, mUnitCode: null, cID: null, sID: null));
                    //mach value 
                    MachExpectedValueinDB(expectedvalue, enableValue, configSettingName, testData.tuid, isUpdateIfNotMatch, configSettingRequestData);
                }
                else
                {
                    ///3. if 2 is null, mach as EnviromentName is null
                    enableValue = CarBSDB.GetServiceConfig(configSettingName);
                    if (enableValue != null)
                    {
                        // set configuration data by expectedValue where configNam have value
                        configSettingRequestData.ConfigSettingDataAdd(new ConfigurationSettingData(env: null, sName: configSettingName, sValue: expectedvalue
                            , jCode: null, cCode: null, mUnitCode: null, cID: null, sID: null));
                        //mach value 
                        MachExpectedValueinDB(expectedvalue, enableValue, configSettingName, testData.tuid, isUpdateIfNotMatch, configSettingRequestData);
                    }
                    else
                    {
                        Assert.Fail("No enable feature value  for " + configSettingName + "in DB,please check the DB.");
                    }
                }
            }

        }

        public static string GetConfigFeature(string configSettingName, TestDataReserve testData, string hostNameKey = "CarBSUri", ConfigSettingType configType = ConfigSettingType.POS)
        {
            //create a configuration request data instance
            ConfigSettingRequestData configSettingRequestData = new ConfigSettingRequestData(hostNameKey, configType);
            configSettingRequestData.TUID = testData.tuid;
            string enableValue = null;
            // get the feature turned value from DB
            string envKey = ServiceConfigUtil.EnvNameGet();
            ///1. mach as EnviromentName and POS's 3 values
            enableValue = CarBSDB.GetServiceConfig(configSettingName, envKey, testData.JurisdictionCountryCode,
                testData.CompanyCode, testData.ManagementUnitCode);
            if (enableValue == null)            
            {
                ///2. if 1 is null, mach only with EnviromentName
                enableValue = CarBSDB.GetServiceConfig(configSettingName, envKey);
                if (enableValue == null)               
                {
                    ///3. if 2 is null, mach as EnviromentName is null
                    enableValue = CarBSDB.GetServiceConfig(configSettingName);
                    if (enableValue == null)                    
                    {
                        Assert.Fail("No enable feature value  for " + configSettingName + "in DB,please check the DB.");
                    }
                }
            }
            return enableValue;
        }

        //Common way to check the Client config setting value
        public static void CheckClientConfigFeatureEnable(string clientID, string configName, string expectedValue, bool null_envKey = false
            , string tuid = "0", bool isUpdateIfNotMatch = false)
        {
            //create a configuration request data instance
            ConfigSettingRequestData configSettingRequestData = new ConfigSettingRequestData("CarBSUri", ConfigSettingType.Client);
            configSettingRequestData.TUID = tuid;
            string envKey = null;
            if(!null_envKey)
                envKey =ConfigurationManager.AppSettings["EnvironmentName"];
            string actualValue = null;
            ///1. mach as EnviromentName and ClientID
            actualValue = CarBSDB.GetClientConfig(configName, envKey, clientID);            
            if (actualValue != null)
            {
                // set configuration data by expectedValue where configName/envKey/clientID have values
                configSettingRequestData.ConfigSettingDataAdd(new ConfigurationSettingData(env: envKey, sName: configName, sValue: expectedValue
                    , jCode: null, cCode: null, mUnitCode: null, cID: clientID, sID: null));
                //mach value 
                MachExpectedValueinDB(expectedValue, actualValue, configName, tuid, isUpdateIfNotMatch, configSettingRequestData);
            }
            else
            {
                ///2. if 1 is null, mach with EnviromentName
                actualValue = CarBSDB.GetClientConfig(configName, envKey);
                if (actualValue != null)
                {
                    // set configuration data by expectedValue where configName/envKey have values
                    configSettingRequestData.ConfigSettingDataAdd(new ConfigurationSettingData(env: envKey, sName: configName, sValue: expectedValue
                        , jCode: null, cCode: null, mUnitCode: null, cID: null, sID: null));
                    //mach value 
                    MachExpectedValueinDB(expectedValue, actualValue, configName, tuid, isUpdateIfNotMatch, configSettingRequestData);
                }
                else
                {
                    ///3. if 2 is null, mach as EnviromentName is null
                    actualValue = CarBSDB.GetClientConfig(configName);
                    if (actualValue != null)
                    {
                        // set configuration data by expectedValue where only configName have values
                        configSettingRequestData.ConfigSettingDataAdd(new ConfigurationSettingData(env: null, sName: configName, sValue: expectedValue
                            , jCode: null, cCode: null, mUnitCode: null, cID: null, sID: null));
                        MachExpectedValueinDB(expectedValue, actualValue, configName, tuid, isUpdateIfNotMatch, configSettingRequestData);
                    }
                    else
                    {
                        actualValue = "0";
                        MachExpectedValueinDB(expectedValue, actualValue, configName, tuid, isUpdateIfNotMatch, configSettingRequestData);
                    }
                }
            }          
        }

        //Common way to check the POS config setting value
        public static void CheckPOSConfigFeatureEnable(string POSCode, string CompanyCode, string ManagementUnitCode, string configName, string expectedValue, bool null_envKey = false
            , string tuid = "0", bool isUpdateIfNotMatch = false)
        {
            //create a configuration request data instance
            ConfigSettingRequestData configSettingRequestData = new ConfigSettingRequestData("CarBSUri", ConfigSettingType.POS);
            configSettingRequestData.TUID = tuid;
            string envKey = null;
            if (!null_envKey)
                envKey = ConfigurationManager.AppSettings["EnvironmentName"];
            string actualValue = null;
            ///1. mach as EnviromentName and ClientID
            actualValue = CarBSDB.GetPOSConfig(configName, envKey, POSCode, CompanyCode, ManagementUnitCode);
            if (actualValue != null)
            {
                // set configuration data by expectedValue where configName/envKey/POSCode/CompanyCode/ManagementUnitCode, have values
                configSettingRequestData.ConfigSettingDataAdd(new ConfigurationSettingData(env: envKey, sName: configName, sValue: expectedValue
                    , jCode: null, cCode: null, mUnitCode: null, cID: POSCode, sID: null));
                //mach value 
                MachExpectedValueinDB(expectedValue, actualValue, configName, tuid, isUpdateIfNotMatch, configSettingRequestData);
            }
            else
            {
                ///2. if 1 is null, mach with EnviromentName
                actualValue = CarBSDB.GetPOSConfig(configName, envKey);
                if (actualValue != null)
                {
                    // set configuration data by expectedValue where configName/envKey have values
                    configSettingRequestData.ConfigSettingDataAdd(new ConfigurationSettingData(env: envKey, sName: configName, sValue: expectedValue
                        , jCode: null, cCode: null, mUnitCode: null, cID: null, sID: null));
                    //mach value 
                    MachExpectedValueinDB(expectedValue, actualValue, configName, tuid, isUpdateIfNotMatch, configSettingRequestData);
                }
                else
                {
                    ///3. if 2 is null, mach as EnviromentName is null
                    actualValue = CarBSDB.GetPOSConfig(configName);
                    if (actualValue != null)
                    {
                        // set configuration data by expectedValue where only configName have values
                        configSettingRequestData.ConfigSettingDataAdd(new ConfigurationSettingData(env: null, sName: configName, sValue: expectedValue
                            , jCode: null, cCode: null, mUnitCode: null, cID: null, sID: null));
                        MachExpectedValueinDB(expectedValue, actualValue, configName, tuid, isUpdateIfNotMatch, configSettingRequestData);
                    }
                    else
                    {
                        actualValue = "0";
                        MachExpectedValueinDB(expectedValue, actualValue, configName, tuid, isUpdateIfNotMatch, configSettingRequestData);
                    }
                }
            }     
        }

        // isReturnConfigEnable  false: will assert.fail when have any dismatch
        public static bool MachExpectedValueinDB(string expectedValue, string actualValue, string Configname
            , string tuid = "0", bool isUpdateIfNotMatch = false, ConfigSettingRequestData configSettingRequestData = null, bool isReturnConfigEnable = false)
        {
            bool isMatchExpectedValue = false;
            if (expectedValue == actualValue)
            {
                Console.WriteLine("Read the expected feature value for " + Configname + " in DB is " + expectedValue);
                isMatchExpectedValue = true;
            }
            else
            {
                if (!isUpdateIfNotMatch)
                {
                    if (isReturnConfigEnable)
                        isMatchExpectedValue = false;
                    else
                        Assert.Fail("The feature value  for " + Configname + " in DB is" + actualValue + ", but the expected feature value " + expectedValue);
                }
                else
                {
                    Console.WriteLine("The feature value  for " + Configname + " in DB is" + actualValue + ", but the expected feature value " + expectedValue);
                    //update to expect value 
                    if (configSettingRequestData != null)
                    {
                        Console.WriteLine("Update : " + Configname + " from " + actualValue + " to " + expectedValue);   
                        //update configuration cache                                     
                        ConfigSettingHelper.ConfiguationCacheUpdate(configSettingRequestData);
                        //set value to actual value for rollback
                        configSettingRequestData.ConfigSettingDatasSettingValueReset(actualValue);
                        //add this data to rollback instance
                        RollBackConfigurationSettingHelper.Instance.Add(configSettingRequestData, tuid);
                    }
                    else
                    {
                        Assert.Fail("No configSettingRequestData data, can not update " + Configname + " from " + actualValue + " to " + expectedValue);
                    }
                }                
            }
            return isMatchExpectedValue;
        }
        

        // ASCS table - suppierConfig
        public static void CheckSupplierConfigFeatureEnable(string configSettingName, string expectedvalue, TestDataReserve testData, bool isUpdateIfNotMatch = false)
        {
            string enableValue = null;
            //create a configuration request data instance
            ConfigSettingRequestData configSettingRequestData = new ConfigSettingRequestData("CarBSUri", ConfigSettingType.Supplier);
            configSettingRequestData.TUID = testData.tuid;
            // get the feature turned value from DB
            string envKey = ConfigurationManager.AppSettings["EnvironmentName"];
            uint supplierID = CarsInventory.GetSupplyIDByVendorCode(testData.vendorCode);
            ConfigurationDBHelper configDb = new ConfigurationDBHelper(CarCommonEnumManager.ServieProvider.Amadeus, ConfigSettingType.Supplier);
            ///1. mach as EnviromentName and POS's 3 values
            enableValue = configDb.SettingValueSupplierGet(configSettingName, envKey, supplierID.ToString());
            if (enableValue != null)
            {
                // set configuration data by expectedValue where configName/envKey/supplierID have values
                configSettingRequestData.ConfigSettingDataAdd(new ConfigurationSettingData(env: envKey, sName: configSettingName, sValue: expectedvalue
                    , jCode: null, cCode: null, mUnitCode: null, cID: null, sID: supplierID.ToString()));
                //mach value 
                MachExpectedValueinDB(expectedvalue, enableValue, configSettingName, testData.tuid, isUpdateIfNotMatch, configSettingRequestData);
            }
            else
            {
                ///2. if 1 is null, mach withthout EnviromentName
                enableValue = configDb.SettingValueSupplierGet(configSettingName, null, supplierID.ToString());
                if (enableValue != null)
                {
                    // set configuration data by expectedValue where configName/supplierID have values
                    configSettingRequestData.ConfigSettingDataAdd(new ConfigurationSettingData(env: null, sName: configSettingName, sValue: expectedvalue
                        , jCode: null, cCode: null, mUnitCode: null, cID: null, sID: supplierID.ToString()));
                    //mach value 
                    MachExpectedValueinDB(expectedvalue, enableValue, configSettingName, testData.tuid, isUpdateIfNotMatch, configSettingRequestData);
                }
                else
                {
                    ///2. if 1 is null, mach withthout EnviromentName
                    enableValue = configDb.SettingValueSupplierGet(configSettingName, null, null);
                    if (enableValue != null)
                    {
                        // set configuration data by expectedValue where configName have values
                        configSettingRequestData.ConfigSettingDataAdd(new ConfigurationSettingData(env: null, sName: configSettingName, sValue: expectedvalue
                            , jCode: null, cCode: null, mUnitCode: null, cID: null, sID: null));
                        //mach value 
                        MachExpectedValueinDB(expectedvalue, enableValue, configSettingName, testData.tuid, isUpdateIfNotMatch, configSettingRequestData);
                    }
                    else
                    {
                        Assert.Fail("No enable feature value  for " + configSettingName + "in DB,please check the DB.");
                    }

                }
            }
        }

        public static string GetPosConfigFeatureValue(string configSettingName, TestDataReserve testData)
        {
            //create a configuration request data instance
            ConfigSettingRequestData configSettingRequestData = new ConfigSettingRequestData("CarBSUri", ConfigSettingType.POS);
            
            string enableValue = null;
            // get the feature turned value from DB
            string envKey = ConfigurationManager.AppSettings["EnvironmentName"];
            if (testData != null)
            {
                configSettingRequestData.TUID = testData.tuid;

                ///1. mach as EnviromentName and POS's 3 values
                enableValue = CarBSDB.GetServiceConfig(configSettingName, envKey, testData.JurisdictionCountryCode,
                    testData.CompanyCode, testData.ManagementUnitCode);
            }
            else 
                enableValue = CarBSDB.GetServiceConfig(configSettingName, envKey);

            if (enableValue == null)
            {
                ///2. if 1 is null, mach only with EnviromentName
                enableValue = CarBSDB.GetServiceConfig(configSettingName, envKey);
                if (enableValue == null)
                {
                    ///3. if 2 is null, mach as EnviromentName is null
                    enableValue = CarBSDB.GetServiceConfig(configSettingName);
                    if (enableValue == null)
                    {
                        Assert.Fail("No enable feature value  for " + configSettingName + "in DB,please check the DB.");
                    }
                }
            }
            return enableValue;
        }
    }

    public class CarBSDBSettingUpdateUtil
    {
        public string noPriceList { get; set; }
        public string noTotalPrice { get; set; }
        public string noRecord { get; set; }
        public string noPolicyList { get; set; }
        public string noDataMissing { get; set; }

        //public CarBSDBSettingUpdateUtil()
        //{
        //    this.noPoliceList = EnableKey.Enable_on;
        //    this.noTotalPrice = EnableKey.Enable_on;
        //    this.noRecord = null;
        //    this.noPriceList = null;
        //}

        // EBL phase 2 for DB updated
        public CarBSDBSettingUpdateUtil(string noPriceList=null, string noTotalPrice=null, string noRecord=null, string noPolicyList=null, string noDataMissing=null)
        {
            this.noPolicyList = noPolicyList;
            this.noTotalPrice = noTotalPrice;
            this.noRecord = noRecord;
            this.noPriceList = noPriceList;
            this.noDataMissing = noDataMissing;
        }

        // EBL phase 2 for DB updated
        public void CarBSDBConfigurationSettingUpdate(string bookingItemID)
        {
            if (this.noRecord != null)
            {
                if (this.noRecord == EnableKey.Enable_on)
                {
                    CarBSDB.RemovedEBLRecordByBooingItemID(bookingItemID);
                }
            }

            if (noPriceList != null)
            {
                if (this.noPriceList == EnableKey.Enable_on)
                {
                    CarBSDB.RemovedEBLAllPriceList(bookingItemID);
                }
            }
            if (noTotalPrice != null)
            {
                // if the priceList be removed , then don't need remove total price 
                if (this.noPriceList != EnableKey.Enable_on)
                {

                    if (this.noTotalPrice == EnableKey.Enable_on)
                    {
                        CarBSDB.RemovedEBLTotalPriceInPriceList(bookingItemID);
                    }
                }
            }
            // if record is exist , and then update policy
            if (noPolicyList != null && this.noRecord == null)
            {
                if (this.noPolicyList == EnableKey.Enable_on)
                {
                    CarBSDB.RemovedAndUpdateEBLForPoliceList(bookingItemID);
                }
            }
        }
    }

}
