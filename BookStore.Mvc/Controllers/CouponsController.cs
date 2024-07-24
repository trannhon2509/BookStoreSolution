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
    [Route("Coupons")]
    public class CouponsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CouponsController> _logger;

        public CouponsController(HttpClient httpClient, ILogger<CouponsController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // GET: Coupons
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<CouponDTO> coupons = new();
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5265/odata/Coupons");
                response.EnsureSuccessStatusCode();

                var odataResponse = await response.Content.ReadFromJsonAsync<ODataResponse<CouponDTO>>();
                if (odataResponse != null)
                {
                    coupons = odataResponse.Value;
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Request error: {e.Message}");
            }

            return View(coupons);
        }

        // GET: Coupons/Details/5
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var coupon = await GetCouponById(id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // GET: Coupons/Create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coupons/Create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Discount,ExpirationDate")] CouponDTO couponDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("http://localhost:5265/odata/Coupons", couponDto);
                    response.EnsureSuccessStatusCode();

                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError($"Request error: {e.Message}");
                }
            }
            return View(couponDto);
        }

        // GET: Coupons/Edit/5
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var coupon = await GetCouponById(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        // POST: Coupons/Edit/5
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Discount,ExpirationDate")] CouponDTO couponDto)
        {
            if (id != couponDto.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PutAsJsonAsync($"http://localhost:5265/odata/Coupons({id})", couponDto);
                    response.EnsureSuccessStatusCode();

                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError($"Request error: {e.Message}");
                }
            }
            return View(couponDto);
        }

        // GET: Coupons/Delete/5
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await GetCouponById(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        // POST: Coupons/Delete/5
        [HttpPost("delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"http://localhost:5265/odata/Coupons({id})");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Request error: {e.Message}");
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<CouponDTO> GetCouponById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:5265/odata/Coupons?$filter=Id eq {id}");
                response.EnsureSuccessStatusCode();

                var odataResponse = await response.Content.ReadFromJsonAsync<ODataResponse<CouponDTO>>();
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
