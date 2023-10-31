using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly SomoTaskManagemnetContext _context;
        private bool disposedValue;
        public UnitOfWork(SomoTaskManagemnetContext context)
        {
            _context = context;
        }

        Repository<Member> repositoryMemeber;
        Repository<FarmTask> repositoryFarmTask;
        Repository<Area> repositoryArea;
        Repository<Employee> repositoryEmployee;
        Repository<EvidenceImage> repositoryEvidenceImage;
        Repository<Farm> repositoryFarm;
        Repository<Field> repositoryField;
        Repository<Plant> repositoryPlant;
        Repository<LiveStock> repositoryLiveStock;
        Repository<HabitantType> repositoryHabitantType;
        Repository<Material> repositoryMaterial;
        Repository<Other> repositoryOther;
        Repository<Role> repositoryRole;
        Repository<TaskEvidence> repositoryTaskEvidence;
        Repository<TaskType> repositoryTaskTaskType;
        Repository<Zone> repositoryZone;
        Repository<ZoneType> repositoryZoneType;
        Repository<MemberToken> repositoryUserToken;
        Repository<Employee_TaskType> repositoryEmployee_TaskType;
        Repository<Employee_Task> repositoryEmployee_Task;
        Repository<Material_Task> repositoryMaterial_Task;
        Repository<HubConnection> repositoryHubConnection;
        Repository<Notification> repositoryNotifycation;
        Repository<Notification_Member> repositoryNotifycation_Member;


        public Repository<Member> RepositoryMember { get { return repositoryMemeber ??= new Repository<Member>(_context);} }
        public Repository<Notification_Member> RepositoryNotifycation_Member { get { return repositoryNotifycation_Member ??= new Repository<Notification_Member>(_context);} }
        public Repository<Notification> RepositoryNotifycation { get { return repositoryNotifycation ??= new Repository<Notification>(_context);} }
        public Repository<HubConnection> RepositoryHubConnection { get { return repositoryHubConnection ??= new Repository<HubConnection>(_context);} }
        public Repository<Employee_Task> RepositoryEmployee_Task { get { return repositoryEmployee_Task ??= new Repository<Employee_Task>(_context);} }
        public Repository<Material_Task> RepositoryMaterial_Task { get { return repositoryMaterial_Task ??= new Repository<Material_Task>(_context);} }
        public Repository<Employee_TaskType> RepositoryEmployee_TaskType { get { return repositoryEmployee_TaskType ??= new Repository<Employee_TaskType>(_context);} }
        public Repository<FarmTask> RepositoryFarmTask { get { return repositoryFarmTask ??= new Repository<FarmTask>(_context);} }
        public Repository<Area> RepositoryArea { get { return repositoryArea ??= new Repository<Area>(_context);} }
        public Repository<Employee> RepositoryEmployee { get { return repositoryEmployee ??= new Repository<Employee>(_context);} }
        public Repository<EvidenceImage> RepositoryEvidenceImage { get { return repositoryEvidenceImage ??= new Repository<EvidenceImage>(_context);} }
        public Repository<Farm> RepositoryFarm { get { return repositoryFarm ??= new Repository<Farm>(_context);} }
        public Repository<Plant> RepositoryPlant { get { return repositoryPlant ??= new Repository<Plant>(_context);} }
        public Repository<LiveStock> RepositoryLiveStock { get { return repositoryLiveStock ??= new Repository<LiveStock>(_context);} }
        public Repository<HabitantType> RepositoryHabitantType { get { return repositoryHabitantType ??= new Repository<HabitantType>(_context);} }
        public Repository<Material> RepositoryMaterial { get { return repositoryMaterial ??= new Repository<Material>(_context);} }
        public Repository<Field> RepositoryField { get { return repositoryField ??= new Repository<Field>(_context);} }
        public Repository<Other> RepositoryOther { get { return repositoryOther ??= new Repository<Other>(_context);} }
        public Repository<Role> RepositoryRole { get { return repositoryRole ??= new Repository<Role>(_context);} }
        public Repository<TaskEvidence> RepositoryTaskEvidence { get { return repositoryTaskEvidence ??= new Repository<TaskEvidence>(_context);} }
        public Repository<TaskType> RepositoryTaskTaskType { get { return repositoryTaskTaskType ??= new Repository<TaskType>(_context);} }
        public Repository<Zone> RepositoryZone { get { return repositoryZone ??= new Repository<Zone>(_context);} }
        public Repository<ZoneType> RepositoryZoneType { get { return repositoryZoneType ??= new Repository<ZoneType>(_context);} }
        public Repository<MemberToken> RepositoryUserToken { get { return repositoryUserToken ??= new Repository<MemberToken>(_context);} }

        public async Task BeginTransactionAsync()
        {
            if (_context.Database.CurrentTransaction == null)
            {
                await _context.Database.BeginTransactionAsync();
            }
        }

        public void CommitTransaction()
        {
            if (_context.Database.CurrentTransaction != null)
            {
                _context.Database.CurrentTransaction.Commit();
            }
        }

        public void RollbackTransaction()
        {
            if (_context.Database.CurrentTransaction != null)
            {
                _context.Database.CurrentTransaction.Rollback();
            }
        }

        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
