using System.Globalization;
using Newtonsoft.Json;
using stock_alert_quote.DeserializeClasses;
using stock_alert_quote.Services;

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

string RECIPIENT_EMAIL = ClientService.GetClientEmail();
while (String.IsNullOrEmpty(RECIPIENT_EMAIL))
{
    Console.WriteLine("Type a valid email address");
    RECIPIENT_EMAIL = ClientService.GetClientEmail();
}

while (true)
{
    string serializedData = await AssetService.GetAssetData(API_KEY, ASSET);
    Data? deserializedData = JsonConvert.DeserializeObject<Data>(serializedData);
    float? currentPrice = deserializedData?.results?[ASSET].price;
    if (currentPrice.HasValue)
    {
        string emailMessage = AssetService.CheckPriceBoundaries(BUYING_PRICE, SELLING_PRICE, currentPrice);
        if (!String.IsNullOrEmpty(emailMessage))
        {
            EmailService.SendEmail(ADMIN_EMAIL, ADMIN_PASSWORD, RECIPIENT_EMAIL, emailMessage, $"{ASSET} price");
            Console.WriteLine($"Current price: R$ {currentPrice}, your selected interval was: R$ {SELLING_PRICE} - R$ {BUYING_PRICE}.");
            Console.WriteLine($"An email was sent to {RECIPIENT_EMAIL}");
        }
    }
    else
    {
        throw new ArgumentException("check the name of your asset", nameof(ASSET));
    }
    await Task.Delay(5000);
}



