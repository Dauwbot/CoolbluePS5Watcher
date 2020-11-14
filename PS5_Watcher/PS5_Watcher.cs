using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace PS5_Watcher
{
    public class PS5_Watcher
    {
        private const string urlToWatch = "https://www.coolblue.be/fr/produit/865866/playstation-5.html";
        //private const string urlToWatch = "https://www.coolblue.be/fr/produit/828805/apple-airpods-2-avec-boitier-de-charge.htmll";

        public static void Main(string[] args)
        {
            Utilities utility = new Utilities();
            var productAvailable = false;

            do
            {
                int retryTimer = utility.GenerateRandomNumberInRange(40, 65);
                Console.Write("Downloading URL HTML Content ");
                string htmlData = utility.HtmlContentAsStream(urlToWatch);
                Console.Write("| Finished downloading \n");
                if (utility.CheckStringForSubstring(htmlData, "/fr/panier?add=") >= 0)
                {
                    productAvailable = true;
                }
                else
                {
                    Console.WriteLine($"Product is not available, retrying in {retryTimer} seconds");
                    Thread.Sleep(retryTimer * 1000);
                }
            } while (productAvailable == false);

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSecrets.json", false, true);
            IConfigurationRoot configuration = builder.Build();

            Console.WriteLine("Item available, alerting user");
            Console.WriteLine("Opening website");
            utility.OpenUrl(urlToWatch);
            
            Console.WriteLine("Sending email alert");
            utility.SendMail(configuration);

            Console.WriteLine("Sending message alert");
            var messageInfos = utility.SendSMS(configuration);
            Console.WriteLine(messageInfos);
        }
    }
}