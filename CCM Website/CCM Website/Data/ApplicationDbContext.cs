using CCM_Website.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CCM_Website.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<CCM_Website.Models.Workbook> Workbooks { get; set; } = default!;
        public DbSet<CCM_Website.Models.Activities> Activities { get; set; } = default!;
        public DbSet<CCM_Website.Models.Week> Weeks { get; set; } = default!;
        public DbSet<CCM_Website.Models.LearningPlatform> LearningPlatforms { get; set; } =
            default!;
        public DbSet<CCM_Website.Models.GraduateAttribute> GraduateAttributes { get; set; } =
            default!;
        public DbSet<CCM_Website.Models.LearningPlatformActivities> LearningPlatformActivities { get; set; } =
            default!;
        public DbSet<CCM_Website.Models.WeekActivities> WeekActivities { get; set; } = default!;
        public DbSet<CCM_Website.Models.WeekGraduateAttributes> WeekGraduateAttributes { get; set; } =
            default!;
        public DbSet<CCM_Website.Models.LearningType> LearningType { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<LearningPlatformActivities>()
                .HasKey(l => new { l.LearningPlatformId, l.ActivitiesId });
            modelBuilder.Entity<WeekActivities>().HasKey(l => new { l.WeekActivityId });
            modelBuilder
                .Entity<WeekGraduateAttributes>()
                .HasKey(l => new { l.WeekId, l.GraduateAttributeId });
        }

        public DbSet<CCM_Website.Models.TaskLocation> TaskLocation { get; set; } = default!;
        public DbSet<CCM_Website.Models.TaskProgressStatus> TaskProgressStatus { get; set; } =
            default!;

        public DbSet<CCM_Website.Models.TaskApproach> TaskApproach { get; set; } = default!;
        public DbSet<CCM_Website.Models.UniversityArea> UniversityArea { get; set; } = default!;
    }
}
