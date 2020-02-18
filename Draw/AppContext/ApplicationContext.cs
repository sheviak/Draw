using Draw.Entities;
using System.Data.Entity;

namespace Draw.AppContext
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Number> Numbers { get; set; }
        public DbSet<NumProperties> NumProperties { get; set; }

        public ApplicationContext() : base("NumbersContext")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NumProperties>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Number>()
                .HasKey(x => x.Id)
                .HasRequired(r => r.NumProperties)
                .WithRequiredPrincipal(c => c.Number)
                .WillCascadeOnDelete(true);
        }
    }
}