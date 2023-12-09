using FirebaseAdmin.Messaging;
using Quartz;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Infrastructure.Quart
{
    public class FarmTaskBackgroundJob : IJob
    {
        private readonly IUnitOfWork _unitOfWork;

        public FarmTaskBackgroundJob(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            var tasks = await _unitOfWork.RepositoryFarmTask.GetData(t => t.IsExpired == false && t.StartDate < t.CreateDate && t.EndDate <= currentDate && (t.Status != 7 && t.Status != 8 && t.Status != 4 && t.Status != 0));
            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    task.IsExpired = true;
                    _unitOfWork.RepositoryFarmTask.Update(task);
                    await _unitOfWork.RepositoryFarmTask.Commit();

                    string message = $"Nhiệm vụ '{task.Name}' đã hết hạn";

                    List<int> memberIds = new List<int>();
                    if (task.SuppervisorId.HasValue)
                    {
                        memberIds.Add(task.SuppervisorId.Value);
                    }
                    if (task.ManagerId.HasValue)
                    {
                        memberIds.Add(task.ManagerId.Value);
                    }

                    if (memberIds.Any())
                    {
                        var supervisorTokens = await GetTokenByMemberIds(memberIds);
                        await SendNotificationToDeviceAndMembers(supervisorTokens, message, memberIds, task.Id);
                    }
                }
            }
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
