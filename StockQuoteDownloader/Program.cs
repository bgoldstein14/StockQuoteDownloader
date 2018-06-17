using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockQuoteDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var doc = new HtmlDocument();
            doc.Load(@"c:\temp\htmlsample.txt");
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

            foreach (HtmlNode priceNode in doc.DocumentNode.SelectNodes("//span[@data-bind='text: price']"))
            {
                decimal price = decimal.Parse(priceNode.InnerHtml);
                prices.Add(price);
            }

            //var doc = new HtmlDocument();
            //string html = @"";
            //doc.LoadHtml(html);

            /*
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            //string address = @"https://www.usatoday.com/money/lookup/stocks/JHG/";
            string address = @"https://www.msn.com/en-us/money/stockdetails/fi-126.1.fxifx.NYS?symbol=fxifx";
            HttpResponseMessage result = client.GetAsync(address).Result;
            result.EnsureSuccessStatusCode();
            string content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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
