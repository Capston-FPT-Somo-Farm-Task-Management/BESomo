using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class TaskCreateAsignModel
    {
        //public string Code {  get; set; }
        [Required]
        //[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Name must contain only letters.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "StartDate is required.")]
        public DateTime StartDate { set; get; }

        [Required(ErrorMessage = "EndDate is required.")]
        public DateTime EndDate { set; get; }
        public DateTime? UpdateDate { set; get; }

        public string? Description { set; get; }


        [Required(ErrorMessage = "Priority is required.")]
        public string Priority { set; get; }

        public int SuppervisorId { set; get; }
        public int? FieldId { set; get; }
        public int TaskTypeId { set; get; }
        public int? PlantId { set; get; }
        public int? LiveStockId { set; get; }
        public string? AddressDetail { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "OverallEfforMinutes must be greater than 0.")]
        public int OverallEfforMinutes { set; get; }
        [Range(0, int.MaxValue, ErrorMessage = "OverallEffortHour must be greater than 0.")]
        public int OverallEffortHour { set; get; }
        public bool? IsPlant { get; set; }
        public bool IsSpecific { get; set; }
    }
}
