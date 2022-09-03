// See https://aka.ms/new-console-template for more information
string API_KEY = "fe9b819b";
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
    public static void checkBoundaries()
    {

    }
}