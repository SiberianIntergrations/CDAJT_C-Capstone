using Microsoft.EntityFrameworkCore;
using back_end.domain.Entities;

namespace back_end.domain.DbContexts
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Billing> Bills { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<DiningSession> DiningSessions { get; set; } = null!;
    public DbSet<Locations> Locations { get; set; } = null!;
    public DbSet<MenuItemAssignment> MenuItemAssignments { get; set; } = null!;
    public DbSet<Menu_Item> MenuItems { get; set; } = null!;
    public DbSet<MenuLocations> MenuLocations { get; set; } = null!;
    public DbSet<Menu> Menus { get; set; } = null!;
    public DbSet<MenuItemTag> MenuItemTags { get; set; } = null!;
    public DbSet<OrderItems> OrderItems { get; set; } = null!;
    public DbSet<ServiceRequest> ServiceRequests { get; set; } = null!;
    public DbSet<SessionOrder> SessionOrders { get; set; } = null!;
    public DbSet<SessionParticipant> SessionParticipants { get; set; } = null!;
    public DbSet<TableEntity> Tables { get; set; } = null!;
    public DbSet<TableGroup> TableGroups { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // Composite keys for junction tables - REQUIRED since EF can't infer these
      modelBuilder.Entity<MenuItemTag>()
          .HasKey(mit => new { mit.Menu_item_id, mit.Tag_id });

      modelBuilder.Entity<MenuItemAssignment>()
          .HasKey(mia => new { mia.Menu_Id, mia.Item_Id });

      modelBuilder.Entity<MenuLocations>()
          .HasKey(ml => new { ml.Menu_Id, ml.Location_Id });

      modelBuilder.Entity<DiningSession>()
          .HasOne(ds => ds.Table)
          .WithMany(t => t.DiningSessions)
          .HasForeignKey(ds => ds.Table_Id)
          .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<DiningSession>()
          .HasOne(ds => ds.TableGroup)
          .WithMany(tg => tg.DiningSessions)
          .HasForeignKey(ds => ds.TableGroup_Id)
          .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<TableEntity>()
          .HasOne(t => t.TableGroup)
          .WithMany(tg => tg.Tables)
          .HasForeignKey(t => t.TableGroup_Id)
          .OnDelete(DeleteBehavior.SetNull);

      modelBuilder.Entity<DiningSession>()
          .ToTable(t => t.HasCheckConstraint(
              "CK_DiningSession_TableAssignment",
              "(Table_Id IS NOT NULL AND TableGroup_Id IS NULL) OR (Table_Id IS NULL AND TableGroup_Id IS NOT NULL)"));

      modelBuilder.Entity<ServiceRequest>()
          .HasOne(sr => sr.RequestedByUser)
          .WithMany(u => u.RequestedServices)
          .HasForeignKey(sr => sr.Request_By)
          .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<ServiceRequest>()
          .HasOne(sr => sr.ClaimedByUser)
          .WithMany(u => u.ClaimedServices)
          .HasForeignKey(sr => sr.Claimed_By)
          .OnDelete(DeleteBehavior.SetNull);

      base.OnModelCreating(modelBuilder);
    }
  }
}