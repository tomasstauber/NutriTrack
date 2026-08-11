using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace NutriTrack.Infraestructure.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarAsync(
            string destinatario,
            string asunto,
            string mensaje)
        {
            var servidor = _configuration["Email:SmtpServer"];
            var puerto = int.Parse(
                _configuration["Email:SmtpPort"] ?? "587");
            var usuario = _configuration["Email:Username"];
            var contraseña = _configuration["Email:Password"];

            using var smtp = new SmtpClient(servidor, puerto)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    usuario,
                    contraseña)
            };

            using var email = new MailMessage
            {
                From = new MailAddress(usuario!),
                Subject = asunto,
                Body = mensaje,
                IsBodyHtml = false
            };

            email.To.Add(destinatario);

            await smtp.SendMailAsync(email);
        }
    }
}
