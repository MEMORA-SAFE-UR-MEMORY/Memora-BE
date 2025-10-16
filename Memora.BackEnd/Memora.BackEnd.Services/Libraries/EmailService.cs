using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Memora.BackEnd.Services.Libraries
{
	public class EmailService
	{
		private readonly IConfiguration _config;
		public EmailService(IConfiguration config)
		{
			_config = config;
		}

		public async Task SendEmailAsync(string toEmail, string subject, BodyBuilder bodyBuilder)
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("Memora Support", _config["Email:From"]));
			message.To.Add(new MailboxAddress("", toEmail));
			message.Subject = subject;
			message.Body = bodyBuilder.ToMessageBody();

			using var client = new SmtpClient();
			await client.ConnectAsync(_config["Email:SmtpHost"], int.Parse(_config["Email:SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
			await client.AuthenticateAsync(_config["Email:Username"], _config["Email:Password"]);
			await client.SendAsync(message);
			await client.DisconnectAsync(true);
		}

		public async Task SendEmailAsync(string toEmail, string subject, string textBody)
		{
			var bodyBuilder = new BodyBuilder { TextBody = textBody };
			await SendEmailAsync(toEmail, subject, bodyBuilder);
		}
	}
}