using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookStore.Api.Dtos;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using BookStore.Mvc.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Mvc.Controllers
{
    [Route("Books")]
    public class BooksController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BooksController> _logger;

        public BooksController(HttpClient httpClient, ILogger<BooksController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // GET: Books
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<BookDTO> books = new();
            List<CategoryDTO> categories = new();

            try
            {
                var bookResponse = await _httpClient.GetAsync("http://localhost:5265/odata/Books");
                bookResponse.EnsureSuccessStatusCode();

                var bookOdataResponse = await bookResponse.Content.ReadFromJsonAsync<ODataResponse<BookDTO>>();
                if (bookOdataResponse != null)
                {
                    books = bookOdataResponse.Value;
                }

                var categoryResponse = await _httpClient.GetAsync("http://localhost:5265/odata/Categories");
                categoryResponse.EnsureSuccessStatusCode();

                var categoryOdataResponse = await categoryResponse.Content.ReadFromJsonAsync<ODataResponse<CategoryDTO>>();
                if (categoryOdataResponse != null)
                {
                    categories = categoryOdataResponse.Value;
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Request error: {e.Message}");
            }

            // Pass categories to the view using ViewBag
            ViewBag.Categories = categories;

            return View(books);
        }


        // GET: Books/Details/5
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var book = await GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        [HttpGet("create")]
        public async Task<IActionResult> Create()
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

            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
            return View();
        }


        // POST: Books/Create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Author,Price,ImageUrl,CategoryId")] BookDTO bookDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("http://localhost:5265/odata/Books", bookDto);
                    response.EnsureSuccessStatusCode();

                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError($"Request error: {e.Message}");
                }
            }
            return View(bookDto);
        }

        // GET: Books/Edit/5
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }

            // Fetch categories for dropdown
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

            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", book.CategoryId);
            return View(book);
        }


        // POST: Books/Edit/5
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Author,Price,ImageUrl,CategoryId")] BookDTO bookDto)
        {
            if (id != bookDto.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PutAsJsonAsync($"http://localhost:5265/odata/Books({id})", bookDto);
                    response.EnsureSuccessStatusCode();

                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError($"Request error: {e.Message}");
                }
            }
            return View(bookDto);
        }

        // GET: Books/Delete/5
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            var response = await _httpClient.DeleteAsync($"http://localhost:5265/odata/Books({id})");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"http://localhost:5265/odata/Books({id})");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Request error: {e.Message}");
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<BookDTO> GetBookById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:5265/odata/Books?$filter=Id eq {id}");
                response.EnsureSuccessStatusCode();

                var odataResponse = await response.Content.ReadFromJsonAsync<ODataResponse<BookDTO>>();
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
