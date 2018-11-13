using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Linq;
using System.IO;
using ScreenScraper.Model;

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
            var collectionPage = new string("https://www.onthewateroutfitters.com/collection/page{0}.html");
            int collectionPageCount = 1;       
            HttpClient client = new HttpClient();
            SortedList<string, string> productList = new SortedList<string, string>();


            for (int pageNumber = 1; pageNumber <= collectionPageCount; pageNumber++)
            {
                var response = await client.GetAsync(string.Format(collectionPage, pageNumber));
                var pageContents = await response.Content.ReadAsStringAsync();
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);

                var productlinks = pageDocument.DocumentNode.SelectNodes("//a")
                                                            .Where(url => url.InnerHtml.Contains("title"))
                                                            .ToList();    
                foreach (var link in productlinks)
                {
                    if (!productList.ContainsKey(link.Attributes["title"].Value))
                    {
                        productList.Add(link.Attributes["title"].Value,link.Attributes["href"].Value);
                    }
                }               
            }  

            List<Product> products = new List<Product>();
            foreach (var productPage in productList)
            {
                var response = await client.GetAsync(productPage.Value);
                var pageContents = await response.Content.ReadAsStringAsync();
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                var product = new Product();
                product.Title = pageDocument.DocumentNode.SelectNodes("//h1")
                                                         .First()
                                                         .InnerText;

                // page info active, get the HTML describing the product
                if (pageDocument.DocumentNode.SelectNodes("//div[@class='page info active']/ul") != null)
                {
                    product.Description = pageDocument.DocumentNode.SelectNodes("//div[@class='page info active']/ul")
                                                                .First()
                                                                .InnerHtml;
                }

                // Images
                var productImages = pageDocument.DocumentNode.SelectNodes("//div[@class='images']/a")                                                            
                                                             .ToList();
    

                product.Images = new List<Image>();
                //Console.WriteLine("Title: " + product.Title);
                //Console.WriteLine("Description: " + product.Description);                 

                foreach (var img in productImages)
                {
                    Console.WriteLine("=======================================================");
                    Console.WriteLine(img.InnerHtml);
                    var firstImage = img.SelectNodes("//img").First();
                    var image = new Image();
                    image.ImageId = img.Attributes["data-image-id"].Value;
                    image.Title = firstImage.Attributes["alt"].Value; 
                    image.Src = firstImage.Attributes["src"].Value; ;
                    image.Alt = firstImage.Attributes["alt"].Value; ;
                    product.Images.Add(image);
                    Console.WriteLine("id: "+image.ImageId);
                    Console.WriteLine("title: "+image.Title);
                    Console.WriteLine("alt: "+image.Alt);
                }

                // <label for="product_configure_option_color"
                // Colors

                // <label for="product_configure_option_size"
                // Sizes

                // Categories
                
            }  
            Console.ReadLine();
        }      
        private static void DownloadImage(string folderImagesPath, Uri url, WebClient webClient)
        {
            try
            {
                webClient.DownloadFile(url, Path.Combine(folderImagesPath, Path.GetFileName(url.ToString())));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
