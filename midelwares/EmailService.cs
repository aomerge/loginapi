using System;
using System.Net;
using System.Net.Mail;

// Clase para enviar correos electrónicos
    public class EmailService
    {
        // Método para enviar un correo electrónico de verificación
            public void SendVerificationEmail(string recipientEmail, string? verificationLink)
            {
                // Configurar la información del remitente y el destinatario
                    string? senderEmail = "sterling48@ethereal.email";
                    string? senderPassword = "dJUPA6sZzKbwd4sKcK";
                    string? emailSubject = "Verificación de correo electrónico";
                    string? emailBody = $"Por favor, haga clic en el siguiente enlace para verificar su correo electrónico: {verificationLink}";
                // Configurar el cliente SMTP
                    SmtpClient smtpClient = new SmtpClient("smtp.ethereal.email", 587 );
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtpClient.EnableSsl = true;
                // Crear el mensaje de correo electrónico
                    MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail, emailSubject, emailBody);
                    mailMessage.IsBodyHtml = true;
                // Enviar el correo electrónico
                    try
                    {
                        smtpClient.Send(mailMessage);
                        Console.WriteLine("Correo electrónico enviado exitosamente.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al enviar el correo electrónico: " + ex.Message);
                    }
            }
        // Metodo de verificacion de correo electronico exitosa
            public void SendVerificationEmailSuccess(string recipientEmail)
            {
                // Configurar la información del remitente y el destinatario
                    string? senderEmail = " ";
                    string? senderPassword = " ";
                    string? emailSubject = "Verificación de correo electrónico";
                    string? emailBody = $"Su correo electrónico ha sido verificado exitosamente";
                // Configurar el cliente SMTP
                    SmtpClient smtpClient = new SmtpClient("smtp.ethereal.email", 587 );
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtpClient.EnableSsl = true;
                // Crear el mensaje de correo electrónico
                    MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail, emailSubject, emailBody);
                    mailMessage.IsBodyHtml = true;
                // Enviar el correo electrónico
                    try
                    {
                        smtpClient.Send(mailMessage);
                        Console.WriteLine("Correo electrónico enviado exitosamente.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al enviar el correo electrónico: " + ex.Message);
                    }
            }
    internal void SendVerificationEmail(object email, object verificationLink)
    {
        throw new NotImplementedException();
    }
}
