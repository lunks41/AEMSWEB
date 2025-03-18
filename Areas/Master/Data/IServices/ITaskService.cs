using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ITaskService
    {
        public Task<TaskViewModelCount> GetTaskListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_Task> GetTaskByIdAsync(short CompanyId, short UserId, int TaskId);

        public Task<SqlResponse> SaveTaskAsync(short CompanyId, short UserId, M_Task m_Task);

        public Task<SqlResponse> DeleteTaskAsync(short CompanyId, short UserId, M_Task m_Task);
    }
}