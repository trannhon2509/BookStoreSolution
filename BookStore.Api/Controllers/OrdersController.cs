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
    public class OrdersController : ODataController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrdersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Orders);
        }

        [EnableQuery]
        public IActionResult GetCount()
        {
            var count = _context.Orders.Count();
            return Ok(count);
        }

        [EnableQuery]
        public IActionResult GetOrderDtos()
        {
            var orders = _context.Orders;
            var orderDtos = _mapper.ProjectTo<OrderDTO>(orders);
            return Ok(orderDtos);
        }

        [EnableQuery]
        public IActionResult GetOrderDtos(int key)
        {
            var order = _context.Orders.Find(key);
            if (order == null)
            {
                return NotFound();
            }
            var orderDto = _mapper.Map<OrderDTO>(order);
            return Ok(orderDto);
        }

        public IActionResult Post([FromBody] OrderDTO orderDto)
        {
            var user = _context.Users.Find(orderDto.UserId);
            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            var coupon = _context.Coupons.Find(orderDto.CouponId);

            var order = _mapper.Map<Order>(orderDto);
            order.User = user;
            order.Coupon = coupon;

            _context.Orders.Add(order);
            _context.SaveChanges();
            return Created(order);
        }

        public IActionResult Put(int key, [FromBody] OrderDTO orderDto)
        {
            if (key != orderDto.Id)
            {
                return BadRequest();
            }

            var order = _mapper.Map<Order>(orderDto);
            _context.Entry(order).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(order);
        }

        public IActionResult Delete(int key)
        {
            var order = _context.Orders.Find(key);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
