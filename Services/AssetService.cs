
namespace stock_alert_quote.Services
{
    public class AssetService
    {
        public static async Task<string> GetAssetData(string apiKey, string asset)
        {
            string uri = $"https://api.hgbrasil.com/finance/stock_price?key={apiKey}&symbol={asset}";
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage();
            var response = await httpClient.GetAsync(uri);
            string data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public static string CheckPriceBoundaries(float buyingPrice, float sellingPrice, float? currentPrice)
        {
            string emailMessage = "";
            if (currentPrice > sellingPrice)
            {
                emailMessage = $"The current price of the asset is above the selling price. It is recommendable selling it. Current price: R$ {currentPrice}. Your selected interval was: R$ {sellingPrice} - R$ {buyingPrice}.";
                Console.WriteLine("The current price of the asset is above the selling price");
            }
            else if (currentPrice < buyingPrice)
            {
                emailMessage = $"The current price of the asset is below the buying price. It is recommendable buying it. Current price: R$ {currentPrice}. Your selected interval was: R$ {sellingPrice} - R$ {buyingPrice}.";
                Console.WriteLine("The current price of the asset is below the buying price");
            }

            else
            {
                Console.WriteLine("The current price of the asset is within boundaries");
            }

            return emailMessage;
        }
    }
}
