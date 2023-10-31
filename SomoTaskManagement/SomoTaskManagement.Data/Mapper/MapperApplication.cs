using AutoMapper;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model.Area;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.Field;
using SomoTaskManagement.Domain.Model.HabitantType;
using SomoTaskManagement.Domain.Model.Livestock;
using SomoTaskManagement.Domain.Model.Material;
using SomoTaskManagement.Domain.Model.Member;
using SomoTaskManagement.Domain.Model.Notification;
using SomoTaskManagement.Domain.Model.Plant;
using SomoTaskManagement.Domain.Model.SubTask;
using SomoTaskManagement.Domain.Model.Task;
using SomoTaskManagement.Domain.Model.TaskEvidence;
using SomoTaskManagement.Domain.Model.TaskType;
using SomoTaskManagement.Domain.Model.Zone;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Data.Mapper
{
    public class MapperApplication : Profile
    {
        public MapperApplication()
        {
            CreateMap<FarmTask, FarmTaskModel>()
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Member.Name))
            .ForMember(dest => dest.SupervisorName, opt => opt.MapFrom(src => src.SuppervisorId))
            .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field.Name))
            .ForMember(dest => dest.FieldStatus, opt => opt.MapFrom(src => GetHabitantDescription((HabitantTypeStatus)src.Field.Status)))
            .ForMember(dest => dest.ZoneId, opt => opt.MapFrom(src => src.Field.Zone.Id))
            .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.Field.Zone.Area.Id))
            .ForMember(dest => dest.TaskTypeName, opt => opt.MapFrom(src => src.TaskType.Name))
            .ForMember(dest => dest.StatusTaskType, opt => opt.MapFrom(src => GetTaskTypeDescription((PlantLivestockEnum)src.TaskType.Status)))
            .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.Plant.Name))
            .ForMember(dest => dest.liveStockName, opt => opt.MapFrom(src => src.LiveStrock.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetTaskStatusDescription((TaskStatusEnum)src.Status)))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => GetPriorityDescription((PriorityEnum)src.Priority)))
            .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => string.Concat($"{src.Field.Zone.Code} - ", src.Field.Zone.Name).ToString()))
             .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => string.Concat($"{src.Field.Zone.Area.Code} - ", src.Field.Zone.Area.Name).ToString()))
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.LiveStrock.ExternalId));

            CreateMap<FarmTask, TaskByEmployeeDates>()
           .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Member.Name))
           //.ForMember(dest => dest.SupervisorName, opt => opt.MapFrom(src => src.SuppervisorId))
           .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field.Name))
           .ForMember(dest => dest.FieldStatus, opt => opt.MapFrom(src => GetHabitantDescription((HabitantTypeStatus)src.Field.Status)))
           .ForMember(dest => dest.ZoneId, opt => opt.MapFrom(src => src.Field.Zone.Id))
           .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.Field.Zone.Area.Id))
           .ForMember(dest => dest.TaskTypeName, opt => opt.MapFrom(src => src.TaskType.Name))
           .ForMember(dest => dest.StatusTaskType, opt => opt.MapFrom(src => GetTaskTypeDescription((PlantLivestockEnum)src.TaskType.Status)))
           .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.Plant.Name))
           .ForMember(dest => dest.liveStockName, opt => opt.MapFrom(src => src.LiveStrock.Name))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetTaskStatusDescription((TaskStatusEnum)src.Status)))
           .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => GetPriorityDescription((PriorityEnum)src.Priority)))
           .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => string.Concat($"{src.Field.Zone.Code} - ", src.Field.Zone.Name).ToString()))
            .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => string.Concat($"{src.Field.Zone.Area.Code} - ", src.Field.Zone.Area.Name).ToString()))
           .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.LiveStrock.ExternalId));

            CreateMap<FarmTask, TaskCreateUpdateModel>().ReverseMap();

            CreateMap<Field, FieldModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetHabitantDescription((HabitantTypeStatus)src.Status)))
                .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => string.Concat($"{src.Zone.Code} - ", src.Zone.Name).ToString()))
                .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => string.Concat($"{src.Zone.Area.Code} - ", src.Zone.Area.Name).ToString()))
                .ForMember(dest => dest.NameCode, opt => opt.MapFrom(src => string.Concat($"{src.Code} - ", src.Name).ToString()))
                .ForMember(dest => dest.ZoneId, opt => opt.MapFrom(src => string.Concat(src.Zone.Id)))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => string.Concat(src.Name)))
                .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => string.Concat(src.Zone.Area.Id)));

            CreateMap<Plant, PlantModel>()
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => string.Concat($"{src.Field.Code} - ", src.Field.Name).ToString()))
                .ForMember(dest => dest.HabitantTypeName, opt => opt.MapFrom(src => src.HabitantType.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                            ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"))
                .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => string.Concat($"{src.Field.Zone.Code} - ", src.Field.Zone.Name).ToString()))
                .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => string.Concat($"{src.Field.Zone.Area.Code} - ", src.Field.Zone.Area.Name).ToString()))
                .ForMember(dest => dest.ZoneId, opt => opt.MapFrom(src => src.Field.Zone.Id))
                .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.Field.Zone.Area.Id));

            CreateMap<LiveStock, LiveStockModel>()
            .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => string.Concat($"{src.Field.Code} - ", src.Field.Name).ToString()))
            .ForMember(dest => dest.HabitantTypeName, opt => opt.MapFrom(src => src.HabitantType.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src =>
                ((GenderEnum)(src.Gender ? GenderEnum.Male : GenderEnum.Female)).GetDescription()))
             .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => string.Concat($"{src.Field.Zone.Code} - ", src.Field.Zone.Name).ToString()))
             .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => string.Concat($"{src.Field.Zone.Area.Code} - ", src.Field.Zone.Area.Name).ToString()))
             .ForMember(dest => dest.ZoneId, opt => opt.MapFrom(src => src.Field.Zone.Id))
            .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.Field.Zone.Area.Id));

            CreateMap<LiveStock, ExternalIdModel>()
             .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId))
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
             ;

            CreateMap<Employee_Task, SubtaskEffortModel>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.Name))
                .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(src => src.Employee.Code))
                .ForMember(dest => dest.EffortTime, opt => opt.MapFrom(src => src.ActualEffort));


            CreateMap<Plant, ExternalIdModel>()
             .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId))
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));


            CreateMap<Notification, NotificationModel>()
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => FormatTimeDifference(src.NotificationDateTime)));

            CreateMap<Member, MemberModel>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Farm.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                            ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"));

            CreateMap<Member, GetMemberModel>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));

            CreateMap<Member, MemberActiveModel>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Farm.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                            ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => string.Concat($"{src.Name} - ", src.Code).ToString()));

            CreateMap<Material, MaterialModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                            ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"));

            CreateMap<Zone, ZoneModel>()
               .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => string.Concat($"{src.Area.Code} - ", src.Area.Name).ToString()))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                            ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.Area.Id))
               .ForMember(dest => dest.NameCode, opt => opt.MapFrom(src => string.Concat($"{src.Code} - ", src.Name).ToString()));

            CreateMap<Area, AreaModel>()
               .ForMember(dest => dest.NameCode, opt => opt.MapFrom(src => string.Concat($"{src.Code} - ", src.Name).ToString()))
               .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Farm.Name))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                            ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"));

            CreateMap<TaskType, TaskTypeModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetTaskTypeDescription((PlantLivestockEnum)src.Status)));


            CreateMap<HabitantType, HabitantTypeModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetHabitantDescription((HabitantTypeStatus)src.Status)));

            CreateMap<Employee, EmployeeCreateModel>();

            CreateMap<Employee, EmployeeListModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                            ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"));


            CreateMap<TaskEvidence, TaskEvidenceModel>();


            CreateMap<Employee, EmployeeFarmModel>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src =>
    src.Gender ? GenderEnum.Female : GenderEnum.Male))
            .ForMember(dest => dest.NameCode, opt => opt.MapFrom(src => string.Concat($"{src.Code} - ", src.Name).ToString()));




            CreateMap<Employee_Task, SubTaskModel>()
             .ForMember(dest => dest.TaskName, opt => opt.MapFrom(src => src.Task.Name))
             .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.Name));
        }

        public string FormatTimeDifference(DateTime startTime)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            TimeSpan timeDifference = vietnamTime - startTime;
            if (timeDifference.TotalDays >= 1)
            {
                int days = (int)timeDifference.TotalDays;
                return $"{days} ngày trước";
            }
            if (timeDifference.TotalHours >= 1)
            {
                int hours = (int)timeDifference.TotalHours;
                return $"{hours} giờ trước";
            }
            if (timeDifference.TotalMinutes >= 1)
            {
                int minutes = (int)timeDifference.TotalMinutes;
                return $"{minutes} phút trước";
            }
            int seconds = (int)timeDifference.TotalSeconds;
            return $"{seconds} giây trước";
        }

        private GenderEnum GetGenderFromBool(bool isMale)
        {
            return isMale ? GenderEnum.Male : GenderEnum.Female;
        }

        public static string GetTaskStatusDescription(TaskStatusEnum status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }

        public static string GetPriorityDescription(PriorityEnum status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }

        public static string GetTaskTypeDescription(PlantLivestockEnum status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }

        public static string GetHabitantDescription(HabitantTypeStatus status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }

        public static string GetGenderDescription(GenderEnum status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : status.ToString();
        }

    }
}
