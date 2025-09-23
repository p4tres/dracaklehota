using Microsoft.EntityFrameworkCore;
using DrDWebAPP.Models;
using DrDWebAPP.Models.ReadOnly;

namespace DrDWebAPP.Data;

    public class DrDContext(DbContextOptions<DrDContext> options) : DbContext(options)
    {
    public DbSet<User> Users { get; set; }

    public DbSet<Character> Characters { get; set; }

    public DbSet<Dungeon> Dungeon { get; set; }

    public DbSet<ProfessionAttributes> ProfessionAttributes { get; set; }
    public DbSet<RaceAttributes> RaceAttributes { get; set; }
    public DbSet<AttributesModifiers> AttributesModifiers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RaceAttributes>().ToView(null);
        modelBuilder.Entity<AttributesModifiers>().ToView(null);
        modelBuilder.Entity<ProfessionAttributes>().ToView(null);
    }
    }