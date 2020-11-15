#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PS5_Watcher
{
    public class Utilities
    {
        //Grab the content of a webpage in a stream and place it in a string for easier analysis
        public string HtmlContentAsStream(string urlAddress)
        {
            string? data;
            try
            {
                string userAgent = SelectRandomUA();
                Console.WriteLine(userAgent);
                
                if (CheckStringForSubstring(urlAddress, "cdiscount") >= 0)
                {
                    ChromeOptions options = new ChromeOptions();
                    options.AddArgument("headless");
                    options.AddArgument(userAgent);
                    
                    ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                    service.HideCommandPromptWindow = true;
                    
                    IWebDriver driver = new ChromeDriver(service, options);
                    driver.Navigate().GoToUrl(urlAddress);
                    data = driver.PageSource;
                    driver.Close();

                    return data;
                }
                
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(urlAddress);
                //Make us appear as a normal web browser rather than just a bot
                request.UserAgent = userAgent;
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader reader;

                    if (string.IsNullOrWhiteSpace(response.CharacterSet))
                        reader = new StreamReader(receiveStream);
                    else
                        reader = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                    data = reader.ReadToEnd();
                    response.Close();
                    reader.Close();
                }
                else
                {
                    //Well if we can't download the HTML we've surely been blacklisted as a bot...
                    //If you've a dynamic IP you should restart your router and hope it works again
                    data = "There was a problem downloading HTML data, check if it is still working";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return data;
        }

        private string SelectRandomUA()
        {
            var userAgentsListFile = File.ReadAllLines("userAgentsList.txt");
            var userAgentsList = new List<string>(userAgentsListFile);
            Random randNum = new Random();
            int randomUA = randNum.Next(userAgentsList.Count);
            return "--user-agent=" + userAgentsList[randomUA];
        }

        //Wrapper for IndexOf
        internal int CheckStringForSubstring(string completeString, string substringToFind)
        {
            return completeString.IndexOf(substringToFind.ToLowerInvariant());
        }

        //Process.Start does not work correctly on .NET CORE. Workaround
        internal void OpenUrl(string urlAddress)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {urlAddress}") {CreateNoWindow = true});
        }

        internal string SendSMS(IConfigurationRoot configuration, KeyValuePair<string, string[]> keyValuePair)
        {
            DateTime? messageDateCreated;
            try
            {
                IConfigurationSection twilioSettings = configuration.GetSection("TwilioSettings");
                var accountSid = twilioSettings["AccountSid"];
                var accountToken = twilioSettings["AuthToken"];
                var senderNumber = twilioSettings["SenderNumber"];
                var receiverNumber = twilioSettings["ReceiverNumber"];

                TwilioClient.Init(accountSid, accountToken);

                MessageResource message = MessageResource.Create(
                    body: $"PS5 available on {keyValuePair.Key} {keyValuePair.Value[0]}",
                    from: new PhoneNumber(senderNumber),
                    to: new PhoneNumber(receiverNumber)
                );

                messageDateCreated = message.DateCreated;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return $"Sent at {messageDateCreated}";
        }

        internal void SendMail(IConfigurationRoot configuration, KeyValuePair<string, string[]> keyValuePair)
        {
            IConfigurationSection mailSettings = configuration.GetSection("MailSettings");
            var mailSenderName = mailSettings["SenderName"];
            var mailSenderAddress = mailSettings["SenderAddress"];
            var receiverName = mailSettings["ReceiverName"];
            var receiverAddress = mailSettings["ReceiverAddress"];
            var mailPass = mailSettings["MailPass"];


            MimeMessage mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(mailSenderName, mailSenderAddress));
            mailMessage.To.Add(new MailboxAddress(receiverName, receiverAddress));
            mailMessage.Subject = $"PS5 available at {keyValuePair.Key}";
            mailMessage.Body = new TextPart("plain")
            {
                Text = $"PS5 available at {keyValuePair.Key} @ {keyValuePair.Value[0]}"
            };

            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.gmail.com", 465, true);
                smtpClient.Authenticate(receiverAddress, mailPass);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }
        }
        
        public void HtmlToFile(string htmlData, string filePath)
        {
            StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8);
            writer.Write(htmlData);
            writer.Close();
        }

        internal int GenerateRandomNumberInRange(int min, int max)
        {
            Random random = new Random(); 
            return random.Next(min, max);
        }

        //Set the retry timer regarding the current time, we will let it run quite a while so...
        public int GetRetryTimer()
        {
            DateTime date = DateTime.Now;

            //If we're another date than the 18th or 19th
            if (date.Day is not (18 or 19))
            {
                return GenerateRandomNumberInRange(300, 900); 
            }
            
            if (date.Hour is (<= 6 or >= 20))
            {
                return GenerateRandomNumberInRange(150, 300);
            }
            
            if (date.Hour is (<= 8 or >= 16))
            {
                return GenerateRandomNumberInRange(30, 150);
            }
            
            return GenerateRandomNumberInRange(10, 30);
        }
    }
}