namespace WeatherWise_front.Models
{
    public class Country
    {
        public string Name { get; set; }
        public string Alpha2Code { get; set; }
        public string Alpha3Code { get; set; }
        public string Capital { get; set; }
        public long Population { get; set; }
        public string[] Languages { get; set; }
        public string[] Currencies { get; set; }
    }
}