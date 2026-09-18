using Microsoft.Extensions.Options;
using Resend;
using Ara.Application.Common.Interfaces;

namespace Ara.Infrastructure.Email;

public class ResendEmailSender(IResend resend, IOptions<ResendOptions> options) : IEmailSender
{
    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        await resend.EmailSendAsync(new EmailMessage
        {
            From = options.Value.FromAddress,
            To = to,
            Subject = subject,
            HtmlBody = htmlBody
        }, cancellationToken);
    }
}
