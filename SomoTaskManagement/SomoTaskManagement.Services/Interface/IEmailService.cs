using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IEmailService
    {
        Task SendPasswordResetEmail(int memberId);
    }
}
