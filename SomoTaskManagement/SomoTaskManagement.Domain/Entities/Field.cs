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
    [Table("Field")]

    public class Field:BaseEntity
    {
        public Field()
        {
            Plants = new HashSet<Plant>();
            Tasks = new HashSet<FarmTask>();
            LiveStocks = new HashSet<LiveStock>();
        }

        [Required(ErrorMessage = "Area is required.")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "FarmArea must be greater than 0.")]
        public double Area { set; get; }

        public int ZoneId { set; get; }

        [JsonIgnore]
        public virtual Zone? Zone { set; get; }
        [JsonIgnore]
        public virtual ICollection<Plant> Plants { set; get; }
        [JsonIgnore]
        public virtual ICollection<LiveStock> LiveStocks { set; get; }
        [JsonIgnore]
        public virtual ICollection<FarmTask> Tasks { set; get; }
    }
}
