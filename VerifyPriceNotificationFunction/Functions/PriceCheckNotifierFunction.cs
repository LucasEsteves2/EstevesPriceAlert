using EstevesPriceAlert.Core.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace VerifyPriceNotificationFunction.Functions;

public class PriceCheckNotifierFunction
{
    private readonly ILogger<PriceCheckNotifierFunction> _logger;
    private readonly IUserRepository _repo;

    public PriceCheckNotifierFunction(ILogger<PriceCheckNotifierFunction> logger, IUserRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    // roda 1x por minuto
    [Function("PriceWatcher")]
    public async Task Run([TimerTrigger("0 * * * * *")] TimerInfo _timer, CancellationToken ct)
    {
        _logger.LogInformation("PriceWatcher START {utc}", DateTimeOffset.UtcNow);

        var allUsers = await _repo.GetAllAsync();

        if (allUsers is null || allUsers.Count == 0)
        {
            _logger.LogDebug("Sem usuários.");
            return;
        }

        foreach (var user in allUsers)
        {
            var notifications = user?.Notifications;

            if (notifications is null || notifications.Count == 0)
                continue;

            foreach (var notification in notifications)
            {
                var hasUrls = notification?.IsActive == true && notification.ProductUrls is { Count: > 0 };

                if (hasUrls)
                {
                    var targetPrice = notification?.TargetPrice;

                    var bestPrice = await PriceUtils.GetBestPriceAsync(notification?.ProductUrls!, ct);

                    if (bestPrice.HasValue && bestPrice.Value <= targetPrice)
                    {
                        _logger.LogInformation("ALERTA user={UserId} price={Price} target={Target}", user.Id, bestPrice, notification.TargetPrice);

                        notification.LastNotifiedAtUtc = DateTime.UtcNow;
                        notification.NotifyCount = notification.NotifyCount <= 0 ? 1 : notification.NotifyCount + 1;

                        if (!string.IsNullOrWhiteSpace(user?.Email))
                            await SendEmailAlertAsync(user.Email, notification, bestPrice.Value, ct);

                        await _repo.UpdateAsync(user);
                    }
                }
            }
        }

        _logger.LogInformation("Verificador de preço Finalziado {utc}", DateTimeOffset.UtcNow);
    }

    private async Task SendEmailAlertAsync(string to, dynamic notification, decimal price, CancellationToken ct)
    {
        // >>> USE A SENHA DE APP DO GMAIL (não a senha normal) <<<
        const string GmailUser = "lucasesteves3235@gmail.com";
        const string GmailAppPassword = "<SUA_SENHA_DE_APP_AQUI>"; // ex: abcd efgh ijkl mnop
        const string FromName = "Esteves Price Alert";

        var subject = $"🔔 Preço atingido: {price:C}";
        var urls = (IEnumerable<string>)(notification?.ProductUrls ?? Array.Empty<string>());
        var firstUrl = urls.FirstOrDefault() ?? "https://www.google.com"; // se quiser, a primeira URL
        var productName = (string?)notification?.ProductName ?? "Produto monitorado";
        var targetText = (notification?.TargetPrice > 0 ? ((decimal)notification.TargetPrice).ToString("C") : "—");

        // HTML bonitão (compatível com e-mail)
        string htmlBody = $@"
<!DOCTYPE html>
<html lang='pt-BR'>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
  <title>Alerta de Preço</title>
</head>
<body style='margin:0;padding:0;background:#f4f7fb;font-family:Arial,Helvetica,sans-serif;color:#1f2937;'>
  <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='background:#f4f7fb;padding:24px 0;'>
    <tr>
      <td align='center'>
        <table role='presentation' width='640' cellpadding='0' cellspacing='0' style='max-width:640px;width:100%;background:#ffffff;border-radius:12px;box-shadow:0 2px 12px rgba(31,41,55,0.08);overflow:hidden;'>
          <!-- Header -->
          <tr>
            <td style='background:#0ea5e9;padding:20px 24px;'>
              <table width='100%'>
                <tr>
                  <td align='left'>
                    <img src='https://i.imgur.com/2Wf3B2g.png' alt='Esteves Price Alert' width='140' style='display:block;border:0;outline:none;text-decoration:none;'/>
                  </td>
                  <td align='right' style='color:#e0f2fe;font-size:12px;'>
                    {DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Title -->
          <tr>
            <td style='padding:28px 24px 8px 24px;'>
              <div style='font-size:18px;color:#6b7280;margin-bottom:6px;'>Alerta de Preço</div>
              <div style='font-size:22px;font-weight:700;color:#111827;line-height:1.35;'>{WebUtility.HtmlEncode(productName)}</div>
            </td>
          </tr>

          <!-- Card with price -->
          <tr>
            <td style='padding:0 24px 8px 24px;'>
              <table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='background:#f9fafb;border:1px solid #e5e7eb;border-radius:10px;'>
                <tr>
                  <td style='padding:20px;'>
                    <table width='100%'>
                      <tr>
                        <td style='font-size:14px;color:#6b7280;'>Preço encontrado</td>
                        <td align='right' style='font-size:14px;color:#6b7280;'>Preço alvo</td>
                      </tr>
                      <tr>
                        <td style='font-size:28px;font-weight:800;color:#16a34a;'>{price:C}</td>
                        <td align='right' style='font-size:20px;font-weight:700;color:#111827;'>{targetText}</td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- URLs -->
          <tr>
            <td style='padding:4px 24px 8px 24px;'>
              <div style='font-size:14px;color:#6b7280;margin-bottom:8px;'>URLs monitoradas</div>
              <table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='background:#ffffff;border:1px dashed #e5e7eb;border-radius:10px;'>
                <tr>
                  <td style='padding:12px 16px;'>
                    {(urls.Any()
                            ? string.Join("", urls.Select(u =>
                                $"<div style='margin:6px 0;line-height:1.4;'><a href='{WebUtility.HtmlEncode(u)}' style='color:#0ea5e9;text-decoration:none;'>{WebUtility.HtmlEncode(u)}</a></div>"))
                            : "<div style='color:#9ca3af;'>Sem URLs cadastradas</div>")}
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- CTA -->
          <tr>
            <td style='padding:16px 24px 24px 24px;'>
              <a href='{WebUtility.HtmlEncode(firstUrl)}'
                 style='display:inline-block;background:#0ea5e9;color:#ffffff;text-decoration:none;padding:12px 18px;border-radius:10px;font-weight:700;'>
                 Ver produto
              </a>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style='padding:20px 24px;background:#f9fafb;border-top:1px solid #e5e7eb;color:#6b7280;font-size:12px;'>
              Você recebeu este e-mail porque configurou um alerta de preço no Esteves Price Alert.
              <br/>Se não foi você, ignore esta mensagem.
            </td>
          </tr>
        </table>

        <div style='color:#9ca3af;font-size:12px;margin-top:12px;'>
          © {DateTime.UtcNow:yyyy} Esteves Price Alert
        </div>
      </td>
    </tr>
  </table>
</body>
</html>";

        // Fallback em texto puro
        var textBody = new StringBuilder()
            .AppendLine("Alerta de Preço")
            .AppendLine(productName)
            .AppendLine()
            .AppendLine($"Preço encontrado: {price:C}")
            .AppendLine($"Preço alvo:      {targetText}")
            .AppendLine()
            .AppendLine("URLs monitoradas:")
            .AppendLine(string.Join(Environment.NewLine, urls))
            .AppendLine()
            .AppendLine("— Esteves Price Alert")
            .ToString();

        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(GmailUser, FromName),
                Subject = subject,
                Body = textBody,         // default (caso cliente não aceite HTML)
                IsBodyHtml = false
            };
            message.To.Add(new MailAddress(to));

            // anexa versão HTML como AlternateView
            var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, "text/html");
            message.AlternateViews.Add(htmlView);

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(GmailUser, GmailAppPassword)
            };

            await Task.Run(() => smtp.Send(message), ct);
            _logger.LogInformation("Email enviado para {to}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar e-mail para {to}", to);
        }
    }
}