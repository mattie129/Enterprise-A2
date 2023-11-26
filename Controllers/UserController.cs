using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models.Entities;
using API.Models.Helpers;
using API.Services;
using API.Responses;
using PeopleAPI.Services;



namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private ImageDatabase _db = new ImageDatabase();
        
        
        [HttpPost]
        public async Task<IActionResult> AddUser(User user){
            var emailInDb = await _db.Users.SingleOrDefaultAsync(x => user.Email == x.Email);

            //if (){}
            if (user.Email == null){
                return BadRequest("Invalid email");
            } else if (user.Name == null || user.Name == ""){
                return BadRequest("Invalid name");
            } else if (emailInDb == null){
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();
                return Created("", "User Added");
            } else{
                return BadRequest("Email already in use");
            }            
        }

    }
}