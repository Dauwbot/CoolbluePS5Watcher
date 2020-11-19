using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PS5_Watcher
{
    public class PS5_Watcher
    {
        public static void Main(string[] args)
        {
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DirectoryInfo dir = new DirectoryInfo(directory ?? string.Empty);
            Directory.CreateDirectory(dir.FullName + "\\data");
            Directory.CreateDirectory(dir.FullName + "\\data\\screenshots");
            Directory.CreateDirectory(dir.FullName + "\\data\\screenshots\\original");
            Directory.CreateDirectory(dir.FullName + "\\data\\html");

            #region websites declaration
            Dictionary<string, string[]> websites = new Dictionary<string, string[]>();
            websites.Add("Mediamarkt",
                new[]
                {
                    //"https://www.mediamarkt.be/nl/product/_tv-samsung-uhd-4k-43-inch-ue43tu7020wxxn-1902171.html",
                    "https://www.mediamarkt.be/nl/product/_playstation-ps5-825-gb-9395201-1907411.html",
                    "servlet/MultiChannelOrderCatalogEntryAdd?storeId="
                });
            websites.Add("Bol.com",
                new []
                {
                    //"https://www.bol.com/nl/p/assassins-creed-valhalla-drakkar-edition-ps4/9300000001687275/?bltgh=osDheCtpKpf4kyTz9UFAyQ.1_46.47.ProductTitle",
                    "https://www.bol.com/nl/p/sony-playstation-5-console/9300000004162282/?language=nl-BE&country=BE&approved=true",
                    "addItems.html?productId=9300000004162282"
                });
            websites.Add("CDiscount",
                new []
                {
                    "https://www.cdiscount.com/jeux-pc-video-console/ps5/console-ps5/l-1035001.html#_his_",
                    "PLAYSTATION 5"
                });
             websites.Add("AmazonDE",
                 new []
                 {
                     //"https://www.amazon.de/-/en/dp/B07HHPX4N1/ref=sr_1_1?dchild=1&keywords=playstation+4&qid=1605615912&quartzVehicle=812-409&replacementKeywords=playstation&sr=8-1",
                     "https://www.amazon.de/-/en/dp/B08H93ZRK9/ref=nav_signin?dchild=1&keywords=playstation+5&qid=1605601079&sr=8-2&",
                     "Add to Basket",
                     "0"
                 });
            websites.Add("AmazonFR",
                new []
                {
                    //"https://www.amazon.fr/PS4-Black-Manette-2%C3%A8me-DualShock/dp/B07HHPX4N1/ref=sr_1_1?__mk_fr_FR=%C3%85M%C3%85%C5%BD%C3%95%C3%91&crid=P3PGYHRK5PIG&dchild=1&keywords=playstation+4&qid=1605615868&sprefix=playstat%2Caps%2C176&sr=8-1",
                    "https://www.amazon.fr/gp/product/B08H93ZRK9/ref=as_li_qf_asin_il_tl?ie=UTF8&tag=michapx7-21&creative=6746&linkCode=as2&creativeASIN=B08H93ZRK9&linkId=d8867619d3a0702f32b28c342ae022db",
                    "Ajouter au panier",
                    "0"
                });
            websites.Add("AmazonNL",
                new []
                {
                    //"https://www.amazon.nl/PlayStation-console-zwart-DualShock-Controller/dp/B01LX4669V?ref_=Oct_s9_apbd_obs_hd_bw_bHzL2zX&pf_rd_r=JEYBED70QASWV4C0VC5C&pf_rd_p=be10f740-f78b-5aba-ade0-b99c0e93e47b&pf_rd_s=merchandised-search-11&pf_rd_t=BROWSE&pf_rd_i=16480631031",
                    "https://www.amazon.nl/Sony-PlayStation-PlayStation%C2%AE5-Console/dp/B08H93ZRK9/ref=nav_ya_signin?dchild=1&pd_rd_r=c495b370-f525-4287-a4e0-67c0fb2d7f7e&pd_rd_w=EVip7&pd_rd_wg=5Rcpb&pf_rd_p=0a56d51a-3836-41ed-bef3-b0c27e77df24&pf_rd_r=PPFC4HEW443D4ER64ZH4&qid=1605601160&s=videogames&sr=1-1&",
                    "Nu kopen",
                    "0"
                });
                websites.Add("AmazonUK",
                    new []
                    {
                        //"https://www.amazon.nl/PlayStation-console-zwart-DualShock-Controller/dp/B01LX4669V?ref_=Oct_s9_apbd_obs_hd_bw_bHzL2zX&pf_rd_r=JEYBED70QASWV4C0VC5C&pf_rd_p=be10f740-f78b-5aba-ade0-b99c0e93e47b&pf_rd_s=merchandised-search-11&pf_rd_t=BROWSE&pf_rd_i=16480631031",
                        "https://www.amazon.co.uk/PlayStation-9395003-5-Console/dp/B08H95Y452/ref=sr_1_2?crid=1OFHCTJS4LU9O&dchild=1&keywords=playstation+5&qid=1605678621&sprefix=Playstat%2Caps%2C178&sr=8-2",
                        "Add to Basket",
                        "0"
                    });
            #endregion
            
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
                try
                {
                    do
                    {
                        //Randomize retry timer to appear "Human". Honestly no human would refresh for more than 10hours.
                        int retryTimer = utility.GetRetryTimer();
                    
                        string htmlData = utility.HtmlContentAsStream(website.Value[0], website.Key);

                        utility.HtmlToFile(htmlData, dir.FullName + "\\data\\html" + $"\\{website.Key}.html");

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
                            String hourMinuteSeconds = DateTime.Now.ToString("HH:mm:ss");
                            Console.WriteLine($"[{website.Key}\t{hourMinuteSeconds}]\tProduct is not available, retrying in {retryTimer} seconds.");
                            //Thread sleep is in milliseconds, I don't want to type milliseconds times...
                            Thread.Sleep(retryTimer * 1000);
                        }
                    } while (productAvailable == false);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
               
                
                utility.OpenUrl(website.Value[0]);
                
                utility.SendMail(configuration, website);

                var messageInfos = utility.SendSMS(configuration, website);
                Console.Write(" " + messageInfos);

                Console.WriteLine($"[{website.Key}]" +
                                  $"\tProduct was available at {DateTime.Now}. Alert has been sent to user");
            });
        }
    }
}