using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.HubConnection;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{   
    public class HubConnectionService: IHubConnection
    {
        private readonly IUnitOfWork _unitOfWork;

        public HubConnectionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<string>> GetTokenByMemberId(int id)
        {
            var hubConnections = await _unitOfWork.RepositoryHubConnection.GetData(h => h.MemberId == id);

            if (hubConnections == null)
            {
                throw new Exception("Không tìm thấy kết nối");
            }

            var connectionIds = hubConnections.Select(h => h.ConnectionId).ToList();
            return connectionIds;
        }


        public async Task<List<int>> GetManagerId()
        {
            var manager = await _unitOfWork.RepositoryMember.GetData(m => m.RoleId == 1);
            var managerId = manager.Select(m=>m.Id).ToList();
            //var managerOfToken = await _unitOfWork.RepositoryHubConnection.GetData(m => managerId.Contains(m.MemberId));
            return managerId;
        }
        
        public async Task Create (HubConnection hubConnection)
        {
            await _unitOfWork.RepositoryHubConnection.Add(hubConnection);
            await _unitOfWork.RepositoryHubConnection.Commit(); 
        }

        public async Task Delete(HubConnectionDeleteModel token)
        {

            var hubDelete = await _unitOfWork.RepositoryHubConnection.GetSingleByCondition(h=>h.ConnectionId == token.Token);
            if(hubDelete == null)
            {
                throw new Exception("Không tìm thấy connection");
            }
            _unitOfWork.RepositoryHubConnection.Delete(hubDelete);
            await _unitOfWork.RepositoryHubConnection.Commit();
        }
    }
}
