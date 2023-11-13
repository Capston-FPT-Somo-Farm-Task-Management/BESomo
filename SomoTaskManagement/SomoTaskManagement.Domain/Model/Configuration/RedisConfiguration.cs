using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Configuration
{
    public class RedisConfiguration
    {
        public bool Enable {  get; set; }
        public string ConnectionString {  get; set; }
    }
}
