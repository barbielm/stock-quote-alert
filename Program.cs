using System.Globalization;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

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

Functions.SendEmail(USER_EMAIL, USER_PASSWORD, "gabriel.meireles@ime.eb.br", "Teste", ASSET);

string informations = await Functions.GetStockPrice(API_KEY);
Console.WriteLine(informations);
Console.ReadLine();

public class Functions
{
    public static async Task<string> GetStockPrice(string apiKey)
    {
        string uri = $"https://api.hgbrasil.com/finance/stock_price?key={apiKey}&symbol=PETR4";
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage();
        var response = await httpClient.GetAsync(uri);
        var data = await response.Content.ReadAsStringAsync();
        return data;
    }
    public static void CheckBoundaries(int buyingPrice, int sellingPrice, int currentPrice)
    {
        if (currentPrice > sellingPrice)
        {

            Console.WriteLine("a ação deve ser vendida");
        }
        if (currentPrice < buyingPrice)
        {
            Console.WriteLine("a ação deve ser comprada");
        }
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