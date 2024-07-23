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
    public class OrderDetailsController : ODataController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderDetailsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.OrderDetails);
        }

        [EnableQuery]
        public IActionResult GetCount()
        {
            var count = _context.OrderDetails.Count();
            return Ok(count);
        }

        [EnableQuery]
        public IActionResult GetOrderDetailDtos()
        {
            var orderDetails = _context.OrderDetails;
            var orderDetailDtos = _mapper.ProjectTo<OrderDetailDTO>(orderDetails);
            return Ok(orderDetailDtos);
        }

        [EnableQuery]
        public IActionResult GetOrderDetailDtos(int key)
        {
            var orderDetail = _context.OrderDetails.Find(key);
            if (orderDetail == null)
            {
                return NotFound();
            }
            var orderDetailDto = _mapper.Map<OrderDetailDTO>(orderDetail);
            return Ok(orderDetailDto);
        }

        public IActionResult Post([FromBody] OrderDetailDTO orderDetailDto)
        {
            var order = _context.Orders.Find(orderDetailDto.OrderId);
            if (order == null)
            {
                return BadRequest("Order does not exist.");
            }

            var book = _context.Books.Find(orderDetailDto.BookId);
            if (book == null)
            {
                return BadRequest("Book does not exist.");
            }

            var orderDetail = _mapper.Map<OrderDetail>(orderDetailDto);
            orderDetail.Order = order;
            orderDetail.Book = book;

            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
            return Created(orderDetail);
        }

        public IActionResult Put(int key, [FromBody] OrderDetailDTO orderDetailDto)
        {
            if (key != orderDetailDto.Id)
            {
                return BadRequest();
            }

            var orderDetail = _mapper.Map<OrderDetail>(orderDetailDto);
            _context.Entry(orderDetail).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(orderDetail);
        }

        public IActionResult Delete(int key)
        {
            var orderDetail = _context.OrderDetails.Find(key);
            if (orderDetail == null)
            {
                return NotFound();
            }

            _context.OrderDetails.Remove(orderDetail);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
