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
    [Table("HabitantType")]

    public class HabitantType :BaseEntity   
    {
        public HabitantType()
        {
            Plants = new HashSet<Plant>();
            LiveStocks = new HashSet<LiveStock>();
        }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        [Required(ErrorMessage = "Quantity is required.")]
        public int Quantity { set; get; }

        [JsonIgnore]
        public virtual ICollection<Plant> Plants { get; set; }

        [JsonIgnore]
        public virtual ICollection<LiveStock> LiveStocks { get; set; }
    }
}
