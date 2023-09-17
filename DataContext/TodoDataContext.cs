using Microsoft.EntityFrameworkCore;
using TodoList.Models;

namespace TodoList.DataContext
{
    public class TodoDataContext: DbContext
    {
        public TodoDataContext(DbContextOptions<TodoDataContext> options) : base(options)
        {
            
        }

        public DbSet<TodoItem> TodoList { get; set; }
        public DbSet<ProfilePic> Images { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
