﻿using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Controllers;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.Models.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AEMSWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class CustomerController : BaseController
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;

        public CustomerController(ILogger<CustomerController> logger,
            IBaseService baseService,
            ICustomerService customerService)
            : base(logger, baseService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        #region Customer CRUD

        [Authorize]
        public async Task<IActionResult> Index(int? companyId)
        {
            if (!companyId.HasValue || companyId <= 0)
            {
                _logger.LogWarning("Invalid company ID: {CompanyId}", companyId);
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var parsedUserId = GetParsedUserId();
            if (!parsedUserId.HasValue)
            {
                _logger.LogWarning("User not logged in or invalid user ID.");
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var permissions = await HasPermission((short)companyId, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Customer);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> List(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _customerService.GetCustomerListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int customerId, string companyId)
        {
            if (customerId <= 0)
                return Json(new { success = false, message = "Invalid Customer ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _customerService.GetCustomerByIdAsync(companyIdShort, parsedUserId.Value, customerId, "", "");
                return data == null
                    ? Json(new { success = false, message = "Customer not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveCustomerViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var customerToSave = new M_Customer
                {
                    CustomerId = model.customer.CustomerId,
                    CompanyId = companyIdShort,
                    CustomerCode = model.customer.CustomerCode ?? string.Empty,
                    CustomerName = model.customer.CustomerName ?? string.Empty,
                    CustomerOtherName = model.customer.CustomerOtherName ?? string.Empty,
                    CustomerShortName = model.customer.CustomerShortName ?? string.Empty,
                    CustomerRegNo = model.customer.CustomerRegNo ?? string.Empty,
                    CurrencyId = model.customer.CurrencyId,
                    CreditTermId = model.customer.CreditTermId,
                    ParentCustomerId = model.customer.ParentCustomerId,
                    AccSetupId = model.customer.AccSetupId,
                    SupplierId = model.customer.SupplierId,
                    BankId = model.customer.BankId,
                    IsCustomer = model.customer.IsCustomer,
                    IsVendor = model.customer.IsVendor,
                    IsTrader = model.customer.IsTrader,
                    IsSupplier = model.customer.IsSupplier,
                    Remarks = model.customer.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.customer.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _customerService.SaveCustomerAsync(companyIdShort, parsedUserId.Value, customerToSave);
                return Json(new { success = true, message = "Customer saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving customer");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int CustomerId, string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Customer);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _customerService.DeleteCustomerAsync(companyIdShort, parsedUserId.Value, CustomerId);
                return Json(new { success = true, message = "Customer deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Customer CRUD

        #region CustomerContact CRUD

        [HttpGet]
        public async Task<JsonResult> ListContacts(int CustomerId, string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _customerService.GetCustomerContactByCustomerIdAsync(companyIdShort, parsedUserId.Value,
                   CustomerId);
                return Json(new { data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer contact list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetContactById(int CustomerId, short contactId, string companyId)
        {
            if (contactId <= 0)
                return Json(new { success = false, message = "Invalid Contact ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _customerService.GetCustomerContactByIdAsync(companyIdShort, parsedUserId.Value, CustomerId, contactId);
                return data == null
                    ? Json(new { success = false, message = "Customer Contact not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer contact by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveContact([FromBody] SaveCustomerContactViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var contactToSave = new M_CustomerContact
                {
                    ContactId = model.customerContact.ContactId,
                    CustomerId = model.customerContact.CustomerId,
                    ContactName = model.customerContact.ContactName ?? string.Empty,
                    OtherName = model.customerContact.OtherName ?? string.Empty,
                    MobileNo = model.customerContact.MobileNo ?? string.Empty,
                    OffNo = model.customerContact.OffNo ?? string.Empty,
                    FaxNo = model.customerContact.FaxNo ?? string.Empty,
                    EmailAdd = model.customerContact.EmailAdd ?? string.Empty,
                    MessId = model.customerContact.MessId ?? string.Empty,
                    ContactMessType = model.customerContact.ContactMessType ?? string.Empty,
                    IsDefault = model.customerContact.IsDefault,
                    IsFinance = model.customerContact.IsFinance,
                    IsSales = model.customerContact.IsSales,
                    IsActive = model.customerContact.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _customerService.SaveCustomerContactAsync(companyIdShort, parsedUserId.Value, contactToSave);
                return Json(new { success = true, message = "Customer Contact saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving customer contact");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteContact(int CustomerId, short contactId, string companyId)
        {
            if (contactId <= 0)
                return Json(new { success = false, message = "Invalid Contact ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Customer);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _customerService.DeleteCustomerContactAsync(companyIdShort, parsedUserId.Value, CustomerId, contactId);
                return Json(new { success = true, message = "Customer Contact deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer contact");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion CustomerContact CRUD

        #region CustomerAddress CRUD

        [HttpGet]
        public async Task<JsonResult> ListAddresses(int CustomerId, string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _customerService.GetCustomerAddressByCustomerIdAsync(companyIdShort, parsedUserId.Value,
                    CustomerId);
                return Json(new { data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer address list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAddressById(int CustomerId, short addressId, string companyId)
        {
            if (addressId <= 0)
                return Json(new { success = false, message = "Invalid Address ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _customerService.GetCustomerAddressByIdAsync(companyIdShort, parsedUserId.Value, CustomerId, addressId);
                return data == null
                    ? Json(new { success = false, message = "Customer Address not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer address by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveAddress([FromBody] SaveCustomerAddressViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var addressToSave = new M_CustomerAddress
                {
                    AddressId = model.customerAddress.AddressId,
                    CustomerId = model.customerAddress.CustomerId,
                    Address1 = model.customerAddress.Address1 ?? string.Empty,
                    Address2 = model.customerAddress.Address2 ?? string.Empty,
                    Address3 = model.customerAddress.Address3 ?? string.Empty,
                    Address4 = model.customerAddress.Address4 ?? string.Empty,
                    PinCode = model.customerAddress.PinCode ?? string.Empty,
                    CountryId = model.customerAddress.CountryId,
                    PhoneNo = model.customerAddress.PhoneNo ?? string.Empty,
                    FaxNo = model.customerAddress.FaxNo ?? string.Empty,
                    EmailAdd = model.customerAddress.EmailAdd ?? string.Empty,
                    WebUrl = model.customerAddress.WebUrl ?? string.Empty,
                    IsDefaultAdd = model.customerAddress.IsDefaultAdd,
                    IsDeliveryAdd = model.customerAddress.IsDeliveryAdd,
                    IsFinAdd = model.customerAddress.IsFinAdd,
                    IsSalesAdd = model.customerAddress.IsSalesAdd,
                    IsActive = model.customerAddress.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _customerService.SaveCustomerAddressAsync(companyIdShort, parsedUserId.Value, addressToSave);
                return Json(new { success = true, message = "Customer Address saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving customer address");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAddress(int CustomerId, short contactId, string companyId)
        {
            if (contactId <= 0)
                return Json(new { success = false, message = "Invalid Address ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Customer);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _customerService.DeleteCustomerAddressAsync(companyIdShort, parsedUserId.Value, CustomerId, contactId);
                return Json(new { success = true, message = "Customer Address deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer contact");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion CustomerAddress CRUD
    }
}