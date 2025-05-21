using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using prjEventosWeb.Models;

namespace prjEventosWeb.Services
{
    public interface IEmailService
    {
        Task<bool> EnviarEmailConfirmacaoInscricao(ApplicationUser usuario, Evento evento);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> EnviarEmailConfirmacaoInscricao(ApplicationUser usuario, Evento evento)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpServer = emailSettings["SmtpServer"];
                var porta = int.Parse(emailSettings["Porta"]);
                var usuarioEmail = emailSettings["Usuario"];
                var senhaEmail = emailSettings["Senha"];
                var remetente = emailSettings["Remetente"];
                var nomeRemetente = emailSettings["NomeRemetente"];

                var client = new SmtpClient(smtpServer, porta)
                {
                    Credentials = new NetworkCredential(usuarioEmail, senhaEmail),
                    EnableSsl = true
                };

                var mensagem = new MailMessage
                {
                    From = new MailAddress(remetente, nomeRemetente),
                    Subject = $"Confirmação de Inscrição: {evento.Nome}",
                    Body = GerarCorpoEmail(usuario, evento),
                    IsBodyHtml = true
                };

                mensagem.To.Add(new MailAddress(usuario.Email));

                await client.SendMailAsync(mensagem);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar email de confirmação.");
                Console.WriteLine("Erro ao enviar email: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
                return false;
            }
        }

        private string GerarCorpoEmail(ApplicationUser usuario, Evento evento)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Confirmação de Inscrição</title>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #4285f4; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; background-color: #f9f9f9; }}
                    .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #777; }}
                    .details {{ margin: 20px 0; }}
                    .details div {{ margin-bottom: 10px; }}
                    .bold {{ font-weight: bold; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Eventos Web</h1>
                        <h2>Confirmação de Inscrição</h2>
                    </div>
                    <div class='content'>
                        <p>Olá, <span class='bold'>{usuario.UserName}</span>!</p>
                        
                        <p>Sua inscrição no evento <span class='bold'>{evento.Nome}</span> foi confirmada com sucesso!</p>
                        
                        <div class='details'>
                            <div><span class='bold'>Evento:</span> {evento.Nome}</div>
                            <div><span class='bold'>Apresentador:</span> {evento.Apresentador}</div>
                            <div><span class='bold'>Data de Início:</span> {evento.DataInicio:dd/MM/yyyy HH:mm}</div>
                            <div><span class='bold'>Data de Término:</span> {evento.DataFim:dd/MM/yyyy HH:mm}</div>
                            <div><span class='bold'>Local:</span> {evento.Local}</div>
                            <div><span class='bold'>Preço:</span> {(evento.Preco > 0 ? $"R$ {evento.Preco:F2}" : "Gratuito")}</div>
                        </div>
                        
                        <p>Obrigado por se inscrever! Esperamos você no evento.</p>
                        
                        <p>Para mais informações ou para cancelar sua inscrição, acesse sua área de usuário em nosso site.</p>
                    </div>
                    <div class='footer'>
                        <p>Este é um email automático. Por favor, não responda.</p>
                        <p>&copy; {DateTime.Now.Year} - Eventos Acadêmicos</p>
                    </div>
                </div>
            </body>
            </html>";
        }
    }
}