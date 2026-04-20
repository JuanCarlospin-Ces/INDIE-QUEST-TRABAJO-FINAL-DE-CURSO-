using Microsoft.EntityFrameworkCore;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.ValueObject;

namespace IndieQuest_Api.Infrastructure;

public class IndieQuestDbContext : DbContext
{
    public IndieQuestDbContext(DbContextOptions<IndieQuestDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<UserPost> UserPosts { get; set; }
    public DbSet<PostTag> PostTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de la tabla User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId)
                .HasColumnName("idUser")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Username)
                .HasColumnName("userName")
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Password)
                .HasColumnName("password")
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.UserBio)
                .HasColumnName("userBio")
                .HasMaxLength(1000);
            entity.Property(e => e.UserProfilePicture)
                .HasColumnName("ProfilePicture")
                .HasMaxLength(500);
            entity.Property(e => e.AvailableForWork)
                .HasColumnName("availableForWork")
                .HasDefaultValue(false);
            entity.Property(e => e.dateOfRegistration)
                .HasColumnName("dateOfRegistration")
                .HasDefaultValueSql("CURRENT_DATE");
            
            // Configurar relación con UserPost
            entity.HasMany(u => u.UserPosts)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de la tabla Post
        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("Post");
            entity.HasKey(e => e.PostId);
            entity.Property(e => e.PostId)
                .HasColumnName("idPost")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Title)
                .HasColumnName("postTitle")
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.MediaContent)
                .HasColumnName("mediaContent")
                .HasMaxLength(500);
            entity.Property(e => e.Description)
                .HasColumnName("Description")
                .HasMaxLength(2000);
            entity.Property(e => e.CreationDate)
                .HasColumnName("CreationDate")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Configurar relaciones
            entity.HasMany(p => p.UserPosts)
                .WithOne(up => up.Post)
                .HasForeignKey(up => up.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(p => p.PostTags)
                .WithOne(pt => pt.Post)
                .HasForeignKey(pt => pt.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de la tabla Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tag");
            entity.HasKey(e => e.tagId);
            entity.Property(e => e.tagId)
                .HasColumnName("idTag")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.tagName)
                .HasColumnName("tagName")
                .IsRequired()
                .HasMaxLength(100);
            
            // Configurar relación con PostTag
            entity.HasMany(t => t.PostTags)
                .WithOne(pt => pt.Tag)
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de la tabla de relación Makes_MadeBy
        modelBuilder.Entity<UserPost>(entity =>
        {
            entity.ToTable("Makes_MadeBy");
            entity.HasKey(up => new { up.UserId, up.PostId });
            entity.Property(e => e.UserId)
                .HasColumnName("idUser");
            entity.Property(e => e.PostId)
                .HasColumnName("idPost");
        });

        // Configuración de la tabla de relación Has_Tag
        modelBuilder.Entity<PostTag>(entity =>
        {
            entity.ToTable("Has_Tag");
            entity.HasKey(pt => new { pt.PostId, pt.TagId });
            entity.Property(e => e.PostId)
                .HasColumnName("idPost");
            entity.Property(e => e.TagId)
                .HasColumnName("idTag");
        });
    }
}
