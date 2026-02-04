using Microsoft.EntityFrameworkCore;
using MemoryApp.Data.Models;

namespace MemoryApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Verb> Verbs => Set<Verb>();
    public DbSet<VerbForm> VerbForms => Set<VerbForm>();
    public DbSet<Image> Images => Set<Image>();
    public DbSet<VerbImage> VerbImages => Set<VerbImage>();
    public DbSet<Sentence> Sentences => Set<Sentence>();
    public DbSet<Attempt> Attempts => Set<Attempt>();
    public DbSet<UserVerbProgress> UserVerbProgress => Set<UserVerbProgress>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();
    public DbSet<Noun> Nouns => Set<Noun>();
    public DbSet<UserNounProgress> UserNounProgress => Set<UserNounProgress>();
    public DbSet<AppSettings> AppSettings => Set<AppSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DisplayName).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Verb configuration
        modelBuilder.Entity<Verb>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Infinitive).IsRequired();
            entity.HasIndex(e => e.Infinitive).IsUnique();
            entity.Property(e => e.TranslationEs).IsRequired();
            entity.Property(e => e.Level).IsRequired();
            entity.Property(e => e.Auxiliary).IsRequired();
            entity.Property(e => e.PartizipII).IsRequired();
        });

        // VerbForm configuration
        modelBuilder.Entity<VerbForm>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Verb)
                .WithMany(v => v.Forms)
                .HasForeignKey(e => e.VerbId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Image configuration
        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired();
            entity.HasIndex(e => e.Key).IsUnique();
            entity.Property(e => e.PathOrData).IsRequired();
        });

        // VerbImage (many-to-many) configuration
        modelBuilder.Entity<VerbImage>(entity =>
        {
            entity.HasKey(e => new { e.VerbId, e.ImageId });

            entity.HasOne(e => e.Verb)
                .WithMany(v => v.VerbImages)
                .HasForeignKey(e => e.VerbId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Image)
                .WithMany(i => i.VerbImages)
                .HasForeignKey(e => e.ImageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Sentence configuration
        modelBuilder.Entity<Sentence>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Level).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.SentenceTemplate).IsRequired();
            entity.Property(e => e.CorrectAnswer).IsRequired();

            entity.HasOne(e => e.Verb)
                .WithMany(v => v.Sentences)
                .HasForeignKey(e => e.VerbId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Attempt configuration
        modelBuilder.Entity<Attempt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Mode).IsRequired();
            entity.Property(e => e.ItemType).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Attempts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserVerbProgress configuration (composite key)
        modelBuilder.Entity<UserVerbProgress>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.VerbId });
            entity.Property(e => e.Mastery).IsRequired();
            entity.Property(e => e.NextReviewAt).IsRequired();
            entity.Property(e => e.LastResult).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.VerbProgress)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Verb)
                .WithMany(v => v.UserProgress)
                .HasForeignKey(e => e.VerbId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserSettings configuration
        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.PreferredLevel).IsRequired();
            entity.Property(e => e.DailyGoal).IsRequired();
            entity.Property(e => e.UiTheme).IsRequired();

            entity.HasOne(e => e.User)
                .WithOne(u => u.Settings)
                .HasForeignKey<UserSettings>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Noun configuration
        modelBuilder.Entity<Noun>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.German).IsRequired();
            entity.HasIndex(e => e.German).IsUnique();
            entity.Property(e => e.Article).IsRequired();
            entity.Property(e => e.TranslationEs).IsRequired();
            entity.Property(e => e.Level).IsRequired();
        });

        // UserNounProgress configuration (composite key)
        modelBuilder.Entity<UserNounProgress>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.NounId });
            entity.Property(e => e.Mastery).IsRequired();
            entity.Property(e => e.NextReviewAt).IsRequired();
            entity.Property(e => e.LastResult).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.NounProgress)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Noun)
                .WithMany(n => n.UserProgress)
                .HasForeignKey(e => e.NounId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AppSettings configuration (single row)
        modelBuilder.Entity<AppSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
