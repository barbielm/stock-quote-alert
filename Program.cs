using System.Globalization;

string ASSET = args[0];
float SELLING_PRICE = float.Parse(args[1], CultureInfo.InvariantCulture.NumberFormat);
float BUYING_PRICE = float.Parse(args[2], CultureInfo.InvariantCulture.NumberFormat);
string API_KEY = "fe9b819b";
Functions.checkIfIntervalIsValid(BUYING_PRICE, SELLING_PRICE);
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

    public static void checkIfIntervalIsValid(float buyingPrice, float sellingPrice)
    {
        if (sellingPrice <= buyingPrice)
        {
            throw new ArgumentException("selling price must always be higher than the buying price", nameof(sellingPrice));
        }
    }
    public static void checkBoundaries(int buyingPrice, int sellingPrice, int currentPrice)
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

    public static void sendEmail()
    {

    }
}