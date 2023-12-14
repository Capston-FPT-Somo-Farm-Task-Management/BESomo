using Quartz;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Infrastructure.Quart
{
    public class LateStartBackgroundJob : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationFCM _notificationFCM;

        public LateStartBackgroundJob(IUnitOfWork unitOfWork, NotificationFCM notificationFCM)
        {
            _unitOfWork = unitOfWork;
            _notificationFCM = notificationFCM;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); 

            DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            var tasks = await _unitOfWork.RepositoryFarmTask.GetData(t => t.StartDate > t.CreateDate && (t.StartDate.Value.AddMinutes(30) < currentDate) && t.Status == 1);

            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    task.IsStartLate = true;
                    _unitOfWork.RepositoryFarmTask.Update(task);
                    await _unitOfWork.RepositoryFarmTask.Commit();

                    string message = $"Nhiệm vụ '{task.Name}' đã bắt đầu trễ";

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
                        var supervisorTokens = await _notificationFCM.GetTokenByMemberIds(memberIds);
                        await _notificationFCM.SendNotificationToDeviceAndMembers(supervisorTokens, message, memberIds, task.Id);
                    }
                }
            }
        }
    }
}
