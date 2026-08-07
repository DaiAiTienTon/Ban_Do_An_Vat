using Ban_Do_An_Vat.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ban_Do_An_Vat.Data
{
    // Design-time factory cho PostgreSQL migrations (Render/Neon).
    // Tao 07/08/2026 - dung rieng de scaffold migrations PostgreSQL,
    // KHONG anh huong den SQL Server migrations trong Data/Migrations/.
    public class ApplicationDbContextPostgresFactory : IDesignTimeDbContextFactory<ApplicationDbContextPostgres>
    {
        public ApplicationDbContextPostgres CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                ?? "postgresql://neondb_owner:npg_3FORPdBsikN1@ep-blue-cell-axqvbg3f-pooler.c-4.us-east-2.aws.neon.tech/neondb?sslmode=require&channel_binding=require";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContextPostgres>();
            optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory");
                npgsqlOptions.MigrationsAssembly("Ban_Do_An_Vat");
            });

            return new ApplicationDbContextPostgres(optionsBuilder.Options);
        }
    }
}
