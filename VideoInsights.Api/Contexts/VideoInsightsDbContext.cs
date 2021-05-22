using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using VideoInsights.Api.Models;

#nullable disable

namespace VideoInsights.Api.Contexts
{
    public partial class VideoInsightsDbContext : DbContext
    {
        public VideoInsightsDbContext()
        {
        }

        public VideoInsightsDbContext(DbContextOptions<VideoInsightsDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Keyframe> Keyframes { get; set; }
        public virtual DbSet<Label> Labels { get; set; }
        public virtual DbSet<Thumbnail> Thumbnails { get; set; }
        public virtual DbSet<Timeperiod> Timeperiods { get; set; }
        public virtual DbSet<Video> Videos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pg_buffercache")
                .HasPostgresExtension("pg_stat_statements")
                .HasAnnotation("Relational:Collation", "English_United States.1252");

            modelBuilder.Entity<Keyframe>(entity =>
            {
                entity.ToTable("keyframe");

                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .HasColumnName("id");

                entity.Property(e => e.Videoid)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("videoid");
            });

            modelBuilder.Entity<Label>(entity =>
            {
                entity.ToTable("label");

                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .HasColumnName("id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("content");

                entity.Property(e => e.Keyframeid)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("keyframeid");
            });


            modelBuilder.Entity<Thumbnail>(entity =>
            {
                entity.ToTable("thumbnail");

                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .HasColumnName("id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content");

                entity.Property(e => e.Externalid)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("externalid");
            });

            modelBuilder.Entity<Timeperiod>(entity =>
            {
                entity.ToTable("timeperiod");

                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .HasColumnName("id");

                entity.Property(e => e.Endtime)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("endtime");

                entity.Property(e => e.Keyframeid)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("keyframeid");

                entity.Property(e => e.Starttime)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("starttime");
            });

            modelBuilder.Entity<Video>(entity =>
            {
                entity.ToTable("video");

                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .HasColumnName("id");

                entity.Property(e => e.Created).HasColumnName("created");

                entity.Property(e => e.Lastindexed).HasColumnName("lastindexed");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Uri)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .HasColumnName("uri");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
