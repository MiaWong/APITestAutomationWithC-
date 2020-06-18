using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expedia.CarInterface.CarServiceTest.DataCollectionObjects
{
    public class MessageDataItem
    {
        public String name { get; set; }
        public Object value { get; set; }

        public MessageDataItem()
        {
        }

        public MessageDataItem(String name, Object value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
