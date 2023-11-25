using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Models.Entities;

namespace API.Services
{

    public class ImageDatabase : DbContext
    {

        public DbSet<Image> Images { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users {get;set;}


        protected override void OnConfiguring
                        (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=imagedata.db");
        }
    }
}