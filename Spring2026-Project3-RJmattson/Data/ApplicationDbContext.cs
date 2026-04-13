using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spring2026_Project3_RJmattson.Models;

namespace Spring2026_Project3_RJmattson.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieActorRel> ActorMovies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MovieActorRel>()
                .HasIndex(ma => new { ma.ActorId, ma.MovieId })
                .IsUnique();
            modelBuilder.Entity<MovieActorRel>()
                .HasOne(ma => ma.Movie)
                .WithMany(m => m.ActorMovies)
                .HasForeignKey(ma => ma.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieActorRel>()
                .HasOne(ma => ma.Actor)
                .WithMany(a => a.ActorMovies)
                .HasForeignKey(ma => ma.ActorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}