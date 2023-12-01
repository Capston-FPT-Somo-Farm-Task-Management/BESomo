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

        public async Task SendPasswordResetEmail(string email)
        {
            var member = await _unitOfWork.RepositoryMember.GetSingleByCondition(m=>m.Email == email)?? throw new Exception("Không tìm thấy người dùng");
            
            var newPassword = Random.Shared.Next(600000, 999999).ToString();
            var emailContent = GeneratePasswordResetEmailContents(member.Name, newPassword);
            string currentPassword = newPassword;

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(currentPassword);
            member.Password = hashedPassword;

            await _unitOfWork.RepositoryMember.Commit();
            try
            {
                var message = new MailMessage();
                message.From = new MailAddress("dohoangvu2001@gmail.com");
                message.To.Add(member.Email);
                message.Subject = "Somo Task Management - Đặt lại mật khẩu";
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

        private static string GeneratePasswordResetEmailContents(string memberName, string newPassword)
        {
            //var resetLink = $"https://yourwebsite.com/reset-password?token={resetToken}";

            var emailContent = $"Chào {memberName},\n\n";
            emailContent += "Chào mừng bạn đến với Somo.\n\n";
            emailContent += $"Mật khẩu mới của bạn là {newPassword}.\n";
            emailContent += $"Cảm ơn bạn đã sử dụng dịch vụ.\n";

            return emailContent;
        }
    }
}
