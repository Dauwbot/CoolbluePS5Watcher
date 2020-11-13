using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Coolblue_PS5_Watcher
{
    public class Coolblue_PS5_Watcher
    {
        private const int retryTimer = 45;

        private const string urlAddress = "https://www.coolblue.be/fr/produit/865866/playstation-5.html";
        //private const string urlAddress = "https://www.coolblue.be/fr/produit/828805/apple-airpods-2-avec-boitier-de-charge.htmll";

        public static void Main(string[] args)
        {
            Utilities baseUtily = new Utilities();
            var productAvailable = false;

            do
            {
                var htmlData = baseUtily.HtmlContentStream(urlAddress);
                if (baseUtily.CheckStringForSubstring(htmlData, "/fr/panier?add=") >= 0)
                {
                    productAvailable = true;
                }
                else
                {
                    Console.WriteLine($"Product is not available, retrying in {retryTimer} seconds");
                    Thread.Sleep(retryTimer * 1000);
                }
            } while (productAvailable == false);
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSecrets.json", optional: false, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            Console.WriteLine("Item available, alerting user");
            Console.WriteLine("Sending email alert");
            baseUtily.SendMail(configuration);

            Console.WriteLine("Sending message alert");
            string messageInfos = baseUtily.SendSMS(configuration);
            Console.WriteLine(messageInfos);
        }
    }
}