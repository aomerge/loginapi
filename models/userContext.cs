// creamos el db context para ejecutar con entitifreamwork
// creamos la coneccion a la base de datos

using Microsoft.EntityFrameworkCore;
public class modelContext : DbContext
{
    public DbSet<User>? Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost;Database=MyDatabase;Trusted_Connection=True;");
    }
}