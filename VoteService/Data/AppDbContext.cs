using Microsoft.EntityFrameworkCore;
using VoteSystem.Models;

namespace VoteSystem.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Vote> Votes { get; set; }
}