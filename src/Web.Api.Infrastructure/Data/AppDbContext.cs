﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.Shared;
namespace Web.Api.Infrastructure.Data;
public class AppDbContext : DbContext
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<User> Users { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<User>(ConfigureUser);
    public void ConfigureUser(EntityTypeBuilder<User> builder)
    {
        var navigation = builder.Metadata.FindNavigation(nameof(User.RefreshTokens));
        //EF access the RefreshTokens collection property through its backing field
        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(b => b.Email);
        builder.OwnsOne(o => o.Address);
    }
    public override int SaveChanges()
    {
        AddAuditInfo();
        return base.SaveChanges();
    }
    public async Task<int> SaveChangesAsync()
    {
        AddAuditInfo();
        return await base.SaveChangesAsync();
    }
    private void AddAuditInfo()
    {
        var entries = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                ((BaseEntity)entry.Entity).Created = DateTimeOffset.UtcNow;
            ((BaseEntity)entry.Entity).Modified = DateTimeOffset.UtcNow;
        }
    }
}