using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ban_Do_An_Vat.Data
{
    public class ApplicationDbContextPostgresFactory : IDesignTimeDbContextFactory<ApplicationDbContextPostgres>
    {
        public ApplicationDbContextPostgres CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContextPostgres>();
            builder.UseNpgsql("Host=localhost;Database=dummy;Username=postgres;Password=postgres");
            return new ApplicationDbContextPostgres(builder.Options);
        }
    }
}
