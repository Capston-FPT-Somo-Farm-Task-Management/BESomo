using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.HubConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IHubConnection
    {
        Task Create(HubConnection hubConnection);
        Task Delete(HubConnectionDeleteModel token);
        Task<List<int>> GetManagerId();
        Task<List<string>> GetTokenByMemberId(int id);
        //Task<List<string>> GetTokenByMemberIds(IEnumerable<int> managerIds);
    }
}
