using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticker_Campaign
{
    public class Event
    {
        public string Name { get; set; }
        public string City { get; set; }
        public decimal Price { get; set; }
    }
    public class Customer
    {
        public string Name { get; set; }
        public string City { get; set; }
    }

    public class EmailDto
    {
        public string Name { get; set; }
        public string City { get; set; }
        public int Distance { get; set; }
        public int Price { get; set; }
    }

}
