using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CCM_Website.Models;
using Microsoft.AspNetCore.Builder;

namespace CCM_Website.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CCM_Website.Models.Course> Courses { get; set; } = default!;
    }
}
