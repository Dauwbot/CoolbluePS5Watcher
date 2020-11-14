## PS5 ECOMMERCE SITES WATCHER

This project will maybe allow me to grab a PS5 🎮 at launch in Europe (19/11/20). 

It act like an automated refresher by watching an url at a random interval and assessing if it's possible to add the item to cart. If it is possible it open the web browser on the URL, send a mail and a SMS to a designated recipient.

#### How to use

You will need to grab a free [Twilio account](https://www.twilio.com/try-twilio) which come with 15.50$. The steps to create a phone number and use it are described [there](https://www.twilio.com/docs/sms/quickstart/csharp-dotnet-core). I'm not responsible about your invoices 💲💲💲

You also need a SMTP capable mail provider, I use Google SMTP with MailKit, less than 10 lines code to send a mail.

Finally, you need to set the correct information into an `appSecrets.json` file, just rename the `example_` file.

Finally no help compiling and running this code will be given, you're on your own. Hint: You could just launch it in your IDE and it will work. Target framework in .NET5 but I don't use any C#9 functions so you could port it back…

#### Skills learned from project 💻

- Hiding sensible user data in JSON files and accessing those fields using `Microsoft.Extension.Configuration`
- Using and configuring Twilio CLI to acquire a phone number
- Using Twilio from code to send SMS
- Sending an Email from code using MailKit
- Hashing the content of a file to compare it to another. Did not work since some tracking elements change between each HTTP request ❌

