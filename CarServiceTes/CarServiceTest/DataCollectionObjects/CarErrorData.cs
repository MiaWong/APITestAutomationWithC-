using Expedia.CarInterface.CarServiceTest.Communication;
using Expedia.CarInterface.CarServiceTest.DBOperation;
using Expedia.CarInterface.CarServiceTest.TestDataGenenator.ActualDataExtractor.DataCollection;
using Expedia.CarInterface.CarServiceTest.TestDataGenenator.TestConfigData;
using Expedia.CarInterface.CarServiceTest.Util;
using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.CarTypes.V5;
using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.PlaceTypes.V4;
using Expedia.CarInterface.CarServiceTest.XSDObjects.S3.CarSCS.Reserve.V4;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web.Script.Serialization;

namespace Expedia.CarInterface.CarServiceTest.DataCollectionObjects
{
    public class CarErrorData
    {
        public string sourceName { get; set; }
        public string sourceId { get; set; }
        public PointOfSaleKey pointOfSaleKey { get; set; }
        public string supplierId { get; set; }
        public string errorText { get; set; }
        public string messageType { get; set; }
        public string carTypeCode { get; set; }
        public string carCategoryCode { get; set; }
        public CarPickupLocationKey pickupLocation { get; set; }

        public static List<CarErrorData> expCarErrorDataGenerator(CarSupplyConnectivityReserveRequest request
            , CarSupplyConnectivityReserveResponse response
            , TestDataErrHandle testDataErrHandle
            , string sourceName = "SCS", string sourceId = "1", string messageType = "Reserve")
        {
            List<CarErrorData> expDatas = new List<CarErrorData>();
            string[] errors = testDataErrHandle.errorDesc_GDS.Split(',');
            PointOfSaleKey pointOfSaleKey = new PointOfSaleKey()
            {
                JurisdictionCountryCode = request.PointOfSaleKey.JurisdictionCountryCode,
                CompanyCode = request.PointOfSaleKey.CompanyCode,
                ManagementUnitCode = request.PointOfSaleKey.ManagementUnitCode,
            };

            CarPickupLocationKey pickupLocationkey = new CarPickupLocationKey()
            {
                LocationCode = request.CarProduct.CarInventoryKey.CarCatalogKey.CarPickupLocationKey.LocationCode,
                CarLocationCategoryCode = request.CarProduct.CarInventoryKey.CarCatalogKey.CarPickupLocationKey.CarLocationCategoryCode,
                SupplierRawText = request.CarProduct.CarInventoryKey.CarCatalogKey.CarPickupLocationKey.SupplierRawText,
            };

            foreach (string error in errors)
            {
                CarErrorData expData = new CarErrorData()
                {
                    sourceName = sourceName,
                    sourceId = sourceId,
                    pointOfSaleKey = pointOfSaleKey,
                    supplierId = request.CarProduct.CarInventoryKey.CarCatalogKey.VendorSupplierID.ToString(),
                    errorText = error,
                    messageType = messageType,
                    carCategoryCode = request.CarProduct.CarInventoryKey.CarCatalogKey.CarVehicle.CarCategoryCode.ToString(),
                    carTypeCode = request.CarProduct.CarInventoryKey.CarCatalogKey.CarVehicle.CarTypeCode.ToString(),
                    pickupLocation = pickupLocationkey,
                };
                expDatas.Add(expData);
            }

            return expDatas;
        }

        /// <summary>
        /// Read and parse the Jason data for the specific request based on MessageGUID
        /// </summary>
        /// <param name="messageGUID"></param>
        public static List<CarErrorData>  readAndParseJsonData(string messageGUID
            , bool dataCollectionWrite
            , bool GZipOn
            , DateTime crsStartT
            , string crsQueryStr)
        {
            List<CarErrorData>  actDatas = new List<CarErrorData>();
            //Read all the data collected for the specific request if DataCollection.write/enable is off
            List<String> dataList = new List<string>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            if (!dataCollectionWrite)
            {
                //If dataCollectionWrite is not enable, get from service log
                dataList = FileManager.readDataCollected(messageGUID);
            }
            else
            {
                //Get the CRSLogging request                
                dataList = DataCollectionUtil.ReadJsonData(crsStartT, crsQueryStr, GZipOn, 1);                
            }

            foreach (string data in dataList)
            {
                try
                {
                    CarErrorData parseData = (CarErrorData)serializer.Deserialize(data, typeof(CarErrorData));
                    Console.WriteLine("originalJasonString=" + data);
                    actDatas.Add(parseData);
                }
                catch
                {
                    throw new Exception("Failed to parse Jason data: " + data.Replace("\n", ""));
                }              
            }
            return actDatas;
        }
      
    }  

}
