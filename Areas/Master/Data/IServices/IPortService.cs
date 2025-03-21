﻿using AEMSWEB.Areas.Master.Models;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IPortService
    {
        public Task<PortViewModelCount> GetPortListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<PortViewModel> GetPortByIdAsync(short CompanyId, short UserId, short PortId);

        public Task<SqlResponse> SavePortAsync(short CompanyId, short UserId, M_Port m_Port);

        public Task<SqlResponse> DeletePortAsync(short CompanyId, short UserId, short portId);
    }
}