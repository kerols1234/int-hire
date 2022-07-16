using GB_Backend.Models;
using GB_Backend.Models.APIModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GB_Backend.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> op) : base(op)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<JobApplicant>().HasKey(ja => new { ja.ApplicantUserId, ja.JobId });
        }

        public DbSet<ApplicantUser> ApplicantUsers { get; set; }
        public DbSet<RecruiterUser> RecruiterUsers { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<MBTIType> MBTITypes { get; set; }
        public DbSet<MPTIModel> MPTIModels { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<JobApplicant> JobApplicants { get; set; }
    }
}
