namespace Comp_584_Server.Data
{
    public class WorldData
    {
        public long id { get; set; }
        public required string city {  get; set; }
        public required string city_ascii { get; set; }
        public required string country { get; set; }
        public required string iso2 { get; set; }
        public required string iso3 { get; set; }
        public required string admin_name { get; set; }
        public required string capital { get; set; }
        public decimal lat { get; set; }
        public decimal lng { get; set; }
        public decimal? population { get; set; }
    }
}
