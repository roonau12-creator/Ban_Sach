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
        public DbSet<Sach> Sachs { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(user => user.NormalizedEmail)
                .IsUnique();

            modelBuilder.Entity<Sach>()
                .HasOne(book => book.ChuDe)
                .WithMany(category => category.Sachs)
                .HasForeignKey(book => book.MaCD)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}