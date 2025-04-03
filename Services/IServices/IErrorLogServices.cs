using AMESWEB.Entities.Admin;

namespace AMESWEB.ModelsServices
{
    public interface IErrorLogServices
    {
        public Task AddErrorLogAsync(AdmErrorLog errorLog);
    }
}