using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model
{
    public class TaskCreateUpdateModel
    {

        [Required]
        //[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Name must contain only letters.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "StartDate is required.")]
        public DateTime StartDate { set; get; }

        [Required(ErrorMessage = "EndDate is required.")]
        public DateTime EndDate { set; get; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { set; get; }


        [Required(ErrorMessage = "Priority is required.")]
        public string Priority { set; get; }
        public string Repeat { get; set; }
        public int Iterations { get; set; }

        public int ReceiverId { set; get; }
        public int? FieldId { set; get; }
        public int TaskTypeId { set; get; }
        public int MemberId { set; get; }
        public int? OtherId { set; get; }
        public int? PlantId { set; get; }
        public int? LiveStockId { set; get; }
        public int Remind { set; get; }
    }
}
