using AutoMapper;
using OfficeOpenXml;
using SomoTaskManagement.Data.Abtract;
using SomoTaskManagement.Domain.Entities;
using SomoTaskManagement.Domain.Enum;
using SomoTaskManagement.Domain.Model.Employee;
using SomoTaskManagement.Domain.Model.TaskType;
using SomoTaskManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Services.Imp
{
    public class TaskTypeService: ITaskTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskTypeModel>> ListTaskType()
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(null);
            return _mapper.Map<IEnumerable<TaskType>, IEnumerable<TaskTypeModel>>(taskType);
        }
        public async Task<IEnumerable<TaskTypeModel>> ListTaskTypePlant()
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(expression:e=>e.Status ==0);
            return _mapper.Map<IEnumerable<TaskType>, IEnumerable<TaskTypeModel>>(taskType);
        }
        public async Task<IEnumerable<TaskTypeModel>> ListTaskTypeActive()
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(expression: e => e.IsDelete == false);
            return _mapper.Map<IEnumerable<TaskType>, IEnumerable<TaskTypeModel>>(taskType);
        }
        public async Task<IEnumerable<TaskTypeModel>> ListTaskTypeLivestock()
        {
            var taskType = await _unitOfWork.RepositoryTaskTaskType.GetData(expression: e => e.Status == 1);
            return _mapper.Map<IEnumerable<TaskType>, IEnumerable<TaskTypeModel>>(taskType);
        }

        public Task<TaskType> GetTaskType(int id)
        {
            return _unitOfWork.RepositoryTaskTaskType.GetById(id);
        }
        public async Task AddTaskType(TaskType taskType)
        {
            taskType.IsDelete = false;
            await _unitOfWork.RepositoryTaskTaskType.Add(taskType);
            await _unitOfWork.RepositoryTaskTaskType.Commit();
        }
        public async Task UpdateTaskType(TaskType taskType)
        {
            _unitOfWork.RepositoryTaskTaskType.Update(taskType);
            await _unitOfWork.RepositoryTaskTaskType.Commit();
        }
        public async Task DeleteTaskType(TaskType taskType)
        {
            _unitOfWork.RepositoryTaskTaskType.Delete(a => a.Id == taskType.Id);
            await _unitOfWork.RepositoryTaskTaskType.Commit();
        }

        public async Task<byte[]> ExportTaskTypeToExcel()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("TaskTypes");

                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Mã";
                worksheet.Cells[1, 3].Value = "Tên";
                worksheet.Cells[1, 4].Value = "Loại công việc";
                worksheet.Cells[1, 5].Value = "Mô tả";

                var taskTypes = await _unitOfWork.RepositoryTaskTaskType.GetData(e => e.IsDelete == false);

                int row = 2;
                int sequence = 1; 

                foreach (var taskType in taskTypes)
                {
                    worksheet.Cells[row, 1].Value = sequence;
                    worksheet.Cells[row, 2].Value = taskType.Id;
                    worksheet.Cells[row, 3].Value = taskType.Name;
                    worksheet.Cells[row, 4].Value = taskType.Status;
                    worksheet.Cells[row, 5].Value = taskType.Description;

                    row++;
                    sequence++;
                }

                return package.GetAsByteArray();
            }
        }

        public async Task ImportTaskTypeFromExcel(Stream excelFileStream)
        {
            try
            {
                using (var package = new ExcelPackage(excelFileStream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var taskTypeImport = new TaskType
                        {
                            Name = worksheet.Cells[row, 3].Value?.ToString(),
                            Status = Convert.ToInt32(worksheet.Cells[row, 4].Value),
                            Description = worksheet.Cells[row, 5].Value?.ToString(),
                        };

                        int excelTaskTypeId = Convert.ToInt32(worksheet.Cells[row, 2].Value); 

                        var existingTaskType = await _unitOfWork.RepositoryTaskTaskType
                            .GetSingleByCondition(tt => tt.Id == excelTaskTypeId);

                        if (existingTaskType == null)
                        {
                            var taskTypeNew = new TaskType
                            {
                                Name = taskTypeImport.Name,
                                Status = taskTypeImport.Status,
                                Description = taskTypeImport.Description,
                                IsDelete = false,
                            };

                            await _unitOfWork.RepositoryTaskTaskType.Add(taskTypeNew);
                        }
                        else
                        {
                            existingTaskType.Name = taskTypeImport.Name;
                            existingTaskType.Status = taskTypeImport.Status;
                            existingTaskType.Description = taskTypeImport.Description;

                            _unitOfWork.RepositoryTaskTaskType.Update(existingTaskType);
                        }
                    }

                    await _unitOfWork.RepositoryTaskTaskType.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during task type import: {ex.Message}");
            }
        }


    }
}
