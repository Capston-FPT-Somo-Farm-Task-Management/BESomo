using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    public class Notification_Member
    {
        public int MemberId { get; set; }
        public int NotificationId { get; set; }

        public bool Status {  get; set; }

        public virtual Member Member { get; set; }

        public virtual Notification Notification { get; set; }
    }
}
