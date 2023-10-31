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
    public class TaskEvidenceUpdateModel
    {
        public EvidenceCreateUpdateModel EvidenceCreateUpdateModel { get; set; }    
        public List<string>? OldUrlImage {  get; set; }
    }
}
