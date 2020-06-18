using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Expedia.CarInterface.CarServiceTest.TestDataGenenator.TestConfigData;
using Expedia.CarInterface.CarServiceTest.ExceptionFacade;

namespace Expedia.CarInterface.CarServiceTest.DBOperation
{
    public class PosToWorldspanDefaultSegmentMap
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["CarWorldspanSCS"].ConnectionString;
        public static DataTable GetPosToWorldspanMapDataTable(TestDataBase testData)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();
                cmd.CommandText = string.Format("SELECT * FROM PoSToWorldspanDefaultSegmentMap WHERE JurisdictionCode = '{0}' "
                    + " and CompanyCode='{1}' and ManagementUnitCode = '{2}'",
                    testData.JurisdictionCountryCode,
                    testData.CompanyCode,
                    testData.ManagementUnitCode);
                cmd.CommandTimeout = 0;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                throw new CarException(CarErrorMessage.readDBTableError + " -[PoSToWorldspanDefaultSegmentMap] ", ex);
            }   
        }
    }
}
