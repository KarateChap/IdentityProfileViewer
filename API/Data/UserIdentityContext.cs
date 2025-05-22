using System;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserIdentityContext : DbContext
{
    public UserIdentityContext(DbContextOptions<UserIdentityContext> options) : base(options)
    {
    }

    public DbSet<UserIdentity> UserIdentities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserIdentity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SourceSystem).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastUpdated).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
            });
    }
}
