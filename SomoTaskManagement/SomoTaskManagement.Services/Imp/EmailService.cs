using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SendPasswordResetEmail(int memberId)
        {
            var member = await _unitOfWork.RepositoryMember.GetById(memberId);
            if (member == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }

            var emailContent = GeneratePasswordResetEmailContents(member.Name);

            try
            {
                var message = new MailMessage();
                message.From = new MailAddress("dohoangvu2001@gmail.com");
                message.To.Add(member.Email);
                message.Subject = "Mail chào mừng";
                message.Body = emailContent;
                message.IsBodyHtml = false;

                var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.Credentials = new NetworkCredential("dohoangvu2001@gmail.com", "eebr wfca lgco xpdn");
                smtpClient.EnableSsl = true;

                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to {member.Email}. Error: {ex.Message}");
            }
        }

        private static string GeneratePasswordResetEmailContents(string memberName)
        {
            //var resetLink = $"https://yourwebsite.com/reset-password?token={resetToken}";

            var emailContent = $"Chào {memberName},\n\n";
            emailContent += "Chào mừng bạn đến với Somo.\n\n";
            //emailContent += $"Please click the following link to reset your password:\n";
            //emailContent += "If you didn't request this, please ignore this email.\n\n";
            //emailContent += "Best regards,\nYour Company";

            return emailContent;
        }
    }
}
