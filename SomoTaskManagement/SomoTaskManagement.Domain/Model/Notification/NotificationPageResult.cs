using SomoTaskManagement.Domain.Model.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Notification
{
    public class NotificationPageResult
    {
        public IEnumerable<NotificationModel> Notifications { get; set; }
        public int TotalPages { get; set; }
    }
}
