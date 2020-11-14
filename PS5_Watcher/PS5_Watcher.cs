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
            //Adding the JSON file containing our secrets to the application
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSecrets.json", false, true);
            IConfigurationRoot configuration = builder.Build();
            //Class containing all our methods
            Utilities utility = new Utilities();
            
            var productAvailable = false;

            do
            {
                //Randomize retry timer to appear "Human". Honestly no human would refresh for more than 10hours.
                int retryTimer = utility.GetRetryTimer();

                Console.Write("Downloading URL HTML Content ");
                string htmlData = utility.HtmlContentAsStream(urlToWatch);
                Console.Write("| Finished downloading \n");
                /*    Checking the HTML data for a specific string that is present on the website if we can add to cart
                      No this program does not use any kind of "Artificial Intelligence" or "Machine Learning"
                      You could find the <a> tags or <class = ...> from other ecommerce site and replicate this functionality on those :wink wink:
                */    
                if (utility.CheckStringForSubstring(htmlData, "/fr/panier?add=") >= 0)
                {
                    productAvailable = true;
                }
                else
                {
                    Console.WriteLine($"Product is not available, retrying in {retryTimer} seconds");
                    //Thread sleep is in milliseconds, I don't want to type milliseconds times...
                    Thread.Sleep(retryTimer * 1000);
                }
            } while (productAvailable == false);

            Console.WriteLine("Item available, alerting user");
            Console.WriteLine("Opening website");
            utility.OpenUrl(urlToWatch);
            
            Console.WriteLine("Sending email alert");
            utility.SendMail(configuration);

            Console.WriteLine("Sending message alert");
            var messageInfos = utility.SendSMS(configuration);
            Console.WriteLine(messageInfos);

            Console.WriteLine($"Product was available at {DateTime.Now}. Alert has been sent to user");
            Console.WriteLine("Press a key to exit!");
            Console.ReadKey();
        }
    }
}