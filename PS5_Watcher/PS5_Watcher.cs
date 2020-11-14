using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PS5_Watcher
{
    public class PS5_Watcher
    {
        //https://www.coolblue.be/fr/produit/828805/apple-airpods-2-avec-boitier-de-charge.html;

        public static void Main(string[] args)
        {
            Dictionary<string, string[]> websites = new Dictionary<string, string[]>();
           websites.Add("Foo", new [] {"Url", "WordToFindInHtml"});

            //Adding the JSON file containing our secrets to the application
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSecrets.json", false, true);
            IConfigurationRoot configuration = builder.Build();
            //Class containing all our methods

            Parallel.ForEach(websites, website =>
            {
                Utilities utility = new Utilities();
                var productAvailable = false;

                do
                {
                    //Randomize retry timer to appear "Human". Honestly no human would refresh for more than 10hours.
                    int retryTimer = utility.GetRetryTimer();
                    
                    string htmlData = utility.HtmlContentAsStream(website.Value[0]);
                    /*    Checking the HTML data for a specific string that is present on the website if we can add to cart
                          No this program does not use any kind of "Artificial Intelligence" or "Machine Learning"
                          You could find the <a> tags or <class = ...> from other ecommerce site and replicate this functionality on those :wink wink:
                    */    
                    if (utility.CheckStringForSubstring(htmlData, website.Value[1]) >= 0)
                    {
                        productAvailable = true;
                    }
                    else
                    {
                        Console.WriteLine($"[{website.Key}] \tProduct is not available, retrying in {retryTimer} seconds");
                        //Thread sleep is in milliseconds, I don't want to type milliseconds times...
                        Thread.Sleep(retryTimer * 1000);
                    }
                } while (productAvailable == false);


                Console.WriteLine($"[{website.Key}]" +
                                  $"\tProduct available, alerting user");
                Console.Write($"\t\t" +
                              $"Opening website |");
                utility.OpenUrl(website.Value[0]);
            
                Console.Write(" Sending email alert |");
                utility.SendMail(configuration, website);

                Console.Write(" Sending message alert |");
                var messageInfos = utility.SendSMS(configuration, website);
                Console.Write(" " + messageInfos);

                Console.WriteLine($"\n" +
                                  $"[{website.Key}]" +
                                  $"\tProduct was available at {DateTime.Now}. Alert has been sent to user");
            });
        }
    }
}