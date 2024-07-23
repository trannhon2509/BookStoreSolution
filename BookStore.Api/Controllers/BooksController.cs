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
    
    public class BooksController : ODataController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BooksController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Books);
        }

        [EnableQuery]
        public IActionResult GetCount()
        {
            var count = _context.Books.Count();
            return Ok(count);
        }

        [EnableQuery]
        public IActionResult GetProductDtos()
        {
            var products = _context.Books;
            var productDtos = _mapper.ProjectTo<BookDTO>(products);
            return Ok(productDtos);
        }

        [EnableQuery]
        public IActionResult GetProductDtos(int key)
        {
            var product = _context.Books.Find(key);
            if (product == null)
            {
                return NotFound();
            }
            var productDto = _mapper.Map<BookDTO>(product);
            return Ok(productDto);
        }

        public IActionResult Post([FromBody] BookDTO productDto)
        {
            var category = _context.Categories.Find(productDto.CategoryId);
            if (category == null)
            {
                return BadRequest("Category does not exist.");
            }

            var product = _mapper.Map<Book>(productDto);
            product.Category = category; 

            
            _context.Books.Add(product);
            _context.SaveChanges();
            return Created(product);
        }

        public IActionResult Put(int key, [FromBody] BookDTO productDto)
        {
            if (key != productDto.Id)
            {
                return BadRequest();
            }
            var product = _mapper.Map<Book>(productDto);
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(product);
        }

        public IActionResult Delete(int key)
        {
            var product = _context.Books.Find(key);
            if (product == null)
            {
                return NotFound();
            }
            _context.Books.Remove(product);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
