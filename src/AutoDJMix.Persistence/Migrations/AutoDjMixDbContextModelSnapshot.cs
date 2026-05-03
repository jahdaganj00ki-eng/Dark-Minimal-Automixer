using AutoDJMix.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace AutoDJMix.Persistence.Migrations;

[DbContext(typeof(AutoDjMixDbContext))]
public partial class AutoDjMixDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.5");

        modelBuilder.Entity<TrackEntity>(b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("TEXT");

            b.Property<string>("ContentHashSha256")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("DisplayName")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<long>("DurationMs")
                .HasColumnType("INTEGER");

            b.Property<string>("SourcePath")
                .IsRequired()
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.HasIndex("ContentHashSha256");

            b.HasIndex("SourcePath")
                .IsUnique();

            b.ToTable("Tracks");
        });
    }
}
