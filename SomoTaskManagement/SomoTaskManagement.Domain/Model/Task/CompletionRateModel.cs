using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class CompletionRateModel
    {
        public DateTime StartDay {  get; set; }
        public DateTime EndDay {  get; set; }
        public string Time {  get; set; }
    }
}
