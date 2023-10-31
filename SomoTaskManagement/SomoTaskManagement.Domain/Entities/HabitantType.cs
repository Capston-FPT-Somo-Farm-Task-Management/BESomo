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
        public string? Origin {  get; set; }
        public string? Environment {  get; set; }
        public string? Description {  get; set; }

        public bool? IsActive {  get; set; }
        [JsonIgnore]
        public virtual ICollection<Plant> Plants { get; set; }

        [JsonIgnore]
        public virtual ICollection<LiveStock> LiveStocks { get; set; }
    }
}
