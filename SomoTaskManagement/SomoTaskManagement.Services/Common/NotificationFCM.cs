using FirebaseAdmin.Messaging;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Common
{
    public class NotificationFCM
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationFCM(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;
        }
        public async Task SendNotificationToDeviceAndMembers(List<string> deviceTokens, string message, List<int> memberIds, int taskId)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            var task = await _unitOfWork.RepositoryFarmTask.GetById(taskId);
            var deviceMessages = deviceTokens.Select(token => new Message
            {
                Token = token,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = $"{task.Name}",
                    Body = message
                },
                Data = new Dictionary<string, string>
                {
                    { "TaskId", taskId.ToString() }
                }
            }).ToList();

            // Send notifications to devices
            foreach (var deviceMessage in deviceMessages)
            {
                await SendNotificationToDevices(new List<string> { deviceMessage.Token }, deviceMessage);
            }

            foreach (var memberId in memberIds)
            {
                var individualNotification = new Domain.Entities.Notification
                {
                    Message = message,
                    MessageType = "Individual",
                    NotificationDateTime = vietnamTime,
                    IsRead = false,
                    IsNew = true,
                    TaskId = taskId
                };
                await _unitOfWork.RepositoryNotifycation.Add(individualNotification);
                await _unitOfWork.RepositoryNotifycation.Commit();

                var member_notify = new Notification_Member
                {
                    NotificationId = individualNotification.Id,
                    MemberId = memberId,
                };
                await _unitOfWork.RepositoryNotifycation_Member.Add(member_notify);
                await _unitOfWork.RepositoryNotifycation_Member.Commit();
            }
        }

        public async Task SendNotificationToDevices(List<string> deviceTokens, Message message)
        {
            foreach (var deviceToken in deviceTokens)
            {
                message.Token = deviceToken;

                var messaging = FirebaseMessaging.DefaultInstance;
                await messaging.SendAsync(message);
            }
        }

        public async Task<List<string>> GetTokenByMemberIds(List<int> memberIds)
        {
            var hubConnections = await _unitOfWork.RepositoryHubConnection.GetData(h => memberIds.Contains(h.MemberId));

            if (hubConnections == null || !hubConnections.Any())
            {
                throw new Exception("Không tìm thấy kết nối");
            }

            var connectionIds = hubConnections.Select(h => h.ConnectionId).ToList();
            return connectionIds;
        }
    }
}
