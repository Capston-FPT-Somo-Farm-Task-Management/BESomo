using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.TaskEvidence
{
    public class NotificationWithCount
    {
        public IEnumerable<NotificationModel> Notifications { get; set; }
        public int Count { get; set; }
    }
}
