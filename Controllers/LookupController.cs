using AEMSWEB.IServices;
using AEMSWEB.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace AEMSWEB.Controllers
{
    public class LookupController : Controller
    {
        private readonly ILogger<LookupController> _logger;
        private readonly IMasterLookupService _masterLookupService;

        public LookupController(ILogger<LookupController> logger, IMasterLookupService masterLookupService)
        {
            _logger = logger;
            _masterLookupService = masterLookupService;
        }

        [HttpGet]
        public async Task<JsonResult> GetCompanyLookup(Int16 UserId)
        {
            var data = await _masterLookupService.GetCompanyLookupAsync(1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetUserLookup()
        {
            var data = await _masterLookupService.GetUserLookupAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetUserGroupLookup()
        {
            var data = await _masterLookupService.GetUserGroupLookupAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetModuleLookup(bool IsVisible, bool IsMandatory)
        {
            var data = await _masterLookupService.GetModuleLookupAsync(IsVisible, IsMandatory);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetTransactionLookup(Int16 moduleId)
        {
            var data = await _masterLookupService.GetTransactionLookupAsync(moduleId);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCurrencyLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetBankLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetChartOfAccountLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetChartOfAccountLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCountryLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCountryLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetVesselDynamic(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetVesselLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetBargeLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetAccountSetupCategoryLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetAccountSetupCategoryLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetAccountSetupLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetAccountSetupLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCOACategory1Lookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCOACategory1LookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCOACategory2Lookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCOACategory2LookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCOACategory3Lookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCOACategory3LookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetAccountTypeLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetAccountTypeLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetAccountGroupLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetAccountGroupLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetPortRegionLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetOrderTypeCategoryLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetDepartmentLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCustomerLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCreditTermsLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        //[HttpGet]
        //public async Task<JsonResult> GetPeriodCloseYear(string companyId)
        //{
        //     var headers = new Dictionary<string, string>(); var data = await _apiService.GetAsync<<PeriodCloseYearModel>>($"/master/getPeriodCloseYear",headers);
        //    return Json(data.Data);
        //}

        //[HttpGet]
        //public async Task<JsonResult> GetPeriodCloseNextYear(string companyId)
        //{
        //     var headers = new Dictionary<string, string>(); var data = await _apiService.GetAsync<<PeriodCloseNextYearModel>>($"/master/getPeriodCloseNextYear",headers);
        //    return Json(data.Data);
        //}

        [HttpGet]
        public async Task<JsonResult> GetGstCategoryLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetTaxCategoryLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetProductLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetGstLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetUomLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetPortLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetVoyageLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetEmployeeLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetTaxLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCustomerAddressLookupFin(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCustomerContactLookupFin(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetSupplierAddressLookupFin(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetSupplierContactLookupFin(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetSupplierLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetPaymentTypeLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetVesselLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetBargeDynamicLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCustomerDynamicLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetSupplierDynamicLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetProductDynamicLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetDocumentTypeLookup(string companyId)
        {
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, 1);
            return Json(data);
        }
    }
}