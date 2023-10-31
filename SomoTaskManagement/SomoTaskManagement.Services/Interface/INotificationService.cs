using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Notification;
using SomoTaskManagement.Domain.Model.TaskEvidence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface INotificationService
    {
        Task<int> GetCount(int id);
        Task<List<Notification>> List();
        Task<IEnumerable<NotificationModel>> ListByMemberIsNew(int id);
        Task UpdateIsRead(int id);
        Task UpdateIsNew(int id);
        Task<NotificationPageResult> ListByMemberIsRead(int id, int pageIndex, int pageSize);
        Task<IEnumerable<Notification>> ListByMemberRead(int id);
        Task<NotificationPageResult> ListByMember(int id, int pageIndex, int pageSize);
        Task UpdateAllIsRead(int id);
    }
}
