//using AEMSWEB.Controllers;
//using AEMSWEB.Models.Masters;
//using AEMSWEB.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace AEMSWEB.Areas.Master.Controllers
//{
//    [Area("master")]
//    public class PortRegionController : BaseController
//    {
//        private readonly ILogger<PortRegionController> _logger;

//        public PortRegionController(
//            ILogger<PortRegionController> logger

//           )

//        {
//            _logger = logger;
//        }

//        // GET: /master/PortRegion/Index
//        public async Task<IActionResult> Index()
//        {
//            return View();
//        }

//        // GET: /master/PortRegion/List
//        [HttpGet]
//        public async Task<JsonResult> List(string searchString, string companyId)
//        {
//            try
//            {
//                var headers = new Dictionary<string, string>
//            {
//                { "PageSize", "10" },
//                { "PageNumber", "1" },
//                { "SearchString", searchString ?? string.Empty }
//            };

//                if (!string.IsNullOrEmpty(companyId))
//                {
//                    headers.Add("CompanyId", companyId);
//                }

//                var apiResponse = await _apiService.GetAsync<List<PortRegionViewModel>>("/master/getportregions", headers);
//                return Json(apiResponse.Data);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while fetching port regions.");
//                return Json(null);
//            }
//        }

//        // GET: /master/PortRegion/GetById
//        [HttpGet]
//        public async Task<JsonResult> GetById(short portRegionId, string companyId)
//        {
//            if (portRegionId <= 0)
//            {
//                return Json(new { success = false, message = "Invalid Port Region ID." });
//            }

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.GetAsync<PortRegionViewModel>($"/master/getportregionbyid/{portRegionId}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, data = apiResponse.Data });
//            }
//            else
//            {
//                return Json(new { success = false, message = "Port Region not found." });
//            }
//        }

//        // POST: /master/PortRegion/Save
//        [HttpPost]
//        public async Task<IActionResult> Save([FromBody] SavePortRegionViewModel model)
//        {
//            if (model == null)
//            {
//                return BadRequest(new { success = false, message = "Data operation failed." });
//            }

//            var portRegion = model.PortRegion;
//            var companyId = model.CompanyId;

//            var portRegionToSave = new PortRegionViewModel
//            {
//                PortRegionId = portRegion.PortRegionId,
//                CompanyId = Convert.ToInt16(companyId),
//                PortRegionCode = portRegion.PortRegionCode ?? string.Empty,
//                PortRegionName = portRegion.PortRegionName ?? string.Empty,
//                CountryId = portRegion.CountryId,
//                CountryCode = portRegion.CountryCode ?? string.Empty,
//                CountryName = portRegion.CountryName ?? string.Empty,
//                Remarks = portRegion.Remarks?.Trim() ?? string.Empty,
//                IsActive = portRegion.IsActive,
//                CreateById = portRegion.CreateById ?? 0,
//                CreateDate = DateTime.Now,
//                EditById = portRegion.EditById ?? 0,
//                EditDate = DateTime.Now,
//                CreateBy = portRegion.CreateBy ?? string.Empty,
//                EditBy = portRegion.EditBy ?? string.Empty
//            };

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.PostAsync<PortRegionViewModel>("/master/saveportregion", portRegionToSave, headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record saved successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Save operation failed." });
//        }

//        // DELETE: /master/PortRegion/Delete
//        [HttpDelete]
//        public async Task<IActionResult> Delete(int id, string companyId)
//        {
//            if (id <= 0)
//            {
//                return BadRequest(new { success = false, message = "Invalid ID." });
//            }

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.DeleteAsync($"/master/deleteportregion/{id}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record deleted successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Delete operation failed." });
//        }
//    }
//}