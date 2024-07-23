using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.BOL.Entities;
using BookStore.DAL.Data;
using AutoMapper;
using Microsoft.AspNetCore.OData.Query;
using BookStore.Api.Dtos;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BookStore.Api.Controllers
{
    public class CouponsController : ODataController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CouponsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Coupons);
        }

        [EnableQuery]
        public IActionResult GetCount()
        {
            var count = _context.Coupons.Count();
            return Ok(count);
        }

        [EnableQuery]
        public IActionResult GetCouponDtos()
        {
            var coupons = _context.Coupons;
            var couponDtos = _mapper.ProjectTo<CouponDTO>(coupons);
            return Ok(couponDtos);
        }

        [EnableQuery]
        public IActionResult GetCouponDtos(int key)
        {
            var coupon = _context.Coupons.Find(key);
            if (coupon == null)
            {
                return NotFound();
            }
            var couponDto = _mapper.Map<CouponDTO>(coupon);
            return Ok(couponDto);
        }

        public IActionResult Post([FromBody] CouponDTO couponDto)
        {
            var coupon = _mapper.Map<Coupon>(couponDto);
            _context.Coupons.Add(coupon);
            _context.SaveChanges();
            return Created(coupon);
        }

        public IActionResult Put(int key, [FromBody] CouponDTO couponDto)
        {
            if (key != couponDto.Id)
            {
                return BadRequest();
            }
            var coupon = _mapper.Map<Coupon>(couponDto);
            _context.Entry(coupon).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(coupon);
        }

        public IActionResult Delete(int key)
        {
            var coupon = _context.Coupons.Find(key);
            if (coupon == null)
            {
                return NotFound();
            }
            _context.Coupons.Remove(coupon);
            _context.SaveChanges();
            return NoContent();
        }
    }

}
