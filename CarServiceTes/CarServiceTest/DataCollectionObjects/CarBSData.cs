using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expedia.CarInterface.CarServiceTest.DataCollectionObjects
{
    public class CarBSData
    {
        public List<CarProductData> carProductDataList;
        public List<MessageData> mesageDataList;

        public CarBSData()
        {
            this.carProductDataList = new List<CarProductData>();
            this.mesageDataList = new List<MessageData>();
        }
    }
}
