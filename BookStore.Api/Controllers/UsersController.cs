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

using Microsoft.AspNetCore.Identity;

namespace BookStore.Api.Controllers
{

    public class UsersController : ODataController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UsersController(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_userManager.Users);
        }

        [EnableQuery]
        public async Task<IActionResult> GetCount()
        {
            var count = await _userManager.Users.CountAsync();
            return Ok(count);
        }

        [EnableQuery]
        public IActionResult GetUserDtos()
        {
            var users = _userManager.Users;
            var userDtos = _mapper.ProjectTo<UserDTO>(users);
            return Ok(userDtos);
        }

        [EnableQuery]
        public async Task<IActionResult> GetUserDtos(string key)
        {
            var user = await _userManager.FindByIdAsync(key);
            if (user == null)
            {
                return NotFound();
            }
            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        }

        public async Task<IActionResult> Post([FromBody] UserDTO userDto)
        {
            var user = _mapper.Map<ApplicationUser>(userDto);
            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Created(user);
        }

        public async Task<IActionResult> Put(string key, [FromBody] UserDTO userDto)
        {
            var user = await _userManager.FindByIdAsync(key);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = userDto.UserName;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Updated(user);
        }

        public async Task<IActionResult> Delete(string key)
        {
            var user = await _userManager.FindByIdAsync(key);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return NoContent();
        }
    }
}
