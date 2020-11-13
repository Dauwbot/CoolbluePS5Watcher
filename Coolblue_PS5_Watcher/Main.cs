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

            Console.WriteLine("Sending message alert");
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSecrets.json", optional: false, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            var twilioSettings = configuration.GetSection("TwilioSecrets");
            var accountSid = twilioSettings["AccountSid"];
            var accountToken = twilioSettings["AuthToken"];
            var senderNumber = twilioSettings["SenderNumber"];
            var receiverNumber = twilioSettings["ReceiverNumber"];
            
            TwilioClient.Init(accountSid, accountToken);

            MessageResource message = MessageResource.Create(
                body: "PS5 disponible sur Coolblue",
                from: new PhoneNumber(senderNumber),
                to: new PhoneNumber(receiverNumber)
            );
        }
    }
}