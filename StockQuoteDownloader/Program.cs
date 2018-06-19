using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
            string path = configuration["StockListPath"];

            var doc = new HtmlDocument();
            doc.Load(path);
            /*
            HtmlNode mainNode = doc.DocumentNode.SelectSingleNode("//tbody[@class='fin-table-body']");
            string mainNodeInnerHtml = mainNode.InnerHtml;
            HtmlNode node0 = mainNode.SelectSingleNode("//th");
            string node0InnerHtml = node0.InnerHtml;
            HtmlNode node1 = mainNode.SelectSingleNode("//th[class='column-cell row-symbol']");
            string node1InnerHtml = node1.InnerHtml;
            */

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
            //string address = @"https://www.usatoday.com/money/lookup/stocks/JHG/";
            string address = @"https://www.apmex.com";
            HttpResponseMessage result = client.GetAsync(address).Result;
            result.EnsureSuccessStatusCode();
            string metalsContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            HtmlDocument metalsDocument = new HtmlDocument();
            metalsDocument.LoadHtml(metalsContent);

            //Get Gold price
            HtmlNodeCollection metalPriceList = metalsDocument.DocumentNode.SelectNodes("//ul[@class='spotprice-embed']//a");
            //IEnumerable<HtmlNode> metalPriceRefs = metalPriceList.SelectNodes("//a").Where(n => n.Attributes["href"].Value.Contains("spotprices"));

            var a = metalPriceList.First(n => n.Attributes["href"].Value.ToLower().Contains("gold")).InnerHtml;

            decimal goldPrice = decimal.Parse(metalPriceList.First(n => n.Attributes["href"].Value.ToLower().Contains("gold")).SelectSingleNode("//ul[@class='spotprice-embed']//span[@class='current']").InnerHtml.Replace("$", ""));
            symbols.Add("*Gold");
            prices.Add(goldPrice);
            decimal silverPrice = decimal.Parse(metalPriceList.First(n => n.Attributes["href"].Value.ToLower().Contains("silver")).SelectSingleNode("//ul[@class='spotprice-embed']//span[@class='current']").InnerHtml.Replace("$", ""));
            symbols.Add("*Silver");
            prices.Add(silverPrice);
            decimal platinumPrice = decimal.Parse(metalPriceList.First(n => n.Attributes["href"].Value.ToLower().Contains("platinum")).SelectSingleNode("//ul[@class='spotprice-embed']//span[@class='current']").InnerHtml.Replace("$", ""));
            symbols.Add("*Platinum");
            prices.Add(platinumPrice);

            //HtmlNode silverPrice = metalPriceList.First(n => n.Attributes["href"].Value.ToLower().Contains("silver"));
            //HtmlNode platinumPrice = metalPriceList.First(n => n.Attributes["href"].Value.ToLower().Contains("platinum"));

            /*
            symbols.Add("*Gold");
            price = decimal.Parse(metalPriceList.SelectNodes("//ul[@class='spotprice-embed']//span[@class='current']").First(n => n.Attributes["href"].Value.ToLower().Contains("gold")).InnerHtml.Replace("$", ""));
            symbols.Add("*Silver");
            price = decimal.Parse(metalPriceList.SelectNodes("//ul[@class='spotprice-embed']//span[@class='current']").First(n => n.Attributes["href"].Value.ToLower().Contains("silver")).InnerHtml.Replace("$", ""));
            symbols.Add("*Platinum");
            price = decimal.Parse(metalPriceList.SelectNodes("//ul[@class='spotprice-embed']//span[@class='current']").First(n => n.Attributes["href"].Value.ToLower().Contains("platinum")).InnerHtml.Replace("$", ""));
            */
            /*
            foreach (HtmlNode metalPriceRef in metalPriceRefs)
            {
                string href = metalPriceRef.Attributes["href"].Value;

                if (href.ToLower().Contains("gold"))
                {
                    symbols.Add("*Gold");
                    decimal price = decimal.Parse(metalPriceRef.SelectNodes("//ul[@class='spotprice-embed']//span[@class='current']").First(n => n.Attributes["href"].Value.ToLower().Contains("gold")).InnerHtml.Replace("$", ""));
                    prices.Add(price);
                }
                else if (href.ToLower().Contains("silver"))
                {
                    symbols.Add("*Silver");
                    decimal price = decimal.Parse(metalPriceRef.SelectSingleNode("//ul[@class='spotprice-embed']//span[@class='current']").InnerHtml.Replace("$", ""));
                    prices.Add(price);
                }
                else if (href.ToLower().Contains("platinum"))
                {
                    symbols.Add("*Platinum");
                    decimal price = decimal.Parse(metalPriceRef.SelectSingleNode("//ul[@class='spotprice-embed']//span[@class='current']").InnerHtml.Replace("$", ""));
                    prices.Add(price);
                }
            }
            */
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

            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }
    }
}
