using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    [Table("Material_Task")]

    public class Material_Task
    {
        [Key]
        public int MaterialId { get; set; }

        [Key]
        public int TaskId { get; set; }

        public bool Status { get; set; }

        public virtual FarmTask Task { set; get; }
        public virtual Material Material { set; get; }
    }
}
