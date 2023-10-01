using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Interface
{
    public interface IMemberService
    {
        Task<Member> CheckLogin(string userName, string password);
        Task CreateMember(Member member);
        Task<Member> FindById(int memberId);
        Task<Member> FindByUserName(string userName);
        Task<GetMemberModel> GetById(int memberId);
        Task<Member> GetByUser(string userName, string password);
        Task<IEnumerable<MemberModel>> List();
        Task<IEnumerable<Member>> ListSupervisor(int id);
        Task UdateMember(Member member);
    }
}
