using CompanyEmployees.Entities.Configuration;
using CompanyEmployees.Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CompanyEmployees.Entities.DataTransferObjects;

namespace CompanyEmployees.Repository
{
    public class RepositoryContext : IdentityDbContext<User>
    {
        public RepositoryContext(DbContextOptions options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }

        public DbSet<Company>? Companies { get; set; }
        public DbSet<Employee>? Employees { get; set; }
        public DbSet<Issue> IssuesDetails { get; set; }
        public DbSet<Notification> NotificationDetails { get; set; }
        public DbSet<UserForRegistrationDto>? UserForRegistrationDto { get; set; }
        public DbSet<UserInformations> UserInfo { get; set; }
        public DbSet<FileUpload>? FileUploadDetails { get; set; }
        public DbSet<CompanyEmployees.Entities.Models.Folder>? Folder { get; set; }
        public DbSet<Group> GroupDetails { get; set; }
        public DbSet<UserActivity> UserActivityDetails { get; set; }
        public DbSet<GroupMessage> GroupMessageDetails { get; set; }
    }
}
