using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBusSenderProj
{
    public  class Order
    {
        public string? Color
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }

        public string? Priority
        {
            get;
            set;
        }
    }
}