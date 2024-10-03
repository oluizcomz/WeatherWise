using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WeatherWise_front.Models;
using WeatherWise_front.Services;

namespace WeatherWise_front.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly RestCountriesService _restCountriesService;

        public HomeController(ILogger<HomeController> logger, RestCountriesService restCountriesService)
        {
            _logger = logger;
            _restCountriesService = restCountriesService;
        }

        public async Task<IActionResult> Index()
        {
            List<Country> countries = await _restCountriesService.GetCountries(); 
            return View(countries);
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
