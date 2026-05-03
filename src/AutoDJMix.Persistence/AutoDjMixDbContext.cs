using AutoDJMix.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoDJMix.Persistence;

public sealed class AutoDjMixDbContext : DbContext
{
    public AutoDjMixDbContext(DbContextOptions<AutoDjMixDbContext> options) : base(options)
    {
    }

    public DbSet<TrackEntity> Tracks => Set<TrackEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackEntity>(b =>
        {
            b.ToTable("Tracks");
            b.HasIndex(x => x.SourcePath).IsUnique();
            b.HasIndex(x => x.ContentHashSha256);
        });
    }
}
