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
        private readonly ISupplierService _supplierService;

        public SupplierController(ILogger<SupplierController> logger,
            IBaseService baseService,
            ISupplierService supplierService)
            : base(logger, baseService)
        {
            _logger = logger;
            _supplierService = supplierService;
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
                var data = await _supplierService.GetSupplierListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching supplier list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int supplierId, string companyId)
        {
            if (supplierId <= 0)
                return Json(new { success = false, message = "Invalid Supplier ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _supplierService.GetSupplierByIdAsync(companyIdShort, parsedUserId.Value, supplierId, "", "");
                return data == null
                    ? Json(new { success = false, message = "Supplier not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching supplier by ID");
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
                var supplierToSave = new M_Supplier
                {
                    SupplierId = model.supplier.SupplierId,
                    CompanyId = companyIdShort,
                    SupplierCode = model.supplier.SupplierCode ?? string.Empty,
                    SupplierName = model.supplier.SupplierName ?? string.Empty,
                    SupplierOtherName = model.supplier.SupplierOtherName ?? string.Empty,
                    SupplierShortName = model.supplier.SupplierShortName ?? string.Empty,
                    SupplierRegNo = model.supplier.SupplierRegNo ?? string.Empty,
                    CurrencyId = model.supplier.CurrencyId,
                    CreditTermId = model.supplier.CreditTermId,
                    ParentSupplierId = model.supplier.ParentSupplierId,
                    AccSetupId = model.supplier.AccSetupId,
                    CustomerId = model.supplier.CustomerId,
                    BankId = model.supplier.BankId,
                    IsCustomer = model.supplier.IsCustomer,
                    IsVendor = model.supplier.IsVendor,
                    IsTrader = model.supplier.IsTrader,
                    IsSupplier = model.supplier.IsSupplier,
                    Remarks = model.supplier.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.supplier.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var sqlResponse = await _supplierService.SaveSupplierAsync(companyIdShort, parsedUserId.Value, supplierToSave);

                if (sqlResponse.Result > 0)
                {
                    var supplierModel = await _supplierService.GetSupplierByIdAsync(companyIdShort, parsedUserId.Value, Convert.ToInt32(sqlResponse.Result), "", "");

                    return Json(new { success = true, message = "Supplier saved successfully", data = supplierModel });
                }

                return Json(new { success = false, message = sqlResponse.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving supplier");
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
                await _supplierService.DeleteSupplierAsync(companyIdShort, parsedUserId.Value, SupplierId);
                return Json(new { success = true, message = "Supplier deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supplier");
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
                var data = await _supplierService.GetSupplierContactBySupplierIdAsync(companyIdShort, parsedUserId.Value,
                   SupplierId);
                return Json(new { data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching supplier contact list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetContactById(int supplierId, short contactId, string companyId)
        {
            if (contactId <= 0)
                return Json(new { success = false, message = "Invalid Contact ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _supplierService.GetSupplierContactByIdAsync(companyIdShort, parsedUserId.Value, supplierId, contactId);
                return data == null
                    ? Json(new { success = false, message = "Supplier Contact not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching supplier contact by ID");
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
                    ContactId = model.supplierContact.ContactId,
                    SupplierId = model.supplierContact.SupplierId,
                    ContactName = model.supplierContact.ContactName ?? string.Empty,
                    OtherName = model.supplierContact.OtherName ?? string.Empty,
                    MobileNo = model.supplierContact.MobileNo ?? string.Empty,
                    OffNo = model.supplierContact.OffNo ?? string.Empty,
                    FaxNo = model.supplierContact.FaxNo ?? string.Empty,
                    EmailAdd = model.supplierContact.EmailAdd ?? string.Empty,
                    MessId = model.supplierContact.MessId ?? string.Empty,
                    ContactMessType = model.supplierContact.ContactMessType ?? string.Empty,
                    IsDefault = model.supplierContact.IsDefault,
                    IsFinance = model.supplierContact.IsFinance,
                    IsSales = model.supplierContact.IsSales,
                    IsActive = model.supplierContact.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _supplierService.SaveSupplierContactAsync(companyIdShort, parsedUserId.Value, contactToSave);
                return Json(new { success = true, message = "Supplier Contact saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving supplier contact");
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
                await _supplierService.DeleteSupplierContactAsync(companyIdShort, parsedUserId.Value, SupplierId, contactId);
                return Json(new { success = true, message = "Supplier Contact deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supplier contact");
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
                var data = await _supplierService.GetSupplierAddressBySupplierIdAsync(companyIdShort, parsedUserId.Value,
                    SupplierId);
                return Json(new { data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching supplier address list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAddressById(int supplierId, short addressId, short companyId)
        {
            if (addressId <= 0)
                return Json(new { success = false, message = "Invalid Address ID" });

            var validationResult = ValidateCompanyAndUserId(companyId.ToString(), out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _supplierService.GetSupplierAddressByIdAsync(companyIdShort, parsedUserId.Value, supplierId, addressId);
                return data == null
                    ? Json(new { success = false, message = "Supplier Address not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching supplier address by ID");
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
                    AddressId = model.supplierAddress.AddressId,
                    SupplierId = model.supplierAddress.SupplierId,
                    Address1 = model.supplierAddress.Address1 ?? string.Empty,
                    Address2 = model.supplierAddress.Address2 ?? string.Empty,
                    Address3 = model.supplierAddress.Address3 ?? string.Empty,
                    Address4 = model.supplierAddress.Address4 ?? string.Empty,
                    PinCode = model.supplierAddress.PinCode ?? string.Empty,
                    CountryId = model.supplierAddress.CountryId,
                    PhoneNo = model.supplierAddress.PhoneNo ?? string.Empty,
                    FaxNo = model.supplierAddress.FaxNo ?? string.Empty,
                    EmailAdd = model.supplierAddress.EmailAdd ?? string.Empty,
                    WebUrl = model.supplierAddress.WebUrl ?? string.Empty,
                    IsDefaultAdd = model.supplierAddress.IsDefaultAdd,
                    IsDeliveryAdd = model.supplierAddress.IsDeliveryAdd,
                    IsFinAdd = model.supplierAddress.IsFinAdd,
                    IsSalesAdd = model.supplierAddress.IsSalesAdd,
                    IsActive = model.supplierAddress.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _supplierService.SaveSupplierAddressAsync(companyIdShort, parsedUserId.Value, addressToSave);
                return Json(new { success = true, message = "Supplier Address saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving supplier address");
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
                await _supplierService.DeleteSupplierAddressAsync(companyIdShort, parsedUserId.Value, SupplierId, contactId);
                return Json(new { success = true, message = "Supplier Address deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supplier contact");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion SupplierAddress CRUD
    }
}