﻿using AMESWEB.IServices;
using Microsoft.AspNetCore.Mvc;

namespace AMESWEB.Controllers
{
    public class LookupController : BaseController
    {
        private readonly ILogger<LookupController> _logger;
        private readonly ILookupService _masterLookupService;

        #region Customer

        [HttpGet]
        public async Task<JsonResult> GetCustomerLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetCustomerLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCustomerAddressLookupFin(string companyId, int customerId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetCustomerAddressLookup_FinAsync(companyIdShort, parsedUserId.Value, customerId);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCustomerContactLookupFin(string companyId, int customerId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetCustomerContactLookup_FinAsync(companyIdShort, parsedUserId.Value, customerId);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCustomerJobOrderLookup(string companyId, int customerId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetCustomerJobOrderLookupAsync(companyIdShort, parsedUserId.Value, customerId);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCustomerCodeLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetCustomerCodeLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        #endregion Customer

        #region Supplier

        [HttpGet]
        public async Task<JsonResult> GetSupplierLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetSupplierLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetSupplierAddressLookupFin(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetSupplierAddressLookup_FinAsync(companyIdShort, parsedUserId.Value, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetSupplierContactLookupFin(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetSupplierContactLookup_FinAsync(companyIdShort, parsedUserId.Value, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetSupplierCodeLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetSupplierCodeLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        #endregion Supplier

        #region Bank

        [HttpGet]
        public async Task<JsonResult> GetBankLookup(string companyId, byte isOwnBank, byte isPettyCash)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetBankLookupAsync(companyIdShort, parsedUserId.Value,  isOwnBank,  isPettyCash);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetBankAddressLookupFin(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetBankAddressLookup_FinAsync(companyIdShort, parsedUserId.Value, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetBankContactLookupFin(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetBankContactLookup_FinAsync(companyIdShort, parsedUserId.Value, 1);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetBankCodeLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetBankCodeLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        #endregion Bank

        public LookupController(ILogger<LookupController> logger, IBaseService baseService, ILookupService masterLookupService) : base(logger, baseService)
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
        public async Task<JsonResult> GetModuleTransCategoryLookup(bool IsVisible, bool IsMandatory)
        {
            var data = await _masterLookupService.GetModuleTransCategoryLookupAsync(IsVisible, IsMandatory);
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
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetCurrencyLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetTaskLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetTaskLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetTaskByJobOrderLookup(string companyId, long jobOrderId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetTaskByJobOrderLookupAsync(companyIdShort, parsedUserId.Value, jobOrderId);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetChargeLookup(string companyId, short taskId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetChargeLookupAsync(companyIdShort, parsedUserId.Value, taskId);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCreditTermLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetCreditTermLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetChartOfAccountLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetChartOfAccountLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetAccountSetupCategoryLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetAccountSetupCategoryLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetAccountSetupLookup(string companyId, byte categoryId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetAccountSetupLookupAsync(companyIdShort, parsedUserId.Value, categoryId);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCountryLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetCountryLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetVesselDynamic(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetVesselLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetVesselLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetVesselLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetBargeLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetBargeLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCOACategory1Lookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetCOACategory1LookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCOACategory2Lookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetCOACategory2LookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetCOACategory3Lookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetCOACategory3LookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetAccountTypeLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetAccountTypeLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetAccountGroupLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetAccountGroupLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetOrderTypeLookup(string companyId, Int16 categoryId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetOrderTypeLookupAsync(companyIdShort, parsedUserId.Value, categoryId);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetPortRegionLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetPortRegionLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetOrderTypeCategoryLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetOrderTypeCategoryLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetDepartmentLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetDepartmentLookupAsync(companyIdShort, parsedUserId.Value);
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
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetGstCategoryLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetProductLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetProductLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetGstLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetGstLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetUomLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetUomLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetPortLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetPortLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetVoyageLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetVoyageLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetEmployeeLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetEmployeeLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetPaymentTypeLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetPaymentTypeLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetJobOrderLookup(string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var data = await _masterLookupService.GetJobOrderLookupAsync(companyIdShort, parsedUserId.Value);
            return Json(data);
        }
    }
}