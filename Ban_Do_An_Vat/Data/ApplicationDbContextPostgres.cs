using Microsoft.EntityFrameworkCore;

namespace Ban_Do_An_Vat.Data
{
    // DbContext rieng cho PostgreSQL (Render/Neon).
    // Tao 07/08/2026 - ke thua ApplicationDbContext nhung dung Npgsql provider.
    // Migrations cua class nay duoc luu vao Data/MigrationsPostgres/
    // hoan toan doc lap voi migrations SQL Server o Data/Migrations/.
    public class ApplicationDbContextPostgres : ApplicationDbContext
    {
        public ApplicationDbContextPostgres(DbContextOptions<ApplicationDbContextPostgres> options)
            : base(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(((Microsoft.EntityFrameworkCore.Infrastructure.IDbContextOptions)options)
                    .FindExtension<Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal.NpgsqlOptionsExtension>()!.ConnectionString)
                .Options)
        {
        }
    }
}
