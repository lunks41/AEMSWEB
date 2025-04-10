using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface ITaskService
    {
        public Task<TaskViewModelCount> GetTaskListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<TaskViewModel> GetTaskByIdAsync(short CompanyId, short UserId, short taskId);

        public Task<SqlResponce> SaveTaskAsync(short CompanyId, short UserId, M_Task m_Task);

        public Task<SqlResponce> DeleteTaskAsync(short CompanyId, short UserId, short taskId);
    }
}