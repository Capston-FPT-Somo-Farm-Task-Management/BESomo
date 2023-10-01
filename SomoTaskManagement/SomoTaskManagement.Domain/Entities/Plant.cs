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
    [Table("Plant")]

    public class Plant:BaseEntity
    {
        public Plant()
        {
            Tasks = new HashSet<FarmTask>();
        }
        [Required(ErrorMessage = "ExternalId is required.")]
        [StringLength(20, ErrorMessage = "ExternalId cannot exceed 20 characters.")]
        public string ExternalId {set;get;}

        [Required(ErrorMessage = "CreateDate is required.")]
        public DateTime CreateDate { set; get; }

        [Required(ErrorMessage = "Height is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Height must be greater than 0.")]
        public int Height { set; get; }

        public int HabitantTypeId {set;get;}
        
        public int FieldId {set;get;}

        [JsonIgnore]
        public virtual ICollection<FarmTask> Tasks { set; get; }
        [JsonIgnore]
        public virtual HabitantType? HabitantType { set; get; }
        [JsonIgnore]
        public virtual Field? Field { set; get; }
    }
}
