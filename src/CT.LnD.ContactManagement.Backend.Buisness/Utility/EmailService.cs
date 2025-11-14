using CT.LnD.ContactManagement.Backend.Business.Utility.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace CT.LnD.ContactManagement.Backend.Business.Utility
{
    public class EmailService(ILogger<EmailService> logger, IConfiguration configuration) : IEmailService
    {
        public void SendExportedContacts(string to, string blobUrl)
        {
            SmtpClient client = new("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(configuration["Email:Address"], configuration["Email:Password"])
            };

            string from = configuration["Email:Address"];
            string subject = "Your Exported Contacts Are Ready!";
            string body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                <div style='max-width: 600px; margin: auto; background: white; border-radius: 8px; padding: 20px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'>
                    <h2 style='color: #333;'>Contacts Export Completed</h2>
                    <p style='font-size: 16px; color: #555;'>Your exported contacts are ready. Click the button below to download the CSV file:</p>
                    <a href='{blobUrl}' style='display: inline-block; padding: 12px 24px; font-size: 16px; color: white; background-color: #007bff; text-decoration: none; border-radius: 5px; margin-top: 10px;'>Download Contacts</a>
                </div>
            </body>
            </html>";

            MailMessage mailMessage = new(from, to, subject, body)
            {
                IsBodyHtml = true
            };

            try
            {
                client.Send(mailMessage);
            }
            catch (Exception e)
            {
                logger.LogError("Error while sending email");
                logger.LogError($"{e.Message}");
                throw new Exception("Failed to send email");
            }



        }

    }
}
