using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.TaskEvidence
{
    public class EvidenceCreateUpdateModel
    {

        [Required(ErrorMessage = "Mô tả bắt buộc nhập")]
        public string Description { get; set; }


        public int TaskId { get; set; }

        [FromForm]
        public List<IFormFile>? ImageFile { get; set; }

        //public int TaskEvidenceId { set; get; }
    }
}
