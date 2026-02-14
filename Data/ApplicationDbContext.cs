using Microsoft.EntityFrameworkCore;
using Application_task.Models;

namespace Application_task.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<UserData> UserDatas { get; set; }
        public DbSet<State> States { get; set; }
    }

    public class State
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
