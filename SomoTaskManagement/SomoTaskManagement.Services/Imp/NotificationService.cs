using AutoMapper;
using SomoTaskManagement.Data;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.Notification;
using SomoTaskManagement.Domain.Model.Task;
using SomoTaskManagement.Domain.Model.TaskEvidence;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationModel>> ListByMemberIsNew(int id)
        {
            var member = await _unitOfWork.RepositoryMember.GetById(id);
            if (member == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }

            var notificationMembers = await _unitOfWork.RepositoryNotifycation_Member.GetData(n => n.MemberId == id);
            var notificationMemberIds = notificationMembers.Select(n => n.NotificationId).ToList();

            var notifications = await _unitOfWork.RepositoryNotifycation.GetData(n => notificationMemberIds.Contains(n.Id) && n.IsNew == true);


            return _mapper.Map<IEnumerable<Notification>, IEnumerable<NotificationModel>>(notifications);
        }

        public async Task<NotificationPageResult> ListByMember(int id, int pageIndex, int pageSize)
        {
            var member = await _unitOfWork.RepositoryMember.GetById(id);
            if (member == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }

            var notificationMembers = await _unitOfWork.RepositoryNotifycation_Member.GetData(n => n.MemberId == id);
            var notificationMemberIds = notificationMembers.Select(n => n.NotificationId).ToList();

            var notifications = await _unitOfWork.RepositoryNotifycation.GetData(n => notificationMemberIds.Contains(n.Id));
            int skipCount = (pageIndex - 1) * pageSize;
            var totalTaskCount = notifications.Count();

            var totalPages = (int)Math.Ceiling((double)totalTaskCount / pageSize);
            notifications = notifications.OrderByDescending(t => t.Id)
                     .Skip(skipCount)
                     .Take(pageSize)
                     .ToList();

            return new NotificationPageResult
            {
                Notifications = _mapper.Map<IEnumerable<Notification>, IEnumerable<NotificationModel>>(notifications),
                TotalPages = totalPages
            };
        }
        public async Task<NotificationPageResult> ListByMemberIsRead(int id, int pageIndex, int pageSize)
        {
            var member = await _unitOfWork.RepositoryMember.GetById(id);
            if (member == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }
            int skipCount = (pageIndex - 1) * pageSize;
            var notificationMembers = await _unitOfWork.RepositoryNotifycation_Member.GetData(n => n.MemberId == id);
            var notificationMemberIds = notificationMembers.Select(n => n.NotificationId).ToList();

            var notifications = await _unitOfWork.RepositoryNotifycation.GetData(n => notificationMemberIds.Contains(n.Id) && n.IsRead == false);
            var totalTaskCount = notifications.Count();

            var totalPages = (int)Math.Ceiling((double)totalTaskCount / pageSize);
            notifications = notifications.OrderByDescending(t => t.Id)
                     .Skip(skipCount)
                     .Take(pageSize)
                     .ToList();

            return new NotificationPageResult
            {
                Notifications = _mapper.Map<IEnumerable<Notification>, IEnumerable<NotificationModel>>(notifications),
                TotalPages = totalPages
            };
        }

       
        public async Task<IEnumerable<Notification>> ListByMemberRead(int id)
        {
            var member = await _unitOfWork.RepositoryMember.GetById(id);
            if (member == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }
            
            var notificationMembers = await _unitOfWork.RepositoryNotifycation_Member.GetData(n => n.MemberId == id);
            var notificationMemberIds = notificationMembers.Select(n => n.NotificationId).ToList();

            var notifications = await _unitOfWork.RepositoryNotifycation.GetData(n => notificationMemberIds.Contains(n.Id) && n.IsRead == true);

            

            return notifications;
        }
        public async Task DeleteNotificationByMember(int id)
        {
            var member = await _unitOfWork.RepositoryMember.GetById(id);
            if (member == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }

             _unitOfWork.RepositoryNotifycation_Member.Delete(n=>n.MemberId == id);
            await _unitOfWork.RepositoryNotifycation_Member.Commit();
        }

        


        public async Task<int> GetCount(int id)
        {
            var member = await _unitOfWork.RepositoryMember.GetById(id);
            if (member == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }

            var notificationMembers = await _unitOfWork.RepositoryNotifycation_Member.GetData(n => n.MemberId == id);
            var notificationMemberIds = notificationMembers.Select(n => n.NotificationId).ToList();

            var notifications = await _unitOfWork.RepositoryNotifycation.GetData(n => notificationMemberIds.Contains(n.Id) && n.IsNew == true);

            int notificationCount = notifications.Count();
            return notificationCount;
        }

        public async Task UpdateIsNew(int id)
        {
            var member = await _unitOfWork.RepositoryMember.GetById(id);
            if (member == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }

            var notificationMembers = await _unitOfWork.RepositoryNotifycation_Member.GetData(n => n.MemberId == id);
            var notificationMemberIds = notificationMembers.Select(n => n.NotificationId).ToList();

            var notifications = await _unitOfWork.RepositoryNotifycation.GetData(n => notificationMemberIds.Contains(n.Id) && n.IsNew == true);
            foreach(var notification in notifications)
            {
                notification.IsNew = false;
                await _unitOfWork.RepositoryNotifycation.Commit();
            }
        }

        public async Task UpdateIsRead(int id)
        {
            var notification = await _unitOfWork.RepositoryNotifycation.GetById(id);
            if (notification == null)
            {
                throw new Exception("Không tìm thấy thông báo");
            }
            notification.IsRead = true;
            await _unitOfWork.RepositoryNotifycation.Commit();
        }

        public async Task DeleteNotificationById(int notificaitonId, int memberId)
        {
            var member = await _unitOfWork.RepositoryNotifycation_Member.GetSingleByCondition(n => n.NotificationId == notificaitonId && n.MemberId == memberId);
            if (member == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }

            _unitOfWork.RepositoryNotifycation_Member.Delete(n => n.NotificationId == notificaitonId && n.MemberId == memberId);
            await _unitOfWork.RepositoryNotifycation_Member.Commit();
        }


        public async Task UpdateAllIsRead(int id)
        {
            var member = await _unitOfWork.RepositoryMember.GetById(id);
            if (member == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }

            var notificationMembers = await _unitOfWork.RepositoryNotifycation_Member.GetData(n => n.MemberId == id);
            var notificationMemberIds = notificationMembers.Select(n => n.NotificationId).ToList();

            var notifications = await _unitOfWork.RepositoryNotifycation.GetData(n => notificationMemberIds.Contains(n.Id));
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                await _unitOfWork.RepositoryNotifycation.Commit();
            }
        }

        public async Task<List<Notification>> List()
        {
            //var notifications = _context.Notification.ToList();
            //foreach (var notification in notifications)
            //{
            //    await _context.Entry(notification).ReloadAsync();
            //}
            return null;
        }
    }
}
