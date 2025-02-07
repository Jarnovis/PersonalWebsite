using System.Net;
using System.Net.Mail;
using WebApi.Enviroment;

namespace WebApi.Services.EmailServices;

public class EmailService : IDisposable
{
    private readonly EnvConfig _envConfig;
    private bool _disposed = false;

    public EmailService(EnvConfig envConfig)
    {
        _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
    }

    private SmtpClient CreateSmptClient()
    {
        return new SmtpClient(_envConfig.Get("SMTP_HOST"))
        {
            Port = int.Parse(_envConfig.Get("SMTP_PORT")),
            Credentials = new NetworkCredential(
                _envConfig.Get("SMTP_EMAIL"),
                _envConfig.Get("SMTP_PASSWORD")
            ),
            EnableSsl = true
        };
    }

    public async Task Send(string toEmail, string subject, string body)
    {
        try
        {
            using (SmtpClient SmtpClient = CreateSmptClient())
            using (MailMessage message = new MailMessage
            {
                From = new MailAddress(_envConfig.Get("SMTP_EMAIL")),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                message.To.Add(toEmail);
                await SmtpClient.SendMailAsync(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                (_envConfig as IDisposable)?.Dispose();
            }

            _disposed = true;
        }
    }

    ~EmailService()
    {
        Dispose(false);
    }
}