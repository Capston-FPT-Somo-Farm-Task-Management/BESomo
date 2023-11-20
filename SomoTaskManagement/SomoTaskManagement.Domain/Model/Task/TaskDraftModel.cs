using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class TaskDraftModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        public DateTime? StartDate { set; get; }

        public DateTime? EndDate { set; get; }

        public string? Description { set; get; }
        [Required]
        public string? Priority { set; get; }
        public int? SupervisorId { get; set; }
        public int? ManagerId { get; set; }
        public int? FieldId { set; get; }
        [Required]
        public bool? IsRepeat { get; set; }
        public int? TaskTypeId { set; get; }
        public int? PlantId { set; get; }
        public int? LiveStockId { set; get; }
        public string? AddressDetail { get; set; }
        [Required]
        public int? Remind { get; set; }
        public bool? IsPlant { get; set; }
        public bool IsSpecific { get; set; }
    }
}
