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
        public string HtmlContentStream(string urlAddress)
        {
            string data = null;
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(urlAddress);
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.193 Safari/537.36";
            request.Headers["cookie"] =
                "Coolblue-Session=10316bdae7e1793985a7553e4b313437; Secure-Coolblue=0dbf3e47077716eae77af95ee19c4713; PHPSESSID=qh139u3ibs9ckr410uisuerjbn; wishListRequested=true; seenLanguageEmphasis=1; locale=fr_BE; assignedVariations=egDtDbt4N36YYsF9XkYW1qGeWFEe61EitS9UDsTm9F1MP3fTxY5VJ7ViUUy3UkLUY1WSn0UFzYivhdhgNDI21LVTWRtSxrooQWAEioaafRPa6e9gxlPJVQ6YTBC7nms97U17DnTXuS6i8aMpSLdBwV2mjQCEQnbmoquk3ayFsP6QykCmO7XURaHyet6MbyWZPtMPb2cAIGKeY1G3SHqRiq8fdDrYpTSFowWtqLFZtYx2HuRAvNxSpNXD19GZ8wVR0JOoJ4i5; _dd_s=rum=0&expire=1605202357056";
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
    }
}