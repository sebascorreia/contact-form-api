using ContactAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace ContactAPI.Data
{
    public class ContactDbContext : DbContext
    {
        public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options) { }

        public DbSet<ContactSubmission> ContactSubmissions => Set<ContactSubmission>();

    }
}
