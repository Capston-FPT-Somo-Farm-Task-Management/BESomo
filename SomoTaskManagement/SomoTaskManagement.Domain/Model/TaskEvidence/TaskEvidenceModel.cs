using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.TaskEvidence
{
    public class TaskEvidenceModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Status { set; get; }
        public DateTime SubmitDate { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public int TaskId { get; set; }

        public List<string> UrlImage { get; set; }

        public string Time {  get; set; }

        public int? EvidenceType {  get; set; }
        public string? AvatarManager {  get; set; }
        public string? ManagerName {  get; set; }
    }
}
