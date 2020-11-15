## PS5 ECOMMERCE SITES WATCHER

This project will maybe allow me to grab a PS5 🎮 at launch in Europe (19/11/20).

It could be updated to target any kind of ecommerce website and any kind of item. Note: The user still need to manually add to cart and order for the item, I'm not trying to create a bot but rather to help people (especially myself) to get the item they want without having to constantly refresh a webpage.

It act like a web crawler by watching an url at a random interval and assessing if it's possible to add the item to cart. If it is possible it open the web browser on the URL, send a mail and a SMS to a designated recipient.

Project as taken a bit of a bigger development path

#### How to use

You will need to grab a free [Twilio account](https://www.twilio.com/try-twilio) which come with 15.50$. The steps to create a phone number and use it are described [there](https://www.twilio.com/docs/sms/quickstart/csharp-dotnet-core). I'm not responsible about your invoices 💲💲💲

You also need a SMTP capable mail provider, I use Google SMTP with MailKit, less than 10 lines code to send a mail.

Finally, you need to set the correct information into an `appSecrets.json` file, just rename and update the `appSecrets.example` file.

No help compiling and running this code will be given, you're on your own.

Target framework is [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) since I use the new `is not or` operators.

#### Skills learned from project 💻

- Hiding sensible user data in JSON files and accessing those fields using `Microsoft.Extension.Configuration`
- Web scraping using Selenium as a headless browser
  - Pick from a random list of proxies for each request
  - Modify the `User-Agent` header between each request to make it seems like it's different browser requesting the page
  - In the hope to bypass browser flagging and having to resolve a captcha
- Using and configuring Twilio CLI to acquire a phone number
- Using Twilio from code to send SMS
- Sending an Email from code using MailKit & Google SMTP server
- Hashing the content of a file to compare it to another. Did not work since some tracking elements change between each HTTP request ❌. Had to resort to search directly in the HTML stream after each scraping

