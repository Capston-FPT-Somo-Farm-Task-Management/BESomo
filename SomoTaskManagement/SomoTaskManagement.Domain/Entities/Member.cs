using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    [Table("Member")]
    public class Member:BaseEntity
    {
        public Member()
        {
            TaskManagers = new HashSet<FarmTask>();
            //TaskSupervisors = new HashSet<FarmTask>();
            MemberTokens = new HashSet<MemberToken>();
            Notification_Members = new HashSet<Notification_Member>();
        }
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { set; get; }

        [StringLength(100, ErrorMessage = "UserName cannot exceed 100 characters.")]
        [Required(ErrorMessage = "UserName is required.")]
        public string UserName { set; get; }


        //[Required(ErrorMessage = "Password is required.")]
        //[RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#$%^&+=!])(?!.*\s).{8,}$",
        //ErrorMessage = "Password must meet the complexity requirements.")]
        public string Password { set; get; }
        public string? Code { set; get; }

        public string PhoneNumber { set; get; }


        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Range(typeof(DateTime), "1900-01-01", "2005-01-01", ErrorMessage = "Please enter a valid date.")]
        //[Required(ErrorMessage = "Birthday is required.")]
        public DateTime Birthday { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        [Required(ErrorMessage = "Address is required.")]
        public string Address { set; get; }
        public int RoleId { set; get; }
        public int FarmId { set; get; }
        public int? HubConnectionId { set; get; }
        public string Avatar { get; set; }
        [JsonIgnore]
        public virtual Role? Role { set; get; }

        [JsonIgnore]
        public virtual Farm? Farm { set; get; }
        [JsonIgnore]
        public virtual HubConnection? HubConnection { set; get; }
        [JsonIgnore]
        public virtual ICollection<FarmTask>? TaskManagers { set; get; }
        //[JsonIgnore]
        //public virtual ICollection<FarmTask>? TaskSupervisors { set; get; }
        [JsonIgnore]
        public virtual ICollection<MemberToken>? MemberTokens { set; get; }
        [JsonIgnore]
        public virtual ICollection<Notification_Member>? Notification_Members { set; get; }
    }
}
