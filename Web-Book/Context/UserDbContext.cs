using Microsoft.EntityFrameworkCore;
using Web_Book.Models;

namespace Web_Book.Context
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<News> NewsItems { get; set; }



    }
}
