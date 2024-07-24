using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookStore.Api.Dtos;
using BookStore.Mvc.Models;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace BookStore.Mvc.Controllers
{
    [Route("Categories")]
    public class CategoriesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(HttpClient httpClient, ILogger<CategoriesController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // GET: Categories
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<CategoryDTO> categories = new();
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5265/odata/Categories");
                response.EnsureSuccessStatusCode();

                var odataResponse = await response.Content.ReadFromJsonAsync<ODataResponse<CategoryDTO>>();
                if (odataResponse != null)
                {
                    categories = odataResponse.Value;
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Request error: {e.Message}");
            }

            return View(categories);
        }

        // GET: Categories/Details/5
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var category = await GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] CategoryDTO categoryDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("http://localhost:5265/odata/Categories", categoryDto);
                    response.EnsureSuccessStatusCode();

                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError($"Request error: {e.Message}");
                }
            }
            return View(categoryDto);
        }

        // GET: Categories/Edit/5
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] CategoryDTO categoryDto)
        {
            if (id != categoryDto.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PutAsJsonAsync($"http://localhost:5265/odata/Categories({id})", categoryDto);
                    response.EnsureSuccessStatusCode();

                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError($"Request error: {e.Message}");
                }
            }
            return View(categoryDto);
        }

        // GET: Categories/Delete/5
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            var response = await _httpClient.DeleteAsync($"http://localhost:5265/odata/Categories({id})");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"http://localhost:5265/odata/Categories({id})");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Request error: {e.Message}");
            }

            return RedirectToAction(nameof(Index));
        }


        private async Task<CategoryDTO> GetCategoryById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:5265/odata/Categories?$filter=Id eq {id}");
                response.EnsureSuccessStatusCode();

                var odataResponse = await response.Content.ReadFromJsonAsync<ODataResponse<CategoryDTO>>();
                return odataResponse?.Value.FirstOrDefault();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Request error: {e.Message}");
                return null;
            }
        }
    }
}
