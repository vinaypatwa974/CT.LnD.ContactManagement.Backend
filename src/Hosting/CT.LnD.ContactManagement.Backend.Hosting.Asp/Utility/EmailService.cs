using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility
{
    public class EmailService(ILogger<EmailService> logger, IConfiguration configuration) : IEmailService
    {
        public void SendVerificationEmail(string to, SecurityToken token)
        {
            SmtpClient client = new("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(configuration["Email:Address"], configuration["Email:Password"])
            };

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            string verificationUrl = $"https://contactmanagement-abhinav-webapp-dev-c0avbhageubtfxfq.canadacentral-01.azurewebsites.net/verify-email?token={tokenString}";
            string from = configuration["Email:Address"]!;
            string subject = "Welcome To Contact Managemnet App  | Email Verification";
            string body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Email Verification</h2>
                <p>Please click the button below to verify your email address:</p>
                <a href='{verificationUrl}' style='display: inline-block; padding: 10px 20px; font-size: 16px; color: white; background-color: #4CAF50; text-decoration: none; border-radius: 5px;'>Verify Email</a>
                <p>If you did not request this, please ignore this email.</p>
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


        public void SendExportedContacts(string to, string blobUrl)
        {
            SmtpClient client = new("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(configuration["Email:Address"], configuration["Email:Password"])
            };

            string from = configuration["Email:Address"]!;
            string subject = "Your Exported Contacts Are Ready!";
            string body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                <div style='max-width: 600px; margin: auto; background: white; border-radius: 8px; padding: 20px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'>
                    <h2 style='color: #333;'>Contacts Export Completed</h2>
                    <p style='font-size: 16px; color: #555;'>Your exported contacts are ready. Click the button below to download the CSV file:</p>
                    <a href='{blobUrl}' style='display: inline-block; padding: 12px 24px; font-size: 16px; color: white; background-color: #007bff; text-decoration: none; border-radius: 5px; margin-top: 10px;'>Download Contacts</a>
                    <p style='font-size: 14px; color: #888; margin-top: 20px;'>If you did not request this export, you can safely ignore this email.</p>
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
