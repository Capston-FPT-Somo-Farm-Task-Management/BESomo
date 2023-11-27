using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IMemberService
    {
        Task ChangStatusMember(int memberId);
        Task<Member> CheckLogin(string userName, string password);
        Task CreateMember(MemberCreateUpdateModel member);
        Task DeleteMember(int memberId);
        Task<Member> FindById(int memberId);
        Task<Member> FindByUserName(string userName);
        Task<GetMemberModel> GetById(int memberId);
        Task<Member> GetByUser(string userName, string password);
        Task HashPassword();
        Task<IEnumerable<MemberModel>> List();
        Task<IEnumerable<MemberModel>> ListMemberByFarm(int farmId);
        Task<IEnumerable<MemberModel>> ListSupervisor(int id);
        Task<IEnumerable<MemberActiveModel>> ListSupervisorActive(int id);
        Task UdateMember(int id, MemberUpdateModel member);
    }
}
