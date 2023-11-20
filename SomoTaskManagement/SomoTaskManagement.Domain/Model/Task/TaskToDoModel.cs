using System.ComponentModel.DataAnnotations;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class TaskToDoModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "StartDate is required.")]
        public DateTime? StartDate { set; get; }

        [Required(ErrorMessage = "EndDate is required.")]
        public DateTime? EndDate { set; get; }

        public string? Description { set; get; }

        [Required(ErrorMessage = "Priority is required.")]
        public string Priority { set; get; }
        public int? SupervisorId { set; get; }
        public int? ManagerId { get; set; }
        public int? FieldId { set; get; }
        public bool? IsRepeat { set; get; }
        public int TaskTypeId { set; get; }
        public int? PlantId { set; get; }
        public int? LiveStockId { set; get; }
        public string? AddressDetail { get; set; }
        public int Remind { get; set; }
        public bool? IsPlant { get; set; }
        public bool IsSpecific { get; set; }
    }
}
