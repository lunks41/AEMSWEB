using AEMSWEB.Models;
using AEMSWEB.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ChangelogController : ControllerBase
    {
        private readonly IChangelogService _changelogService;

        public ChangelogController(IChangelogService changelogService)
        {
            _changelogService = changelogService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicEntries()
        {
            var entries = await _changelogService.GetPublicEntriesAsync();
            return Ok(entries);
        }

        [HttpPost]
        public async Task<IActionResult> AddEntry([FromBody] ChangelogEntry entry)
        {
            var createdEntry = await _changelogService.AddEntryAsync(entry);
            return CreatedAtAction(nameof(GetPublicEntries), createdEntry);
        }
    }
}