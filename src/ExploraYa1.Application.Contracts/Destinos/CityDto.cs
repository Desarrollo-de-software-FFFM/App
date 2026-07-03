namespace ExploraYa1.Destinos
{
    public class CityDto
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public string? Region { get; set; }          // NUEVO
        public int? Population { get; set; }         // NUEVO

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

}