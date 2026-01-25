using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace SWD.DAL.Models;

public partial class IoTFinalDbContext : DbContext
{
    public IoTFinalDbContext()
    {
    }

    public IoTFinalDbContext(DbContextOptions<IoTFinalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AlertHistory> AlertHistories { get; set; }

    public virtual DbSet<AlertRule> AlertRules { get; set; }

    public virtual DbSet<Hub> Hubs { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Reading> Readings { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sensor> Sensors { get; set; }

    public virtual DbSet<SensorType> SensorTypes { get; set; }

    public virtual DbSet<Site> Sites { get; set; }

    public virtual DbSet<SystemLog> SystemLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(GetConnectionString());
        }
    }

    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
        var strConn = config["ConnectionStrings:DefaultConnection"];

        return strConn;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlertHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId);

            entity.ToTable("AlertHistory");

            entity.Property(e => e.HistoryId).HasColumnName("HistoryID");
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.ResolvedAt).HasColumnType("timestamp with time zone");
            entity.Property(e => e.RuleId).HasColumnName("RuleID");
            entity.Property(e => e.SensorId).HasColumnName("SensorID");
            entity.Property(e => e.Severity).HasMaxLength(20);
            entity.Property(e => e.TriggeredAt)
                .HasDefaultValueSql("NOW()")
                .HasColumnType("timestamp with time zone");

            entity.HasOne(d => d.Rule).WithMany(p => p.AlertHistories)
                .HasForeignKey(d => d.RuleId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Sensor).WithMany(p => p.AlertHistories)
                .HasForeignKey(d => d.SensorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AlertRule>(entity =>
        {
            entity.HasKey(e => e.RuleId);

            entity.ToTable("AlertRule");

            entity.Property(e => e.RuleId).HasColumnName("RuleID");
            entity.Property(e => e.ConditionType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.NotificationMethod)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Priority).HasMaxLength(20);
            entity.Property(e => e.SensorId).HasColumnName("SensorID");

            entity.HasOne(d => d.Sensor).WithMany(p => p.AlertRules)
                .HasForeignKey(d => d.SensorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Hub>(entity =>
        {
            entity.HasKey(e => e.HubId);

            entity.ToTable("Hub");

            entity.HasIndex(e => e.MacAddress, "UQ__Hub__MacAddress").IsUnique();

            entity.Property(e => e.HubId).HasColumnName("HubID");
            entity.Property(e => e.IsOnline).HasDefaultValue(false);
            entity.Property(e => e.LastHandshake).HasColumnType("timestamp with time zone");
            entity.Property(e => e.MacAddress)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.SiteId).HasColumnName("SiteID");

            entity.HasOne(d => d.Site).WithMany(p => p.Hubs)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotiId);

            entity.ToTable("Notification");

            entity.Property(e => e.NotiId).HasColumnName("NotiID");
            entity.Property(e => e.HistoryId).HasColumnName("HistoryID");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("NOW()")
                .HasColumnType("timestamp with time zone");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.History).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.HistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.OrgId);

            entity.ToTable("Organization");

            entity.Property(e => e.OrgId).HasColumnName("OrgID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()")
                .HasColumnType("timestamp with time zone");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Reading>(entity =>
        {
            entity.HasKey(e => e.ReadingId);

            entity.ToTable("Reading");

            entity.HasIndex(e => new { e.SensorId, e.RecordedAt }, "IDX_Reading_Sensor_Time");

            entity.Property(e => e.ReadingId).HasColumnName("ReadingID");
            entity.Property(e => e.RecordedAt)
                .HasPrecision(6)
                .HasDefaultValueSql("NOW()");
            entity.Property(e => e.SensorId).HasColumnName("SensorID");

            entity.HasOne(d => d.Sensor).WithMany(p => p.Readings)
                .HasForeignKey(d => d.SensorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__RoleName").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.HasKey(e => e.SensorId);

            entity.ToTable("Sensor");

            entity.Property(e => e.SensorId).HasColumnName("SensorID");
            entity.Property(e => e.HubId).HasColumnName("HubID");
            entity.Property(e => e.LastUpdate).HasPrecision(6);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.TypeId).HasColumnName("TypeID");

            entity.HasOne(d => d.Hub).WithMany(p => p.Sensors)
                .HasForeignKey(d => d.HubId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Type).WithMany(p => p.Sensors)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SensorType>(entity =>
        {
            entity.HasKey(e => e.TypeId);

            entity.ToTable("SensorType");

            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.TypeName).HasMaxLength(50);
            entity.Property(e => e.Unit).HasMaxLength(20);
        });

        modelBuilder.Entity<Site>(entity =>
        {
            entity.HasKey(e => e.SiteId);

            entity.ToTable("Site");

            entity.Property(e => e.SiteId).HasColumnName("SiteID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.GeoLocation).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.OrgId).HasColumnName("OrgID");

            entity.HasOne(d => d.Org).WithMany(p => p.Sites)
                .HasForeignKey(d => d.OrgId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.HasKey(e => e.LogId);

            entity.ToTable("SystemLog");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()")
                .HasColumnType("timestamp with time zone");
            entity.Property(e => e.Source)
                .HasMaxLength(50)
                .HasDefaultValue("EOH-Webhook");
            entity.Property(e => e.RawPayload)
                .HasColumnType("text");
            entity.Property(e => e.ErrorMessage)
                .HasColumnType("text");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__Email").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.OrgId).HasColumnName("OrgID");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SiteId).HasColumnName("SiteID");

            entity.HasOne(d => d.Org).WithMany(p => p.Users)
                .HasForeignKey(d => d.OrgId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Site).WithMany(p => p.Users)
                .HasForeignKey(d => d.SiteId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
