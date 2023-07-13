/*
* Autor: Ángel Ortega
* Name: loginapi
* Fecha: 10/10/2021
* Descripción: Este archivo contiene el código de la API de inicio de sesión
* Tecnologias: .NET 7.0, jwt, bcrypt, entity framework core, sql server
*/
using Microsoft.EntityFrameworkCore;

internal class modelContext : DbContext
{
    public DbSet<User>? Users { get; set; }
    public DbSet<VerificationUser>? VerificationUsers { get; set; }

    public modelContext(DbContextOptions<modelContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        try
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<VerificationUser>().ToTable("VerificationUsers");            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear la tabla 'Users': {ex.Message}");
        }
    }
}


