using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace screenScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        
        async static Task MainAsync(string[] args)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://www.onthewateroutfitters.com/collection/page1.html");
            var pageContents = await response.Content.ReadAsStringAsync();
            
            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);
            
            var productLinks = pageDocument.DocumentNode.SelectNodes("//div[contains(@class,'info')]");

            foreach (var link in productLinks)
            {
                Console.WriteLine(link.InnerText);
            }
           
            Console.ReadLine();
        }
    }
}
