using Microsoft.EntityFrameworkCore;
using DrDWebAPP.Models;

namespace DrDWebAPP.Data;

    public class DrDContext(DbContextOptions<DrDContext> options) : DbContext(options)
    {
    public DbSet<User> Users { get; set; }

    public DbSet<Character> Characters { get; set; }

    public DbSet<Dungeon> Dungeon { get; set; }
    }
