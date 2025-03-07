//using AEMSWEB.Controllers;
//using AEMSWEB.Models.Masters;
//using AEMSWEB.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace AEMSWEB.Areas.Master.Controllers
//{
//    [Area("master")]
//    public class VoyageController : BaseController
//    {
//        private readonly ILogger<VoyageController> _logger;

//        public VoyageController(
//            ILogger<VoyageController> logger

//           )

//        {
//            _logger = logger;
//        }

//        // GET: /master/Voyage/Index
//        public async Task<IActionResult> Index()
//        {
//            return View();
//        }

//        // GET: /master/Voyage/List
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

//                var apiResponse = await _apiService.GetAsync<List<VoyageViewModel>>("/master/getvoyages", headers);
//                return Json(apiResponse.Data);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while fetching voyages.");
//                return Json(null);
//            }
//        }

//        // GET: /master/Voyage/GetById
//        [HttpGet]
//        public async Task<JsonResult> GetById(short voyageId, string companyId)
//        {
//            if (voyageId <= 0)
//            {
//                return Json(new { success = false, message = "Invalid Voyage ID." });
//            }

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.GetAsync<VoyageViewModel>($"/master/getvoyagebyid/{voyageId}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, data = apiResponse.Data });
//            }
//            else
//            {
//                return Json(new { success = false, message = "Voyage not found." });
//            }
//        }

//        // POST: /master/Voyage/Save
//        [HttpPost]
//        public async Task<IActionResult> Save([FromBody] SaveVoyageViewModel model)
//        {
//            if (model == null)
//            {
//                return BadRequest(new { success = false, message = "Data operation failed." });
//            }

//            var voyage = model.Voyage;
//            var companyId = model.CompanyId;

//            var voyageToSave = new VoyageViewModel
//            {
//                CompanyId = Convert.ToInt16(companyId),
//                VoyageId = voyage.VoyageId,
//                VoyageNo = voyage.VoyageNo ?? string.Empty,
//                ReferenceNo = voyage.ReferenceNo ?? string.Empty,
//                VesselId = voyage.VesselId,
//                VesselCode = voyage.VesselCode ?? string.Empty,
//                VesselName = voyage.VesselName ?? string.Empty,
//                BargeId = voyage.BargeId,
//                BargeCode = voyage.BargeCode ?? string.Empty,
//                BargeName = voyage.BargeName ?? string.Empty,
//                Remarks = voyage.Remarks?.Trim() ?? string.Empty,
//                IsActive = voyage.IsActive,
//                CreateById = voyage.CreateById ?? 0,
//                CreateDate = DateTime.Now,
//                EditById = voyage.EditById ?? 0,
//                EditDate = DateTime.Now,
//                CreateBy = voyage.CreateBy ?? string.Empty,
//                EditBy = voyage.EditBy ?? string.Empty
//            };

//            var headers = new Dictionary<string, string>();
//            if (!string.IsNullOrEmpty(companyId))
//            {
//                headers.Add("CompanyId", companyId);
//            }

//            var apiResponse = await _apiService.PostAsync<VoyageViewModel>("/master/savevoyage", voyageToSave, headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record saved successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Save operation failed." });
//        }

//        // DELETE: /master/Voyage/Delete
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

//            var apiResponse = await _apiService.DeleteAsync($"/master/deletevoyage/{id}", headers);

//            if (apiResponse.Result > 0)
//            {
//                return Json(new { success = true, message = "Record deleted successfully." });
//            }

//            return BadRequest(new { success = false, message = apiResponse.Message?.ToString() ?? "Delete operation failed." });
//        }
//    }
//}