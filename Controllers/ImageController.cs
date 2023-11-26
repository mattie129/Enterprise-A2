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
    public class ImageController : ControllerBase
    {
        private ImageDatabase _db = new ImageDatabase();



        [HttpGet("populartags")]
        public async Task<IActionResult> GetPopularTags()
        {
            var popularTags = _db.Tags
                .GroupBy(tag => tag.Text) 
                .OrderByDescending(group => group.Count()) 
                .Take(5) 
                .Select(group => new { Tag = group.Key, Count = group.Count() })
                .ToList();

            return Ok(popularTags);
        }




        [HttpGet("random")]
        public async Task<IActionResult> GetRandomImages(int count)
        {
            var allImages = _db.Images.ToList(); 

            var random = new Random();

            //randomize list then return specified amount
            var randomImages = allImages
                                        .OrderBy(x => random.Next())
                                        .Take(count)
                                        .ToList();

            return Ok(randomImages);
        }


        [HttpGet("byTag")]
        public async Task<IActionResult> GetImagesByTag(string tag, int page = 1, int pageSize = 10)
        {
            var images = _db.Images
                .Where(i => i.Tags.Any(t => t.Text == tag))
                .OrderByDescending(i => i.PostingDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (images.Count == 0)
            {
                return NotFound("No images found with given tag.");
            }

            var response = new PagedResponse<Image>(images);
            var total = await _db.Images.CountAsync();
            
            response.Links = LinksGenerator.GenerateLinks("/api/Image", page, total, pageSize);

            return Ok(response);
            
        }


        [HttpGet("{imageId}")]
        public async Task<IActionResult> GetImageById(Guid imageID)
        {
            var image = await _db.Images.FirstOrDefaultAsync(x => x.Id == imageID);
            if (image == null)
                return NotFound();

            return Ok(image);
        }


        
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

            
            response.Links = LinksGenerator.GenerateLinks("/api/Image", page, total, pageSize);

            return Ok(response);
            
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

                //generate tags from ImageHelper, and add each tag from image to database
                IEnumerable<string> tags = ImageHelper.GetTags(image.Url);
                foreach (var tagText in tags)
                {
                    Tag tag = new Tag { Text = tagText };

                    tag.Images = new List<Image> { image };

                    await _db.Tags.AddAsync(tag);
                }
                await _db.Images.AddAsync(image);
                await _db.SaveChangesAsync();


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

    }
}