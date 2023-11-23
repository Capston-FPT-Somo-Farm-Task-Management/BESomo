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
    public class EvidencePendingAndCancel
    {
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }


        [FromForm]
        public List<IFormFile>? ImageFile { get; set; }
    }
}
