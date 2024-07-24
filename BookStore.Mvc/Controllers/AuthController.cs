using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BookStore.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.Plugins;

namespace BookStore.Mvc.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthController> _logger;

        public AuthController(HttpClient httpClient, ILogger<AuthController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("http://localhost:5265/Login", model);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<LoginResponseModel>();
                        var token = result?.Token;
                        var role = result?.Role;

                        if (token != null)
                        {
                            HttpContext.Session.SetString("AuthToken", token);

                            if (role == "ADMIN")
                            {
                                return RedirectToAction("Index", "Admin");
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Login failed. No token received.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Login failed. Please check your credentials.");
                    }
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError($"Request error: {e.Message}");
                    ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                }
            }

            return View(model);
        }




        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("http://localhost:5265/Register", model);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
                    }
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError($"Request error: {e.Message}");
                    ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                }
            }

            return View(model);
        }
    }
}
