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

        [HttpGet("{userId}/images")]
        public async Task<IActionResult> GetUserImages(Guid userId, int page = 1, int pageSize = 10) //default page and pageSize values if unspecified
        {

            
            var user = _db.Users
                .Include(x => x.Images) 
                .FirstOrDefault(x => x.Id == userId);

            if (user != null)
            {
                var images = user.Images.OrderByDescending(i => i.PostingDate).ToList();

                images = images
                    .Skip((page - 1) * pageSize) 
                    .Take(pageSize)
                    .ToList();

                var response = new PagedResponse<Image>(images);

                var total = await _db.Images.CountAsync();

                
                response.Links = LinksGenerator.GenerateLinks("/api/User", page, total, pageSize);

                return Ok(response);


            }

            return NotFound("User not found");
        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> Get10UserImages(Guid userId)
        {

            
            var user = _db.Users
                .Include(x => x.Images) 
                .FirstOrDefault(x => x.Id == userId);

            if (user != null)
            {

                //return first 10
                if (user.Images.Count < 10)
                {
                    var images = user.Images.OrderByDescending(i => i.PostingDate).ToList();
                    return Ok(images);
                }else {
                    var images = user.Images.OrderByDescending(i => i.PostingDate).Take(10);
                    return Ok(images);
                }
                
            }
            



            return NotFound("User not found");
        }


           
        [HttpPost]
        public async Task<IActionResult> AddUser(User user){
            var emailInDb = await _db.Users.SingleOrDefaultAsync(x => user.Email == x.Email);

            //if (){}
            if (user.Email == null){
                return BadRequest("Invalid email"); //----> email format is verified in updated User.cs <----
            } else if (user.Name == null || user.Name == ""){
                return BadRequest("Invalid name");
            } else if (emailInDb == null){
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();
                return Created("", "User Added");
            } else{
                //if user.Email != null:  email is already in database 
                return BadRequest("Email already in use");
            }            
        }

    }
}