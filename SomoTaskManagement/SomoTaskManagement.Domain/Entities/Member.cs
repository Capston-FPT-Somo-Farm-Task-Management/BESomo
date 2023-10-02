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
            Tasks = new HashSet<FarmTask>();
            MemberTokens = new HashSet<MemberToken>();
        }
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { set; get; }

        [StringLength(100, ErrorMessage = "UserName cannot exceed 100 characters.")]
        [Required(ErrorMessage = "UserName is required.")]
        public string UserName { set; get; }


        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#$%^&+=!])(?!.*\s).{8,}$",
        ErrorMessage = "Password must meet the complexity requirements.")]
        public string Password { set; get; }

        [StringLength(10, ErrorMessage = "Phone number must be exactly 10 digits.", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only digits.")]
        public int PhoneNumber { set; get; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1900-01-01", "2005-01-01", ErrorMessage = "Please enter a valid date.")]
        [Required(ErrorMessage = "Birthday is required.")]
        public DateTime Birthday { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        [Required(ErrorMessage = "Address is required.")]
        public string Address { set; get; }
        public int RoleId { set; get; }
        public int FarmId { set; get; }
        public int? HubConnectionId { set; get; }
        public int? NotificcationId { set; get; }

        [JsonIgnore]
        public virtual Role? Role { set; get; }

        [JsonIgnore]
        public virtual Farm? Farm { set; get; }
        public virtual HubConnection? HubConnection { set; get; }
        public virtual Notification? Notification { set; get; }
        [JsonIgnore]
        public virtual ICollection<FarmTask> Tasks { set; get; }
        [JsonIgnore]
        public virtual ICollection<MemberToken> MemberTokens { set; get; }
    }
}
