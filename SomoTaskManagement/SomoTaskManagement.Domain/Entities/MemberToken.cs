using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    public class MemberToken:BaseEntity
    {
        public int MemberId { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiredDateAccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string CodeRefreshToken { get; set; }
        public DateTime ExpiredDateRefreshToken { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Member Member { get; set; }
    }
}
