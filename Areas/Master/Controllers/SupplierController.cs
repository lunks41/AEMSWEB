using AMESWEB.Areas.Master.Data.IServices;
using AMESWEB.Controllers;
using AMESWEB.Entities.Masters;
using AMESWEB.Enums;
using AMESWEB.IServices;
using AMESWEB.Models.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMESWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class SupplierController : BaseController
    {
        private readonly ILogger<SupplierController> _logger;
        private readonly ISupplierService _customerService;

        public SupplierController(ILogger<SupplierController> logger,
            IBaseService baseService,
            ISupplierService customerService)
            : base(logger, baseService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        #region Supplier CRUD

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
                (short)E_Modules.Master, (short)E_Master.Supplier);

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

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _customerService.GetSupplierListAsync(companyIdShort, parsedUserId.Value,
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
                return Json(new { success = false, message = "Invalid Supplier ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _customerService.GetSupplierByIdAsync(companyIdShort, parsedUserId.Value, customerId, "", "");
                return data == null
                    ? Json(new { success = false, message = "Supplier not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveSupplierViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var customerToSave = new M_Supplier
                {
                    SupplierId = model.customer.SupplierId,
                    CompanyId = companyIdShort,
                    SupplierCode = model.customer.SupplierCode ?? string.Empty,
                    SupplierName = model.customer.SupplierName ?? string.Empty,
                    SupplierOtherName = model.customer.SupplierOtherName ?? string.Empty,
                    SupplierShortName = model.customer.SupplierShortName ?? string.Empty,
                    SupplierRegNo = model.customer.SupplierRegNo ?? string.Empty,
                    CurrencyId = model.customer.CurrencyId,
                    CreditTermId = model.customer.CreditTermId,
                    ParentSupplierId = model.customer.ParentSupplierId,
                    AccSetupId = model.customer.AccSetupId,
                    CustomerId = model.customer.CustomerId,
                    IsSupplier = model.customer.IsSupplier,
                    IsVendor = model.customer.IsVendor,
                    IsTrader = model.customer.IsTrader,
                    Remarks = model.customer.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.customer.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _customerService.SaveSupplierAsync(companyIdShort, parsedUserId.Value, customerToSave);
                return Json(new { success = true, message = "Supplier saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving customer");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int SupplierId, string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Supplier);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _customerService.DeleteSupplierAsync(companyIdShort, parsedUserId.Value, SupplierId);
                return Json(new { success = true, message = "Supplier deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Supplier CRUD

        #region SupplierContact CRUD

        [HttpGet]
        public async Task<JsonResult> ListContacts(int SupplierId, string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _customerService.GetSupplierContactBySupplierIdAsync(companyIdShort, parsedUserId.Value,
                   SupplierId);
                return Json(new { data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer contact list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetContactById(int customerId, short contactId, string companyId)
        {
            if (contactId <= 0)
                return Json(new { success = false, message = "Invalid Contact ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _customerService.GetSupplierContactByIdAsync(companyIdShort, parsedUserId.Value, customerId, contactId);
                return data == null
                    ? Json(new { success = false, message = "Supplier Contact not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer contact by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveContact([FromBody] SaveSupplierContactViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var contactToSave = new M_SupplierContact
                {
                    ContactId = model.customerContact.ContactId,
                    SupplierId = model.customerContact.SupplierId,
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

                var result = await _customerService.SaveSupplierContactAsync(companyIdShort, parsedUserId.Value, contactToSave);
                return Json(new { success = true, message = "Supplier Contact saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving customer contact");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteContact(int SupplierId, short contactId, string companyId)
        {
            if (contactId <= 0)
                return Json(new { success = false, message = "Invalid Contact ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Supplier);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _customerService.DeleteSupplierContactAsync(companyIdShort, parsedUserId.Value, SupplierId, contactId);
                return Json(new { success = true, message = "Supplier Contact deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer contact");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion SupplierContact CRUD

        #region SupplierAddress CRUD

        [HttpGet]
        public async Task<JsonResult> ListAddresses(int SupplierId, string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _customerService.GetSupplierAddressBySupplierIdAsync(companyIdShort, parsedUserId.Value,
                    SupplierId);
                return Json(new { data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer address list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAddressById(int customerId, short addressId, short companyId)
        {
            if (addressId <= 0)
                return Json(new { success = false, message = "Invalid Address ID" });

            var validationResult = ValidateCompanyAndUserId(companyId.ToString(), out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _customerService.GetSupplierAddressByIdAsync(companyIdShort, parsedUserId.Value, customerId, addressId);
                return data == null
                    ? Json(new { success = false, message = "Supplier Address not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer address by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveAddress([FromBody] SaveSupplierAddressViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var addressToSave = new M_SupplierAddress
                {
                    AddressId = model.customerAddress.AddressId,
                    SupplierId = model.customerAddress.SupplierId,
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

                var result = await _customerService.SaveSupplierAddressAsync(companyIdShort, parsedUserId.Value, addressToSave);
                return Json(new { success = true, message = "Supplier Address saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving customer address");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAddress(int SupplierId, short contactId, string companyId)
        {
            if (contactId <= 0)
                return Json(new { success = false, message = "Invalid Address ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Supplier);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _customerService.DeleteSupplierAddressAsync(companyIdShort, parsedUserId.Value, SupplierId, contactId);
                return Json(new { success = true, message = "Supplier Address deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer contact");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion SupplierAddress CRUD
    }
}