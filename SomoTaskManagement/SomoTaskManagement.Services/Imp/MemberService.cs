using AutoMapper;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Member;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SomoTaskManagement.Services.Imp
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SomoTaskManagemnetContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public MemberService(IUnitOfWork unitOfWork, SomoTaskManagemnetContext context, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Member> CheckLogin(string userName, string password)
        {
            var member = await _context.Member.SingleOrDefaultAsync(m => m.UserName == userName && m.Status == 1);

            if (member != null && BCrypt.Net.BCrypt.Verify(password, member.Password))
            {
                return member; 
            }

            return null;
        }

        public async Task HashPassword()
        {
            var members = await _unitOfWork.RepositoryMember.GetData(null);

            foreach (var member in members)
            {
                string currentPassword = member.Password;

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(currentPassword);

                member.Password = hashedPassword;

                await _unitOfWork.RepositoryMember.Commit();
            }

        }

        public async Task<Member> GetByUser(string userName, string password)
        {
            return await _unitOfWork.RepositoryMember.GetSingleByCondition(m => m.UserName == userName && m.Password == password);
        }

        public async Task CreateMember(MemberCreateUpdateModel member)
        {
            var exitsMember = await _unitOfWork.RepositoryMember.GetSingleByCondition(m=>m.Email == member.Email);
            if (exitsMember == null)
            {
                var memberNew = new Member
                {
                    Name = member.Name,
                    Status = 1,
                    Email = member.Email,
                    UserName = member.UserName,
                    Code = member.Code,
                    PhoneNumber = member.PhoneNumber,
                    Birthday = member.Birthday,
                    Address = member.Address,
                    RoleId = member.RoleId,
                    FarmId = member.FarmId,
                };
                string currentPassword = member.Password;

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(currentPassword);
                memberNew.Password = hashedPassword;
                var urlImage = await UploadImageToFirebaseAsync(memberNew, member.ImageFile);
                memberNew.Avatar = urlImage;

                var existingMember = await _unitOfWork.RepositoryMember.GetSingleByCondition(m => m.Code == member.Code || m.Email == member.Email);
                if (existingMember != null)
                {
                    throw new Exception("Mã người dùng, email không được trùng");
                }
                await _unitOfWork.RepositoryMember.Add(memberNew);
                await _unitOfWork.RepositoryMember.Commit();
            }
            else
            {
                throw new Exception("Email không thể trùng");
            }
            
        }

        private async Task<string> UploadImageToFirebaseAsync(Member member, IFormFile imageFile)
        {
            var options = new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(_configuration["Firebase:apiKey"])
            };

            string uniqueIdentifier = Guid.NewGuid().ToString();
            string fileName = $"{member.Id}_{uniqueIdentifier}";
            string fileExtension = Path.GetExtension(imageFile.FileName);

            var firebaseStorage = new FirebaseStorage(_configuration["Firebase:Bucket"], options)
               .Child("images")
               .Child("MemberAvatar")
               .Child(fileName + fileExtension);

            using (var stream = imageFile.OpenReadStream())
            {
                await firebaseStorage.PutAsync(stream);
            }

            var imageUrl = await firebaseStorage.GetDownloadUrlAsync();

            return imageUrl;
        }

        public async Task UpdatePassword(int memberId,UpdatePasswordModel passwordModel)
        {
            var member = await _unitOfWork.RepositoryMember.GetById(memberId) ?? throw new Exception("Không tìm thấy nhân viên");

            if (member != null && BCrypt.Net.BCrypt.Verify(passwordModel.OldPassword, member.Password))
            {
                if (!passwordModel.Password.Equals(passwordModel.ConfirmPassword))
                {
                    throw new Exception("Xác nhận lại mật khẩu");
                }

                string currentPassword = passwordModel.Password;

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(currentPassword);
                member.Password = hashedPassword;

                await _unitOfWork.RepositoryMember.Commit();
            }
            else
            {
                throw new Exception("Sai mật khẩu cũ");
            }
        }


        public async Task UdateMember(int id, MemberUpdateModel member)
        {
            var memberUpdate = await _unitOfWork.RepositoryMember.GetById(id);
            if(memberUpdate == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }
            memberUpdate.Id= id;
            memberUpdate.Name = member.Name;
            memberUpdate.Email = member.Email;
            memberUpdate.Code = member.Code;
            memberUpdate.PhoneNumber = member.PhoneNumber;
            memberUpdate.Birthday = member.Birthday;
            memberUpdate.Address = member.Address;

            var urlImage = member.ImageFile != null
                   ? await UploadImageToFirebaseAsync(memberUpdate, member.ImageFile)
                   : memberUpdate.Avatar;

            memberUpdate.Avatar = urlImage;

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
                FarmName = farm.Name,
                Name = member.Name,
                Status = member.Status,
                Avatar = member.Avatar,
                Code =member.Code
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

        public async Task<IEnumerable<MemberModel>> ListSupervisor(int id)
        {
            var includes = new Expression<Func<Member, object>>[]
            {
                t => t.Role,
                t => t.Farm,
            };
            var member = await _unitOfWork.RepositoryMember.GetData(expression: m => m.FarmId == id && m.RoleId == 3, includes: includes);
            if(member == null)
            {
                throw new Exception("Không tìm thấy người giám sát");
            }
            return _mapper.Map<IEnumerable<Member>, IEnumerable<MemberModel>>(member);
        }

        public async Task<IEnumerable<MemberModel>> ListMemberByFarm(int farmId)
        {
            var includes = new Expression<Func<Member, object>>[]
            {
                t => t.Role,
                t => t.Farm,
            };
            var member = await _unitOfWork.RepositoryMember.GetData(expression: m => m.FarmId == farmId , includes: includes);
            if (member == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }
            return _mapper.Map<IEnumerable<Member>, IEnumerable<MemberModel>>(member);
        }

        public async Task<IEnumerable<MemberActiveModel>> ListSupervisorActive(int id)
        {
            var includes = new Expression<Func<Member, object>>[]
            {
                t => t.Role,
                t => t.Farm,
            };
            var member = await _unitOfWork.RepositoryMember.GetData(expression: m => m.FarmId == id && m.RoleId == 3 && m.Status == 1, includes: includes);
            if (member == null)
            {
                throw new Exception("Không tìm thấy người giám sát");
            }
            return _mapper.Map<IEnumerable<Member>, IEnumerable<MemberActiveModel>>(member);
        }

        public async Task ChangStatusMember(int memberId)
        {
            var member = await _unitOfWork.RepositoryMember.GetById(memberId);
            if(member == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }
            member.Status = member.Status == 1 ? 0 : 1;
            await _unitOfWork.RepositoryMember.Commit();
        }

        public async Task DeleteMember(int memberId)
        {
            var member = await _unitOfWork.RepositoryMember.GetById(memberId);
            if (member == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }
            _unitOfWork.RepositoryMember.Delete(m => m.Id == memberId);
            await _unitOfWork.RepositoryMember.Commit();
        }

        


    }
}
