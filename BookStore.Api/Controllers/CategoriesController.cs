using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using BookStore.BOL.Entities;
using BookStore.DAL.Data;
using BookStore.Api.Dtos;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNet.OData.Routing;

namespace BookStore.Api.Controllers
{
    public class CategoriesController : ODataController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Categories);
        }

        [EnableQuery]
        public IActionResult GetCount()
        {
            var count = _context.Categories.Count();
            return Ok(count);
        }

        [EnableQuery]
        public IActionResult GetCategoryDtos()
        {
            var categories = _context.Categories;
            var categoryDtos = _mapper.ProjectTo<CategoryDTO>(categories);
            return Ok(categoryDtos);
        }

        [EnableQuery]
        [ODataRoute("Categories({key})")]
        public IActionResult GetById([FromODataUri] int key)
        {
            var category = _context.Categories.Find(key);
            if (category == null)
            {
                return NotFound();
            }
            var categoryDto = _mapper.Map<CategoryDTO>(category);
            return Ok(categoryDto);
        }

        public IActionResult Post([FromBody] CategoryDTO categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            _context.Categories.Add(category);
            _context.SaveChanges();
            return Created(category);
        }

        public IActionResult Put(int key, [FromBody] CategoryDTO categoryDto)
        {
            if (key != categoryDto.Id)
            {
                return BadRequest();
            }
            var category = _mapper.Map<Category>(categoryDto);
            _context.Entry(category).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(category);
        }

        public IActionResult Delete(int key)
        {
            var category = _context.Categories.Find(key);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
