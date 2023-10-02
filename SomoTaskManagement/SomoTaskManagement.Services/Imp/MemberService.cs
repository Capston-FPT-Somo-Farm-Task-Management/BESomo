using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SomoTaskManagemnetContext _context;
        private readonly IMapper _mapper;

        public MemberService(IUnitOfWork unitOfWork, SomoTaskManagemnetContext context,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<Member> CheckLogin(string userName, string password)
        {
            return await _context.Member.SingleOrDefaultAsync(m => m.UserName == userName && m.Password == password);
        }

        public async Task<Member> GetByUser(string userName, string password)
        {
            return await _unitOfWork.RepositoryMember.GetSingleByCondition(m => m.UserName == userName && m.Password == password);
        }

        public async Task CreateMember(Member member)
        {
            await _unitOfWork.RepositoryMember.Add(member);
        }

        public async Task UdateMember(Member member)
        {
            _unitOfWork.RepositoryMember.Update(member);
            await _unitOfWork.RepositoryMember.Commit();
        }

        public async Task<Member> FindByUserName(string userName)
        {
            return await _unitOfWork.RepositoryMember.GetSingleByCondition(m => m.UserName == userName);
        }
        public async Task<Member> FindById(int memberId)
        {
            return await _unitOfWork.RepositoryMember.GetSingleByCondition(m => m.Id == memberId);
        }
        public async Task<GetMemberModel> GetById(int memberId)
        {
            var member = await _unitOfWork.RepositoryMember.GetSingleByCondition(m => m.Id == memberId);

            var role = await _unitOfWork.RepositoryRole.GetSingleByCondition(r => r.Id == member.RoleId);
            var farm = await _unitOfWork.RepositoryFarm.GetSingleByCondition(r => r.Id == member.FarmId);

            var memberModel = new GetMemberModel
            {
                Id = member.Id,
                Email = member.Email,
                UserName = member.UserName,
                Password = member.Password,
                PhoneNumber = member.PhoneNumber,
                Birthday = member.Birthday,
                Address = member.Address,
                RoleName = member.Role.Name,
                FarmId = member.FarmId,
                Name = member.Name,
                Status = member.Status,
            };

            return memberModel;
        }

        public async Task<IEnumerable<MemberModel>> List()
        {
            var includes = new Expression<Func<Member, object>>[]
            {
                t => t.Role,
                t => t.Farm,
            };

            var member = await _unitOfWork.RepositoryMember.GetData(expression: null, includes: includes);

            return _mapper.Map<IEnumerable<Member>, IEnumerable<MemberModel>>(member);
        }

        public async Task<IEnumerable<MemberModel>> ListSupervisor (int id)
        {
            var includes = new Expression<Func<Member, object>>[]
            {
                t => t.Role,
                t => t.Farm,
            };
            var member = await _unitOfWork.RepositoryMember.GetData(expression: m=>m.FarmId == id && m.RoleId == 3, includes: includes);
            return _mapper.Map<IEnumerable<Member>, IEnumerable<MemberModel>>(member);
        }
    }
}
