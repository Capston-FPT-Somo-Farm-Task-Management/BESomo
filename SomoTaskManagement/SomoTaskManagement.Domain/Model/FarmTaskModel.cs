using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model
{
    public class FarmTaskModel
    {
        public int Id { get; set; }
        [Required]
        //[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Name must contain only letters.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        public string Status { set; get; }

        [Required(ErrorMessage = "CreateDate is required.")]
        public DateTime CreateDate { set; get; }

        [Required(ErrorMessage = "StartDate is required.")]
        public DateTime StartDate { set; get; }

        [Required(ErrorMessage = "EndDate is required.")]
        public DateTime EndDate { set; get; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { set; get; }


        [Range(0, int.MaxValue, ErrorMessage = "Priority must be greater than 0.")]
        [Required(ErrorMessage = "Priority is required.")]
        public string Priority { set; get; }
        public string Repeat { get; set; }
        public int Iterations { get; set; }

        public string ReceiverName { set; get; }
        public int ReceiverId { set; get; }
        public string? FieldName { set; get; }
        public string TaskTypeName { set; get; }
        public string StatusTaskType { set; get; }
        public string MemberName { set; get; }
        public string? OtherName { set; get; }
        public string? PlantName { set; get; }
        public string? liveStockName { set; get; }

        public int Remind { get; set; }

        public string ExternalId { set; get; }
        public string ZoneName { set; get; }
        public string AreaName { set; get; }

        public string EmployeeName {  set; get; }
    }
}
