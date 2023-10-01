using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    public class Notification
    {
        //public Notification()
        //{
        //    Members = new HashSet<Member>();
        //}

        [Key]
        public int Id { get; set; }
        public string Message { get; set; } = null!;
        public string MessageType { get; set; } = null!;
        public DateTime NotificationDateTime { get; set; }

        //public virtual ICollection<Member> Members { get; set; }
    }
}
