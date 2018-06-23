using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

//Publish self-contained EXE...
//https://blogs.msdn.microsoft.com/luisdem/2017/03/19/net-core-1-1-how-to-publish-a-self-contained-application/

namespace StockQuoteDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            // Create the configuration object that the application will
            // use to retrieve configuration information.
            IConfigurationRoot configuration = builder.Build();
            string stockListPath = configuration["StockListPath"];
            string priceResultsPath = configuration["PriceResultsPath"];

            var doc = new HtmlDocument();
            doc.Load(stockListPath);

            List<string> symbols = new List<string>();
            List<decimal> prices = new List<decimal>();

            foreach (HtmlNode symbolNode in doc.DocumentNode.SelectNodes("//th[@class='column-cell row-symbol']//div[@class='item-symbol']")) 
            {
                string symbol = symbolNode.InnerHtml;

                if (!symbols.Contains(symbol))
                {
                    symbols.Add(symbol);
                }
            }

            decimal price;

            foreach (HtmlNode priceNode in doc.DocumentNode.SelectNodes("//span[@data-bind='text: price']"))
            {
                price = decimal.Parse(priceNode.InnerHtml);
                prices.Add(price);
            }

            //var doc = new HtmlDocument();
            //string html = @"";
            //doc.LoadHtml(html);

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            string address = @"https://www.apmex.com";
            HttpResponseMessage result = client.GetAsync(address).Result;
            result.EnsureSuccessStatusCode();
            string metalsContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            HtmlDocument metalsDocument = new HtmlDocument();
            metalsDocument.LoadHtml(metalsContent);

            IEnumerable<HtmlNode> metalPriceList = metalsDocument.DocumentNode.SelectNodes("//ul[@class='spotprice-embed']//a").Where(n => n.Attributes["href"].Value.Contains("spotprices")).ToList();

            foreach (HtmlNode metalPriceRef in metalPriceList)
            {
                string href = metalPriceRef.Attributes["href"].Value;

                //Get Gold price
                if (href.ToLower().Contains("gold"))
                {
                    symbols.Add("*Gold");
                    price = decimal.Parse(metalPriceRef.SelectSingleNode(".//span[@class='current']").InnerHtml.Replace("$", ""));
                    prices.Add(price);
                }
                //Get Silver price
                else if (href.ToLower().Contains("silver"))
                {
                    symbols.Add("*Silver");
                    price = decimal.Parse(metalPriceRef.SelectSingleNode(".//span[@class='current']").InnerHtml.Replace("$", ""));
                    prices.Add(price);
                }
                //Get Platinum price
                else if (href.ToLower().Contains("platinum"))
                {
                    symbols.Add("*Platinum");
                    price = decimal.Parse(metalPriceRef.SelectSingleNode(".//span[@class='current']").InnerHtml.Replace("$", ""));
                    prices.Add(price);
                }
            }
            
            //string content = Task.Run(async () => await result.Content.ReadAsStringAsync()).Result;
            /*
            client.GetAsync(address).ContinueWith(
               (requestTask) =>
               {
                    // Get HTTP response from completed task. 
                    HttpResponseMessage response = requestTask.Result;

                    // Check that response was successful or throw exception 
                    response.EnsureSuccessStatusCode();

                    //// Read response asynchronously as JsonValue and write out top facts for each country 
                    //response.Content.ReadAsAsync<JsonArray>().ContinueWith(
                    //   (readTask) =>
                    //   {
                    //       Console.WriteLine("First 50 countries listed by The World Bank...");
                    //       foreach (var country in readTask.Result[1])
                    //       {
                    //           Console.WriteLine("   {0}, Country Code: {1}, Capital: {2}, Latitude: {3}, Longitude: {4}",
                    //               country.Value["name"],
                    //               country.Value["iso2Code"],
                    //               country.Value["capitalCity"],
                    //               country.Value["latitude"],
                    //               country.Value["longitude"]);
                    //       }
                    //   });
               });
            */

            if (symbols.Count != prices.Count)
            {
                throw new DataMisalignedException("Missing pricing information.");
            }

            using (FileStream file = File.Create(priceResultsPath))
            {
                using (TextWriter csvFileWriter = new StreamWriter(file))
                {
                    int index = 0;

                    foreach (string symbol in symbols)
                    {
                        csvFileWriter.WriteLine($"{symbol},{prices.ElementAt(index).ToString(CultureInfo.InvariantCulture)}");
                        Console.WriteLine($"{symbol}, {prices.ElementAt(index++).ToString(CultureInfo.InvariantCulture)}");
                    }
                }
            }

            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }
    }
}
