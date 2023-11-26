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
        
        [HttpGet]
        public async Task<IActionResult> GetImages(int page = 1, int pageSize = 10)
        {

            var images = await _db.Images
                .OrderByDescending(i => i.PostingDate)
                .Skip((page - 1) * pageSize) 
                .Take(pageSize)
                .ToListAsync();

            var response = new PagedResponse<Image>(images);

            var total = await _db.Images.CountAsync();

            
            response.Links = LinksGenerator.GenerateLinks("/api/People", page, total, pageSize);

            return Ok(response);
            
        }

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

        [HttpPost("AddImage/{userId}")]
        public async Task<IActionResult> AddImageToUser(Guid userId, Image image)
        {
            if (image == null){
                return NotFound("Image not found");
            }

            User user = _db.Users.Include(u => u.Images).FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                
                image.User = user;
                image.UserID = user.Id;
                image.PostingDate = DateTime.UtcNow;

                user.Images.Add(image);

                IEnumerable<string> tags = ImageHelper.GetTags(image.Url);

                foreach (var tagText in tags)
                {
                    Tag tag = new Tag { Text = tagText };

                    tag.Images = new List<Image> { image };

                    await _db.Tags.AddAsync(tag);
                }
                await _db.Images.AddAsync(image);
                await _db.SaveChangesAsync();


                //TODO: Fix this 

                return Ok(user);


                if (user.Images.Count >= 10){
                    var lastTenImages = user.Images.OrderByDescending(i => i.PostingDate).Take(10).ToList();
                    return Ok(new { User = user, LastTenImages = lastTenImages });

                }
                return Ok(user.Images);
            }

            return NotFound("User not found");
        }

    }
}