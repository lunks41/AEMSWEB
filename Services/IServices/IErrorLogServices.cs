using AEMSWEB.Entities.Admin;

namespace AEMSWEB.ModelsServices
{
    public interface IErrorLogServices
    {
        public Task AddErrorLogAsync(AdmErrorLog errorLog);
    }
}