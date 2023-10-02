using AutoMapper;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model;
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
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.Name))
            .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.ReceiverId))
            .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field.Name))
            .ForMember(dest => dest.TaskTypeName, opt => opt.MapFrom(src => src.TaskType.Name))
            .ForMember(dest => dest.StatusTaskType, opt => opt.MapFrom(src => GetTaskTypeDescription((PlantLivestockEnum)src.TaskType.Status)))
            .ForMember(dest => dest.PlantName, opt => opt.MapFrom(src => src.Plant.Name))
            .ForMember(dest => dest.liveStockName, opt => opt.MapFrom(src => src.LiveStrock.Name))
            .ForMember(dest => dest.OtherName, opt => opt.MapFrom(src => src.Other.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetTaskStatusDescription((TaskStatusEnum)src.Status)))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => GetPriorityDescription((PriorityEnum)src.Priority)))
            .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => src.Field.Zone.Name))
            .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Field.Zone.Area.Name))
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.LiveStrock.ExternalId));


            CreateMap<FarmTask, TaskCreateUpdateModel>().ReverseMap();

            CreateMap<Field, FieldModel>()
                .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => src.Zone.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetHabitantDescription((HabitantTypeStatus)src.Status)))
                .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Zone.Area.Name));

            CreateMap<Plant, PlantModel>()
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field.Name))
                .ForMember(dest => dest.HabitantTypeName, opt => opt.MapFrom(src => src.HabitantType.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                            ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"))
                .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => src.Field.Zone.Name))
                .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Field.Zone.Area.Name));

            CreateMap<LiveStock, LiveStockModel>()
            .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field.Name))
            .ForMember(dest => dest.HabitantTypeName, opt => opt.MapFrom(src => src.HabitantType.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src =>
                ((GenderEnum)(src.Gender ? GenderEnum.Male : GenderEnum.Female)).GetDescription()))
             .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => src.Field.Zone.Name))
             .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Field.Zone.Area.Name));

            CreateMap<LiveStock, ExternalIdModel>()
             .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId))
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<Plant, ExternalIdModel>()
             .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId))
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            

            CreateMap<Member, MemberModel>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Farm.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                            ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"));

            CreateMap<Member, GetMemberModel>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));

            CreateMap<Material, MaterialModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                            ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"));

            CreateMap<Zone, ZoneModel>()
               .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.Name))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                            ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"));

            CreateMap<Area, AreaModel>()
               .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Farm.Name))
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

            CreateMap<Employee, EmployeeFarmModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                            ((EnumStatus)src.Status) == EnumStatus.Active ? "Active" : "Inactive"));
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
