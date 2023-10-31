using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    public class Notification
    {
        public Notification()
        {
            Notification_Members = new HashSet<Notification_Member>();
        }

        [Key]
        public int Id { get; set; }
        public string Message { get; set; } = null!;
        public string MessageType { get; set; } = null!;
        public DateTime NotificationDateTime { get; set; }
        public bool IsRead{  get; set; }
        public bool IsNew { get; set; }
        public int TaskId {  get; set; }
        [JsonIgnore]
        public virtual ICollection<Notification_Member> Notification_Members { get; set; }
    }
}
