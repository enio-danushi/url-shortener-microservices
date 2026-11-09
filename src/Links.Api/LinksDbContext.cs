using Microsoft.EntityFrameworkCore;

namespace Links.Api;

public class LinksDbContext(DbContextOptions<LinksDbContext> options) : DbContext(options)
{
    public DbSet<Link> Links => Set<Link>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Link>(link =>
        {
           link.ToTable("links");
           link.HasKey(l => l.Code);
           link.Property(l => l.Code).HasColumnName("code").HasMaxLength(16);
           link.Property(l => l.Url).HasColumnName("url");
           link.Property(l => l.CreatedAt).HasColumnName("created_at");
           link.Property(l => l.DisabledAt).HasColumnName("disabled_at");
        });
    }
}