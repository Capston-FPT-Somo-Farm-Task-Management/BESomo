using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Notification
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public string Message { get; set; } = null!;
        public string MessageType { get; set; } = null!;
        public DateTime NotificationDateTime { get; set; }
        public bool IsRead { get; set; }
        public bool IsNew { get; set; }
        public string Time { get; set; }
        public int TaskId {  get; set; }
    }
}
