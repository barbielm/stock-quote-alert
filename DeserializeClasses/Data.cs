namespace stock_alert_quote.DeserializeClasses
{
    public class Data
    {
        public string? by { get; set; }
        public bool valid_key { get; set; }
        public Results? results { get; set; }
        public float execution_time { get; set; }
        public bool from_cache { get; set; }

    }
}
