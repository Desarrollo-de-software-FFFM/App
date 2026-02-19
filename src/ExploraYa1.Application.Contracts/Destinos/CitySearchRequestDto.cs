namespace ExploraYa1.Destinos
{
    public class CitySearchRequestDto
    {
        public string PartialName { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }
        public int? MinimumPopulation { get; set; }
    }
}
