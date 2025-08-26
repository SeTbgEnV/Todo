using Microsoft.EntityFrameworkCore;
using ToDo.Models;

namespace ToDo.Data;
public class ToDoDbContext(DbContextOptions<ToDoDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Item> Items { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       modelBuilder.Entity<Item>()
           .HasOne(i => i.User)
           .WithMany(u => u.Items)
           .HasForeignKey(i => i.UserId);
   }

}
