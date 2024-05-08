using Microsoft.EntityFrameworkCore;

namespace AccountServer.DB;

public class AppDbContext : DbContext
{
    public DbSet<User> User { get; set; }
    public DbSet<Unit> Unit { get; set; }
    public DbSet<UserUnit> UserUnit { get; set; }
    public DbSet<Deck> Deck { get;set; }
    public DbSet<DeckUnit> DeckUnit { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>().HasIndex(user => user.UserAccount).IsUnique();
        
        builder.Entity<Unit>(entity =>
        {
            entity.Property(unit => unit.UnitId).HasConversion(
                v => (int)v, v => (UnitId)v);
            entity.Property(unit => unit.Class).HasConversion(
                v => (int)v, v => (UnitClass)v);
            entity.Property(unit => unit.Species).HasConversion(
                v => (int)v, v => (UnitId)v);
            entity.Property(unit => unit.Role).HasConversion(
                v => (int)v, v => (UnitRole)v);
            entity.Property(unit => unit.Camp).HasConversion(
                v => (int)v, v => (Camp)v);
        });
        
        builder.Entity<DeckUnit>().HasKey(deckUnit => new { deckUnit.DeckId, deckUnit.UnitId });
        builder.Entity<DeckUnit>(entity =>
        {
            entity.Property(unit => unit.UnitId).HasConversion(
                v => (int)v, v => (UnitId)v);
        });
        
        builder.Entity<UserUnit>().HasKey(userUnit => new { userUnit.UserId, userUnit.UnitId });
        builder.Entity<UserUnit>(entity =>
        {
            entity.Property(unit => unit.UnitId).HasConversion(
                v => (int)v, v => (UnitId)v);
        });
        
        builder.Entity<ExpTable>().HasKey(e => e.Level);
        builder.Entity<ExpTable>().Property(e => e.Level).ValueGeneratedNever();
    }
}