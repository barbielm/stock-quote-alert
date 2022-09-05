namespace stock_alert_quote.DeserializeClasses
{
    public class Results : Dictionary<string, AssetInformations> { }

    public class AssetInformations
    {
        public string? symbol { get; set; }
        public string? name { get; set; }
        public string? company_name { get; set; }
        public string? document { get; set; }
        public string? description { get; set; }
        public string? website { get; set; }
        public string? region { get; set; }
        public string? currency { get; set; }
        public MarketTime? market_time { get; set; }
        public string? market_cap { get; set; }
        public float? price { get; set; }
        public float? change_percent { get; set; }
        public string? updated_at { get; set; }

    }

}
