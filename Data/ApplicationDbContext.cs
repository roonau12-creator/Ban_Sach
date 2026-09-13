using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BanSach.Models;
using Microsoft.EntityFrameworkCore;
namespace BanSach.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {
            
        }
        public DbSet<ChuDe>ChuDes { get; set; }
    }
}