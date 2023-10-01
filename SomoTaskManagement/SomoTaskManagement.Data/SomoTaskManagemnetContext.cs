using Microsoft.EntityFrameworkCore;
using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Data
{
    public class SomoTaskManagemnetContext : DbContext
    {
        public SomoTaskManagemnetContext()
        {

        }
        public SomoTaskManagemnetContext(DbContextOptions<SomoTaskManagemnetContext> options) : base(options)
        {

        }

        public virtual DbSet<Area> Areas { get; set; } = null!;
        public virtual DbSet<Employee> Employee { get; set; } = null!;
        public virtual DbSet<Employee_Task> Employee_Task { get; set; } = null!;
        public virtual DbSet<Employee_TaskType> Employee_TaskType { get; set; } = null!;
        public virtual DbSet<EvidenceImage> EvidenceImage { get; set; } = null!;
        public virtual DbSet<Farm> Farm { get; set; } = null!;
        public virtual DbSet<Field> Field { get; set; } = null!;
        public virtual DbSet<Plant> Plant { get; set; } = null!;
        public virtual DbSet<HabitantType> HabitantType { get; set; } = null!;
        public virtual DbSet<Material> Material { get; set; } = null!;
        public virtual DbSet<Material_Task> Material_Task { get; set; } = null!;
        public virtual DbSet<Member> Member { get; set; } = null!;
        public virtual DbSet<Other> Other { get; set; } = null!;
        public virtual DbSet<Role> Role { get; set; } = null!;
        public virtual DbSet<FarmTask> FarmTask { get; set; } = null!;
        public virtual DbSet<TaskEvidence> TaskEvidence { get; set; } = null!;
        public virtual DbSet<TaskType> TaskType { get; set; } = null!;
        public virtual DbSet<Zone> Zone { get; set; } = null!;
        public virtual DbSet<ZoneType> ZoneType { get; set; } = null!;
        public virtual DbSet<LiveStock> LiveStock { get; set; } = null!;
        public virtual DbSet<HubConnection> HubConnection { get; set; } = null!;
        public virtual DbSet<Notification> Notification { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=tcp:somodb.database.windows.net,1433;Initial Catalog=SomoTaskManagement;Persist Security Info=False;User ID=hoangvu;Password=somo123@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False");
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.Message)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.MessageType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NotificationDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<HubConnection>(entity =>
            {
                entity.ToTable("HubConnection");

                entity.Property(e => e.ConnectionId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MemberId)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                //entity.HasOne(e => e.Member)
                //    .WithOne()
                //    .HasForeignKey<HubConnection>(e => e.MemberId);
            });

            //Farm
            modelBuilder.Entity<Farm>(entity =>
            {
                entity.ToTable("Farm");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.FarmArea).IsRequired();
                entity.Property(e => e.Address).IsRequired();

            });

            //Area
            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("Area");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.FArea).IsRequired();

                entity.HasOne(d => d.Farm).WithMany(p => p.Areas).HasForeignKey(d => d.FarmId).HasConstraintName("FK_Farm_Area").OnDelete(DeleteBehavior.NoAction);

            });

            //Zone
            modelBuilder.Entity<Zone>(entity =>
            {
                entity.ToTable("Zone");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.FarmArea).IsRequired();

                entity.HasOne(d => d.Area).WithMany(p => p.Zones).HasForeignKey(d => d.AreaId).HasConstraintName("FK_Area_Zone").OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.ZoneType).WithMany(p => p.Zones).HasForeignKey(d => d.ZoneTypeId).HasConstraintName("FK_ZoneType_Zone").OnDelete(DeleteBehavior.NoAction);
            });


            //ZoneType
            modelBuilder.Entity<ZoneType>(entity =>
            {
                entity.ToTable("ZoneType");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
            });


            //Field
            modelBuilder.Entity<Field>(entity =>
            {
                entity.ToTable("Field");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Area).IsRequired();

                entity.HasOne(d => d.Zone).WithMany(p => p.Fields).HasForeignKey(d => d.ZoneId).HasConstraintName("FK_Zone_Field").OnDelete(DeleteBehavior.NoAction);
            });


            //FarmTask  
            modelBuilder.Entity<FarmTask>(entity =>
            {
                entity.ToTable("FarmTask");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Priority).IsRequired();
                entity.Property(e => e.ReceiverId).IsRequired();
                entity.Property(e => e.Repeat).IsRequired();
                entity.Property(e => e.Iterations).IsRequired();
                entity.Property(e => e.CreateDate).IsRequired();
                entity.Property(e => e.Remind).IsRequired();

                entity.HasOne(d => d.Plant).WithMany(p => p.Tasks).HasForeignKey(d => d.PlantId).HasConstraintName("FK_Plant_FarmTask").OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.LiveStrock).WithMany(p => p.Tasks).HasForeignKey(d => d.LiveStockId).HasConstraintName("FK_LiveStock_FarmTask").OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.Field).WithMany(p => p.Tasks).HasForeignKey(d => d.FieldId).HasConstraintName("FK_Field_Task").OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.Other).WithMany(p => p.Tasks).HasForeignKey(d => d.OtherId).HasConstraintName("FK_Other_Task").OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.TaskType).WithMany(p => p.Tasks).HasForeignKey(d => d.TaskTypeId).HasConstraintName("FK_TaskType_Task").OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.Member).WithMany(p => p.Tasks).HasForeignKey(d => d.MemberId).HasConstraintName("FK_Member_Task").OnDelete(DeleteBehavior.NoAction);
            });

            //Plant
            modelBuilder.Entity<Plant>(entity =>
            {
                entity.ToTable("Plant");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.ExternalId).IsRequired();
                entity.Property(e => e.CreateDate).IsRequired();
                entity.Property(e => e.Height).IsRequired();


                entity.HasOne(d => d.HabitantType).WithMany(p => p.Plants).HasForeignKey(d => d.HabitantTypeId).HasConstraintName("FK_HabitantType_Plant");
                entity.HasOne(d => d.Field).WithMany(p => p.Plants).HasForeignKey(d => d.FieldId).HasConstraintName("FK_Field_Plant");
            });

            //LiveStock
            modelBuilder.Entity<LiveStock>(entity =>
            {
                entity.ToTable("LiveStock");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.ExternalId).IsRequired();
                entity.Property(e => e.CreateDate).IsRequired();
                entity.Property(e => e.Weight).IsRequired();
                entity.Property(e => e.DateOfBirth).IsRequired();
                entity.Property(e => e.Gender).IsRequired();


                entity.HasOne(d => d.HabitantType).WithMany(p => p.LiveStocks).HasForeignKey(d => d.HabitantTypeId).HasConstraintName("FK_HabitantType_LiveStock");
                entity.HasOne(d => d.Field).WithMany(p => p.LiveStocks).HasForeignKey(d => d.FieldId).HasConstraintName("FK_Field_LiveStock");
            });

            //HabitantType
            modelBuilder.Entity<HabitantType>(entity =>
            {
                entity.ToTable("HabitantType");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
            });

            //Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.PhoneNumber).IsRequired();
                entity.Property(e => e.Address).IsRequired();

                entity.HasOne(d => d.Farm).WithMany(p => p.Employees).HasForeignKey(d => d.FarmId).HasConstraintName("FK_Farm_Employee");
            });

            //Employee_Task
            modelBuilder.Entity<Employee_Task>(entity =>
            {
                entity.ToTable("Employee_Task");
                entity.HasKey(et => new { et.EmployeeId, et.TaskId });
                entity.HasOne(d => d.Employee).WithMany(p => p.Employee_Tasks).HasForeignKey(d => d.EmployeeId).HasConstraintName("FK_Employee_Employee_Task");
                entity.HasOne(d => d.Task).WithMany(p => p.Employee_Tasks).HasForeignKey(d => d.TaskId).HasConstraintName("FK_Task_Employee_Task");
            });

            //Employee_TaskType
            modelBuilder.Entity<Employee_TaskType>(entity =>
            {
                entity.ToTable("Employee_TaskType");
                entity.HasKey(et => new { et.EmployeeId, et.TaskTypeId });
                entity.HasOne(d => d.Employee).WithMany(p => p.Employee_TaskTypes).HasForeignKey(d => d.EmployeeId).HasConstraintName("FK_Employee_Employee_TaskType");
                entity.HasOne(d => d.TaskType).WithMany(p => p.Employee_TaskTypes).HasForeignKey(d => d.TaskTypeId).HasConstraintName("FK_Task_Employee_TaskType");
            });


            //TaskType
            modelBuilder.Entity<TaskType>(entity =>
            {
                entity.ToTable("TaskType");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.IsDelete).IsRequired();
            });

            //Other
            modelBuilder.Entity<Other>(entity =>
            {
                entity.ToTable("Other");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
            });

            //Material_Task
            modelBuilder.Entity<Material_Task>(entity =>
            {
                entity.ToTable("Material_Task");
                entity.HasKey(et => new { et.MaterialId, et.TaskId });
                entity.HasOne(d => d.Task).WithMany(p => p.Material_Tasks).HasForeignKey(d => d.TaskId).HasConstraintName("FK_Task_MaterialTask");
                entity.HasOne(d => d.Material).WithMany(p => p.MaterialTasks).HasForeignKey(d => d.MaterialId).HasConstraintName("FK_Material_MaterialTask");
            });

            //Material
            modelBuilder.Entity<Material>(entity =>
            {
                entity.ToTable("Material");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
            });

            //Member
            modelBuilder.Entity<Member>(entity =>
            {
                entity.ToTable("Member");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.UserName).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.PhoneNumber).IsRequired();
                entity.Property(e => e.Birthday).IsRequired();
                entity.Property(e => e.Address).IsRequired();

                entity.HasOne(d => d.Role).WithMany(p => p.Members).HasForeignKey(d => d.RoleId).HasConstraintName("FK_Role_Member");
                entity.HasOne(d => d.Farm).WithMany(p => p.Members).HasForeignKey(d => d.FarmId).HasConstraintName("FK_Farms_Member");
                
                //entity.HasOne(d => d.Notification).WithMany(p => p.Members).HasForeignKey(d => d.NotificcationId).HasConstraintName("FK_Notification_Member");
                //entity.HasOne(e => e.HubConnection)
                //   .WithOne()
                //   .HasForeignKey<Member>(e => e.HubConnectionId);
            });

            //MemberToken
            modelBuilder.Entity<MemberToken>(entity =>
            {
                entity.ToTable("MemberToken");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.AccessToken).IsRequired();
                entity.Property(e => e.ExpiredDateAccessToken).IsRequired();
                entity.Property(e => e.RefreshToken).IsRequired();
                entity.Property(e => e.CodeRefreshToken).IsRequired();
                entity.Property(e => e.ExpiredDateRefreshToken).IsRequired();
                entity.Property(e => e.CreateDate).IsRequired();

                entity.HasOne(d => d.Member).WithMany(p => p.MemberTokens).HasForeignKey(d => d.MemberId).HasConstraintName("FK_Member_MemberToken");
            });

            //Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
            });

            //Evidence
            modelBuilder.Entity<TaskEvidence>(entity =>
            {
                entity.ToTable("TaskEvidence");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.SubmitDate).IsRequired();
                entity.Property(e => e.Description).IsRequired();

                entity.HasOne(d => d.Task).WithMany(p => p.TaskEvidences).HasForeignKey(d => d.TaskId).HasConstraintName("FK_Task_TaskEvidence");
            });

            //EvidenceImage
            modelBuilder.Entity<EvidenceImage>(entity =>
            {
                entity.ToTable("EvidenceImage");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.ImageUrl).IsRequired();
            });
        }
    }
}
