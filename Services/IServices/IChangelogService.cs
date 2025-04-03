using AMESWEB.Models;

namespace AMESWEB.Services.IServices
{
    public interface IChangelogService
    {
        Task<IEnumerable<ChangelogEntry>> GetPublicEntriesAsync();

        Task<ChangelogEntry> AddEntryAsync(ChangelogEntry entry);

        Task<bool> DeleteEntryAsync(int id);

        Task<IEnumerable<ChangelogEntry>> SearchEntriesAsync(string query, string changeType);
    }
}