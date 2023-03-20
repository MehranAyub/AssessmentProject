using Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Data
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options)
     : base(options)
        {

        }
        public DbSet<User> User { get; set; }


    }
}