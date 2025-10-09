using ContentCreator.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ContentCreator.Infrastructure.Persistence.Repositories
{
    public class EmailService : IEmailService
    {
        private readonly IContentCreatorDBContext _dbContext;
        public EmailService(IContentCreatorDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> SendEmailMessageAsync(string emailId, string subject, string emailMessage, string otp, string firstName)
        {
            string returnMessage = "Email Template not found";
            try
            {
                if (!IsValidEmail(emailId))
                    returnMessage = "Invalid email address";
                else
                {
                    var getEmailTemplate = await _dbContext.EmailTemplates.Where(x => x.Subject == subject).FirstOrDefaultAsync();
                    if (getEmailTemplate != null)
                    {
                        string body = ReplacePlaceholders(getEmailTemplate.Body, firstName, emailMessage, otp);
                        SendEmailAsync(subject, body, emailId);
                        returnMessage = "Email sent";
                    }
                }
            }
            catch (Exception ex)
            {
                returnMessage = ex.Message;
            }
            return returnMessage;
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private string ReplacePlaceholders(string templateBody, string firstName, string EmailMessage, string otp)
        {
            templateBody = templateBody.Replace("{UserName}", firstName);
            templateBody = templateBody.Replace("{EmailMessage}", EmailMessage);
            templateBody = templateBody.Replace("{OTP}", otp);
            return templateBody;
        }
        private async Task SendEmailAsync(string subject, string body, string recipientEmail)
        {
            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("pramveersingh847@gmail.com");
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.To.Add(recipientEmail);
                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
                    {
                        smtp.Port = 587;
                        smtp.UseDefaultCredentials = false;
                        smtp.EnableSsl = true;
                        smtp.Credentials = new System.Net.NetworkCredential("pramveersingh847@gmail.com", "dfuysntaiinbypbw");

                        await smtp.SendMailAsync(mailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
