using CallQuality.Core.DataAccess.Context.Entities;
using Microsoft.EntityFrameworkCore;

namespace CallQuality.Core.DataAccess.Context;

public partial class ADUsersDbContext : DbContext
{
    public ADUsersDbContext()
    {
    }

    public ADUsersDbContext(DbContextOptions<ADUsersDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ADUser> ADUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ADUser>(entity =>
        {
            entity.ToTable("ADUser", "dbo");

            entity.HasKey(e => e.ADUserID);

            entity.Property(e => e.ADUserID)
                .HasColumnName("ADUserID");

            entity.Property(e => e.ID)
                .HasColumnName("ID");

            entity.Property(e => e.ID_Guid)
                .HasColumnName("ID_Guid");

            entity.Property(e => e.DisplayName)
                .HasColumnName("DisplayName");

            entity.Property(e => e.GivenName)
                .HasColumnName("GivenName");

            entity.Property(e => e.Mail)
                .HasColumnName("Mail");

            entity.Property(e => e.Surname)
                .HasColumnName("Surname");

            entity.Property(e => e.UserPrincipalName)
                .HasColumnName("UserPrincipalName");

            entity.Property(e => e.EmployeeId)
                .HasColumnName("EmployeeId");

            entity.Property(e => e.Department)
                .HasColumnName("Department");

            entity.Property(e => e.JobTitle)
                .HasColumnName("JobTitle");

            entity.Property(e => e.Extension)
                .HasColumnName("Extension");

            entity.Property(e => e.AccountEnabled)
                .HasColumnName("AccountEnabled");

            entity.Property(e => e.Manager_ID)
                .HasColumnName("Manager_ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}