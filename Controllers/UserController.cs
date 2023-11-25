using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using API.Services;



namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : ControllerBase
    {
        //private PeopleDbContext _db = new PeopleDbContext();



        


/*
        [HttpPost]
        public async Task<IActionResult> PopulatePeople(Person[] people)
        {
            await _db.People.AddRangeAsync(people);
            await _db.SaveChangesAsync();
            return Created("", "Database poplulated");
        }*/
    }
}