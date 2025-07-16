using DadJokeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DadJokeConsoleApi.Data
{
    public class DadJokeDbContext : DbContext
    {
        public DadJokeDbContext(DbContextOptions<DadJokeDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<DadJoke> Jokes { get; set; }
    }
}
