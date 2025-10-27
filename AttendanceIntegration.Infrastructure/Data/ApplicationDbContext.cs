using AttendanceIntegration.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceIntegration.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AttendanceRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EmployeeName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Source).HasMaxLength(50);
            entity.HasIndex(e => new { e.CompanyId, e.Date, e.EmployeeId });
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Timestamp);
        });
    }
}
