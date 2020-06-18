using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Expedia.CarInterface.CarServiceTest.XSDObjects.E3.PlaceTypes.V4;
using Expedia.CarInterface.CarServiceTest.Util;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class TravServer
    {

        public static int getMaxTRL(int tpid)
        {
            int maxTRL = 0;
            using (SqlConnection conn = new SqlConnection(getConnectString(tpid)))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("select max(trl) maxtrl from ItineraryTravelServer where travelproductid = {0}", tpid);
                cmd.CommandTimeout = 0;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader[0].ToString() != "") maxTRL = int.Parse(reader[0].ToString());
                }
                conn.Close();
            }

            return maxTRL;

        }

        public static void itineraryTravelServerAdd(int tpid, uint tuid, int trl)
        {
            using (SqlConnection conn = new SqlConnection(getConnectString(tpid)))
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "ItineraryTravelServerAdd " + tpid + "," + trl + ", " + tuid + " ,'" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "',0, 0," + tuid;
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                conn.Close();
            }

        }

        //We need to get the correct connect string to support Int testing - different TravServer in EU and NA
        public static string getConnectString(int tpid)
        {
            string connString = "";
            //Get TravServer config if exist
            if (null != ConfigurationManager.ConnectionStrings["TravServer"]) connString = ConfigurationManager.ConnectionStrings["TravServer"].ConnectionString;
           
            //Get POS from tpid
            List<PointOfSaleKey> poses = CarsInventory.getPointOfSaleRecord(tpid);
            //Get travServer beanName based on POS
            string travSBeanName = ServiceConfigUtil.getCarBSConfig(ServiceConfigSettingName.TravServe_BeanName, poses[0]);
            //Get travServer name based on bean name
            if (travSBeanName.Contains("US"))
            {
                //Get TravServerUS config if exist
                if(null != ConfigurationManager.ConnectionStrings["TravServerUS"])
                    connString = ConfigurationManager.ConnectionStrings["TravServerUS"].ConnectionString;
                //If no TravServerUS and TravServer configed, throw an exception
                if (connString.Length == 0) throw new Exception("Please make sure TravServer or TravServerUS is congied for NA POS!");
            }
            else if (travSBeanName.Contains("EU"))
            {
                if (null != ConfigurationManager.ConnectionStrings["TravServerEU"])
                    connString = ConfigurationManager.ConnectionStrings["TravServerEU"].ConnectionString;
                //If no TravServerEU and TravServer configed, throw an exception
                if (connString.Length == 0) throw new Exception("Please make sure TravServer or TravServerEU is congied for EU POS!");
            }

            return connString;
        }
    }
}
