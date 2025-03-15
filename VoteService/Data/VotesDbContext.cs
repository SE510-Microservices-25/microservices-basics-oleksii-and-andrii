using System.Reflection;
using Microsoft.EntityFrameworkCore;
using VoteSystem.Entities;

namespace VoteSystem.Data;

public class VotesDbContext(DbContextOptions<VotesDbContext> options) : DbContext(options)
{
    public DbSet<VoteEntity> Votes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}