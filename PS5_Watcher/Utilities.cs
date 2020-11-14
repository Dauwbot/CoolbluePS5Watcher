using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PS5_Watcher
{
    public class Utilities
    {
        public string HtmlContentAsStream(string urlAddress)
        {
            string data = null;
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(urlAddress);
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.193 Safari/537.36";
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader reader = null;

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
                data = "There was a problem downloading HTML data, check if it is still working";
            }

            return data;
        }

        internal int CheckStringForSubstring(string completeString, string substringToFind)
        {
            return completeString.IndexOf(substringToFind);
        }

        internal void OpenUrl(string urlAddress)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {urlAddress}") {CreateNoWindow = true});
        }

        internal string SendSMS(IConfigurationRoot configuration)
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
                    body: "PS5 disponible sur Coolblue",
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

            return $"Message should have been sent at {messageDateCreated}";
        }

        internal void SendMail(IConfigurationRoot configuration)
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
            mailMessage.Subject = "PS5 Disponible";
            mailMessage.Body = new TextPart("plain")
            {
                Text = $"PS5 disponible sur le site {mailSenderName}"
            };

            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.gmail.com", 465, true);
                smtpClient.Authenticate(receiverAddress, mailPass);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }
        }

        internal int GenerateRandomNumberInRange(int min, int max)
        {
            Random random = new Random(); 
            return random.Next(min, max);
        }

        internal DateTime GetCurrentDate()
        {
            DateTime date = DateTime.Now;
            return date;
        }
    }
}