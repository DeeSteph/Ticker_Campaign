// See https://aka.ms/new-console-template for more information
using Ticker_Campaign;
using System.Linq;
using System.Text.Json;

string message = string.Empty;

var events = new List<Event>{
new Event{ Name = "Phantom of the Opera", City = "New York", Price = 100},
new Event{ Name = "Metallica", City = "Los Angeles", Price = 110 },
new Event{ Name = "Metallica", City = "New York", Price = 120 },
new Event{ Name = "Metallica", City = "Boston", Price = 130},
new Event{ Name = "LadyGaGa", City = "New York", Price = 120 },
new Event{ Name = "LadyGaGa", City = "Boston", Price = 150},
new Event{ Name = "LadyGaGa", City = "Chicago", Price = 160},
new Event{ Name = "LadyGaGa", City = "San Francisco", Price = 120},
new Event{ Name = "LadyGaGa", City = "Washington", Price = 130}
};

Dictionary<string, int> distanceDictionary = new Dictionary<string, int>();
Dictionary<string, string> eventDictionary = new Dictionary<string, string>();
Dictionary<string, int> priceDictionary = new Dictionary<string, int>();

var customers = new List<Customer>{
new Customer{ Name = "Nathan", City = "New York"},
new Customer{ Name = "Bob", City = "Boston"},
new Customer{ Name = "Cindy", City = "Chicago"},
new Customer{ Name = "Lisa", City = "Los Angeles"},
new Customer {Name = "John Smith"}
};

var emailObjs = new List<EmailDto>();
//1. find out all events that arein cities of customer
// then add to email.
//var customer = new Customer { Name = "Mr. Fake", City = "New York" };

//var query = from result in customer
//            where result.Contains("New York")
//            select result;

Console.WriteLine("Please enter event price to filter within the price range:");
var filterPrice = Console.ReadLine();

filterPrice = filterPrice == "" ? "0" : filterPrice;

// 1. TASK
foreach (var item in customers)
{
    var customer = new Customer { Name = item.Name, City = item.City };
    emailObjs.Clear();
    if (string.IsNullOrEmpty(item.Name) || string.IsNullOrEmpty(item.City))
    {
        var emailobj = new EmailDto
        {
            Name = "No information found for client because no city was found",
        };
        emailObjs.Add(emailobj);
    }
    else
    {
        foreach (var ev in events)
        {

            if (eventDictionary.ContainsKey(item.City == null ? "NoCity" : item.City.Trim() + ":" + ev.City.Trim()) || eventDictionary.ContainsKey(ev.City.Trim() + ":" + item.City == null ? "NoCity" : item.City.Trim()))
            {
                var emailData = JsonSerializer.Deserialize<EmailDto>(eventDictionary[item.City.Trim() + ":" + ev.City.Trim()]);
                var price = 0;
                if (priceDictionary.ContainsKey(ev.Name.Trim() + ":" + ev.City.Trim()))
                {
                    price = priceDictionary[ev.Name.Trim() + ":" + ev.City.Trim()];
                }
                else
                {
                    price = GetPrice(ev);
                    priceDictionary.Add(ev.Name.Trim() + ":" + ev.City.Trim(), price);
                }
                emailData.Name = ev.Name;
                emailObjs.Add(emailData);
            }
            else
            {
                var result = AddToEmail(customer, ev, distanceDictionary: distanceDictionary, priceDictionary: priceDictionary);
                if (result.Distance == -1)
                {
                    Console.Clear();
                    Console.WriteLine("500: The system has encountered internal server error, please contact your administrator.");
                    return;
                }
                var emailobj = new EmailDto
                {
                    Name = ev.Name,
                    City = result.City,
                    Distance = result.Distance,
                    Price = result.Price,
                };
                emailObjs.Add(emailobj);

                if (!eventDictionary.ContainsKey(item.City == null ? "NoCity" : item.City + ":" + ev.City.Trim()) || !eventDictionary.ContainsKey(ev.City.Trim() + ":" + item.City == null ? "NoCity" : item.City.Trim()))
                {
                    eventDictionary.Add(item.City == null ? "NoCity" : item.City.Trim() + ":" + ev.City.Trim(), JsonSerializer.Serialize(emailobj));
                }
            }
        }

    }


    
    if (Convert.ToInt32(filterPrice) == 0 && filterPrice.ToString() != "")
    {
        emailObjs = emailObjs.ToList().OrderBy(x => x.Distance).Take(5).ToList();
    }
    else if (Convert.ToInt32(filterPrice) > 0 && filterPrice.ToString() != "")
    {
        emailObjs = emailObjs.ToList().OrderBy(x => x.Distance).Where(i => i.Price <= Convert.ToInt32(filterPrice)).Take(5).ToList();
    }
    else
    {
        emailObjs = emailObjs.ToList().OrderBy(x => x.Distance).Take(5).ToList();
    }
    SendMail(emailObjs, customer);
}

/*
* We want you to send an email to this customer with all events in their city
* Just call AddToEmail(customer, event) for each event you think they should get
*/

static void SendMail(List<EmailDto> emailDtos, Customer customer)
{
    Console.WriteLine($"Dear {customer.Name}," + "\n" + "We have compiled lots of events for you and your family for this season." + "\n" +
        $"Please find below the list of our upcoming events in and close to {customer.City} you with their prices." + "\n"
        + JsonSerializer.Serialize(emailDtos) + "\n\n");
}
Console.ReadLine();

static EmailDto AddToEmail(Customer c, Event e, Dictionary<string, int> distanceDictionary = null, Dictionary<string, int> priceDictionary = null)
{
    var distance = 0;
    var price = 0;
    if (distanceDictionary.ContainsKey(c.City.Trim() + ":" + e.City.Trim()))
    {
        distance = distanceDictionary[c.City.Trim() + ":" + e.City.Trim()];
    }
    else
    {
        distance = GetDistance(c.City, e.City);
        distanceDictionary.Add(c.City.Trim() + ":" + e.City.Trim(), distance);
    }
    if (priceDictionary.ContainsKey(e.Name.Trim() + ":" + e.City.Trim()))
    {
        price = price = priceDictionary[e.Name.Trim() + ":" + e.City.Trim()];
    }
    else
    {
        price = GetPrice(e);
        priceDictionary.Add(e.Name.Trim() + ":" + e.City.Trim(), price);
    }


    return new EmailDto
    {
        City = e.City,
        Distance = distance > 0 ? distance : 0,
        Price = price,
    };
}


static int GetPrice(Event e)
{
    return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
}

static int GetDistance(string fromCity, string toCity)
{
    return AlphebiticalDistance(fromCity, toCity);
}

static int AlphebiticalDistance(string s, string t)
{
    var result = 0;
    try
    {
        var i = 0;
        for (i = 0; i < Math.Min(s.Trim().Length, t.Trim().Length); i++)
        {
            // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
            result += Math.Abs(s.Trim()[i] - t.Trim()[i]);
        }
        for (; i < Math.Max(s.Trim().Length, t.Trim().Length); i++)
        {
            // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
            result += s.Trim().Length > t.Trim().Length ? s.Trim()[i] : t.Trim()[i];
        }
    }
    catch (Exception ex)
    {
        result = -1;
    }

    return result;
}



