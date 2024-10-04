using System.Collections.Generic;

namespace WeatherWise_front.Models
{
    public class HomeModelView
    {
        public List<Country> Countries { get; set; } 
        public List<Country> Favorites { get; set; }
    }
}