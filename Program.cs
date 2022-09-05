using System.Globalization;
using System.Text.RegularExpressions;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;
using stock_alert_quote.DeserializeClasses;


DotNetEnv.Env.Load();
string? ADMIN_EMAIL = Environment.GetEnvironmentVariable("ADMIN");
string? ADMIN_PASSWORD = Environment.GetEnvironmentVariable("PASSWORD");
string? API_KEY = Environment.GetEnvironmentVariable("API_KEY");

if (API_KEY is null || ADMIN_EMAIL is null || ADMIN_PASSWORD is null)
{
    throw new ArgumentException("fill in your .env file accordingly");
}

string ASSET = args[0];
float SELLING_PRICE = float.Parse(args[1], CultureInfo.InvariantCulture.NumberFormat);
float BUYING_PRICE = float.Parse(args[2], CultureInfo.InvariantCulture.NumberFormat);
if (SELLING_PRICE <= BUYING_PRICE)
{
    throw new ArgumentException("selling price must always be higher than the buying price", nameof(SELLING_PRICE));
}

string RECIPIENT_EMAIL = Functions.GetClientEmail();
while (String.IsNullOrEmpty(RECIPIENT_EMAIL))
{
    Console.WriteLine("Type a valid email address");
    RECIPIENT_EMAIL = Functions.GetClientEmail();
}

while (true)
{
    string serializedData = await Functions.GetStockData(API_KEY, ASSET);
    Data? deserializedData = JsonConvert.DeserializeObject<Data>(serializedData);
    float? currentPrice = deserializedData?.results?[ASSET].price;
    if (currentPrice.HasValue)
    {
        string emailMessage = Functions.CheckBoundaries(BUYING_PRICE, SELLING_PRICE, currentPrice);
        if (!String.IsNullOrEmpty(emailMessage))
        {
            Functions.SendEmail(ADMIN_EMAIL, ADMIN_PASSWORD, RECIPIENT_EMAIL, emailMessage, $"{ASSET} price");
            
            Console.WriteLine($"An email was sent to {RECIPIENT_EMAIL}");
        }
    }
    else
    {
        throw new ArgumentException("check the name of your asset", nameof(ASSET));
    }
    await Task.Delay(5000);
}



public class Functions
{
    public static async Task<string> GetStockData(string apiKey, string asset)
    {
        string uri = $"https://api.hgbrasil.com/finance/stock_price?key={apiKey}&symbol={asset}";
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage();
        var response = await httpClient.GetAsync(uri);
        string data = await response.Content.ReadAsStringAsync();
        return data;
    }
    public static string CheckBoundaries(float buyingPrice, float sellingPrice, float? currentPrice)
    {
        string emailMessage = "";
        if (currentPrice > sellingPrice)
        {
            emailMessage = "The current price of the asset is above the selling price. It is recommendable selling it.";
            Console.WriteLine("The current price of the asset is above the selling price");
        }
        else if (currentPrice < buyingPrice)
        {
            emailMessage = "The current price of the asset is below the buying price. It is recommendable buying it.";
            Console.WriteLine("The current price of the asset is below the buying price");
        }

        else 
        {
            Console.WriteLine("The current price of the asset is within boundaries");
        }
        
        return emailMessage;
    }

    public static string GetClientEmail()
    {
        string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";

        Console.WriteLine("Type the email of the asset owner");
        string? email = Console.ReadLine();
        if ( !String.IsNullOrEmpty(email) && Regex.IsMatch(email, regex, RegexOptions.IgnoreCase))
        {
            return email;
            
        }

        return "";   
    }
    
    

    public static void SendEmail(string userEmail, string password, string recipientEmail, string message, string subject)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(userEmail));
        email.To.Add(MailboxAddress.Parse(recipientEmail));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Plain) { Text = message };

        using var smtp = new SmtpClient();
        smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        smtp.Authenticate(userEmail, password);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}