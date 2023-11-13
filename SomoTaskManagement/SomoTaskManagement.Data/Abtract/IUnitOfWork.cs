using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Data.Abtract
{
    public interface IUnitOfWork
    {
        Repository<Member> RepositoryMember { get; }
        Repository<FarmTask> RepositoryFarmTask { get; }
        Repository<Area> RepositoryArea { get; }
        Repository<Employee> RepositoryEmployee { get; }
        Repository<EvidenceImage> RepositoryEvidenceImage { get; }
        Repository<Farm> RepositoryFarm { get; }
        Repository<HabitantType> RepositoryHabitantType { get; }
        Repository<Material> RepositoryMaterial { get; }
        Repository<Field> RepositoryField { get; }
        Repository<Role> RepositoryRole { get; }
        Repository<TaskEvidence> RepositoryTaskEvidence { get; }
        Repository<TaskType> RepositoryTaskTaskType { get; }
        Repository<Zone> RepositoryZone { get; }
        Repository<ZoneType> RepositoryZoneType { get; }
        Repository<MemberToken> RepositoryUserToken { get; }
        Repository<Plant> RepositoryPlant { get; }
        Repository<LiveStock> RepositoryLiveStock { get; }
        Repository<Employee_TaskType> RepositoryEmployee_TaskType { get; }
        Repository<Employee_Task> RepositoryEmployee_Task { get; }
        Repository<Material_Task> RepositoryMaterial_Task { get; }
        Repository<HubConnection> RepositoryHubConnection { get; }
        Repository<Notification> RepositoryNotifycation { get; }
        Repository<Notification_Member> RepositoryNotifycation_Member { get; }

        Task BeginTransactionAsync();
        void CommitTransaction();
        void RollbackTransaction();
    }
}
