/*
* Autor: Ángel Ortega
* Name: loginapi
* Fecha: 10/10/2021
* Descripción: Este archivo contiene el código de la API de inicio de sesión
* Tecnologias: .NET 7.0, jwt, bcrypt, entity framework core, sql server
*/
// creamos el modelos de verificacion de usuario 
// en el cual se guardaran los datos del usuario que se esta verificando
// tendra que venir una referencia al modelo de usuario y un estado de verificacion
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class VerificationUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    public int? Id { get; set; }
    [ForeignKey("id_user")]
    public int? UserId { get; set; }
    [DefaultValue(false)]
    public bool Verified { get; set; }

}