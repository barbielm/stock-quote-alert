using System.Globalization;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;
using stock_alert_quote.DeserializeClasses;

DotNetEnv.Env.Load();
string? USER_EMAIL = Environment.GetEnvironmentVariable("USER");
string? USER_PASSWORD = Environment.GetEnvironmentVariable("PASSWORD");
string? API_KEY = Environment.GetEnvironmentVariable("API_KEY");

if (API_KEY is null || USER_EMAIL is null || USER_PASSWORD is null)
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
string data = await Functions.GetStockData(API_KEY, ASSET);
var obj = JsonConvert.DeserializeObject<Data>(data);

Console.WriteLine(obj.results[ASSET].price);


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
    public static string CheckBoundaries(int buyingPrice, int sellingPrice, int currentPrice)
    {
        string message = "";
        if (currentPrice > sellingPrice)
        {
            message = "The current price of the asset is above the selling price. It is recommendable selling it.";
        }
        else if (currentPrice < buyingPrice)
        {
            message = "The current price of the asset is below the buying price. It is recommendable buying it.";
        }

        return message;
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