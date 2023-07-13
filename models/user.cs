/*
* Autor: Ángel Ortega
* Name: loginapi
* Fecha: 10/10/2021
* Descripción: Este archivo contiene el código de la API de inicio de sesión
* Tecnologias: .NET 7.0, jwt, bcrypt, entity framework core, sql server
*/

// creamos la clase User para poder guardar los datos de los usuarios en la base de datos
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Index(nameof(user_handle), nameof(Email), IsUnique = true)]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    [Column("id_user")]
    public int Id_user { get; set; }
    public string? Username { get; set; }
    [Required]
    [Column("user_handle")]
    [StringLength(50)]
    [MinLength(3)]
    [MaxLength(50)]
    [RegularExpression("@^[a-zA-Z0-9_]+$")]
    public string? user_handle {get; set; }
    [Required]
    [Column("email")]
    [StringLength(150)]
    [MinLength(3)]
    [MaxLength(150)]
    [RegularExpression(@"^[a-zA-Z0-9_]+$")]
    [EmailAddress]
    public string? Email { get; set; }
    [Required]
    [Column("password")]
    [StringLength(150)]
    [MinLength(8)]
    [MaxLength(250)]
    [RegularExpression(@"^[a-zA-Z0-9_]+$")]
    public string? Password { get; set; }

    public static implicit operator User(int v)
    {
        throw new NotImplementedException();
    }
}
