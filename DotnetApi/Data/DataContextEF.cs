using DotnetApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DotnetApi.Data;

public class DataContextEF : DbContext
{
    public readonly IConfiguration _configuration;

    public DataContextEF(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserJobInfo> UserJobInfo { get; set; }

    public virtual DbSet<UserSalary> UserSalary { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
    {
        if (!dbContextOptionsBuilder.IsConfigured)
        {
            dbContextOptionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"),
            dbContextOptionsBuilder => dbContextOptionsBuilder.EnableRetryOnFailure());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("TutorialAppSchema");
        modelBuilder.Entity<User>()
        .ToTable("Users", "TutorialAppSchema")
        .HasKey(e => e.UserId);

        modelBuilder.Entity<UserSalary>()
        .ToTable("UserSalary", "TutorialAppSchema")
        .HasKey(e => e.UserId);

        modelBuilder.Entity<UserJobInfo>()
        .ToTable("UserJobInfo", "TutorialAppSchema")
        .HasKey(e => e.UserId);
    }




}