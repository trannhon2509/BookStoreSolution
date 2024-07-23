using BookStore.Api.Dtos;
using BookStore.BOL.Entities;
using BookStore.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookStore.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            List<BookDTO> books = new();
            try
            {
                // Replace with your OData API endpoint
                var response = await _httpClient.GetAsync("http://localhost:5265/odata/Books");
                response.EnsureSuccessStatusCode();

                var odataResponse = await response.Content.ReadFromJsonAsync<ODataResponse<BookDTO>>();
                if (odataResponse != null)
                {
                    books = odataResponse.Value;
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Request error: {e.Message}");
            }


            return View(books);
        }

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
