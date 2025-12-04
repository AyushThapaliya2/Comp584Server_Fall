namespace Comp_584_Server.DTOs
{
    public class PopulationDTO
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public decimal Population { get; set; }
        public required string Iso2 { get; set; }
        public required string Iso3 { get; set; }
    }
}
