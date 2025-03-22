using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Areas.Master.Models;
using AEMSWEB.Controllers;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class BankController : BaseController
    {
        private readonly ILogger<BankController> _logger;
        private readonly IBankService _bankService;

        public BankController(ILogger<BankController> logger,
            IBaseService baseService,
            IBankService bankService)
            : base(logger, baseService)
        {
            _logger = logger;
            _bankService = bankService;
        }

        #region Bank CRUD

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
                (short)E_Modules.Master, (short)E_Master.Bank);

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
                var data = await _bankService.GetBankListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bank list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(short bankId, string companyId)
        {
            if (bankId <= 0)
                return Json(new { success = false, message = "Invalid Bank ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _bankService.GetBankByIdAsync(companyIdShort, parsedUserId.Value, bankId);
                return data == null
                    ? Json(new { success = false, message = "Bank not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bank by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveBankViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var bankToSave = new M_Bank
                {
                    BankId = model.bank.BankId,
                    CompanyId = companyIdShort,
                    BankCode = model.bank.BankCode ?? string.Empty,
                    BankName = model.bank.BankName ?? string.Empty,
                    CurrencyId = model.bank.CurrencyId,
                    AccountNo = model.bank.AccountNo ?? string.Empty,
                    SwiftCode = model.bank.SwiftCode ?? string.Empty,
                    Remarks1 = model.bank.Remarks1 ?? string.Empty,
                    Remarks2 = model.bank.Remarks2 ?? string.Empty,
                    GLId = model.bank.GLId,
                    IsActive = model.bank.IsActive,
                    IsOwnBank = model.bank.IsOwnBank,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _bankService.SaveBankAsync(companyIdShort, parsedUserId.Value, bankToSave);
                return Json(new { success = true, message = "Bank saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving bank");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(short BankId, string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Bank);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _bankService.DeleteBankAsync(companyIdShort, parsedUserId.Value, BankId);
                return Json(new { success = true, message = "Bank deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bank");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Bank CRUD

        //#region BankContact CRUD

        //[HttpGet]
        //public async Task<JsonResult> ListContacts(int BankId, string companyId)
        //{
        //    var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
        //    if (validationResult != null) return validationResult;

        //    try
        //    {
        //        var data = await _bankService.GetBankContactByBankIdAsync(companyIdShort, parsedUserId.Value,
        //           BankId);
        //        return Json(new { data = data });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error fetching bank contact list");
        //        return Json(new { success = false, message = "An error occurred" });
        //    }
        //}

        //[HttpGet]
        //public async Task<JsonResult> GetContactById(int BankId, short contactId, string companyId)
        //{
        //    if (contactId <= 0)
        //        return Json(new { success = false, message = "Invalid Contact ID" });

        //    var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
        //    if (validationResult != null) return validationResult;

        //    try
        //    {
        //        var data = await _bankService.GetBankContactByIdAsync(companyIdShort, parsedUserId.Value, BankId, contactId);
        //        return data == null
        //            ? Json(new { success = false, message = "Bank Contact not found" })
        //            : Json(new { success = true, data });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error fetching bank contact by ID");
        //        return Json(new { success = false, message = "An error occurred" });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> SaveContact([FromBody] SaveBankContactViewModel model)
        //{
        //    if (model == null || !ModelState.IsValid)
        //        return Json(new { success = false, message = "Invalid request data" });

        //    var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
        //    if (validationResult != null) return validationResult;

        //    try
        //    {
        //        var contactToSave = new M_BankContact
        //        {
        //            ContactId = model.bankContact.ContactId,
        //            BankId = model.bankContact.BankId,
        //            ContactName = model.bankContact.ContactName ?? string.Empty,
        //            OtherName = model.bankContact.OtherName ?? string.Empty,
        //            MobileNo = model.bankContact.MobileNo ?? string.Empty,
        //            OffNo = model.bankContact.OffNo ?? string.Empty,
        //            FaxNo = model.bankContact.FaxNo ?? string.Empty,
        //            EmailAdd = model.bankContact.EmailAdd ?? string.Empty,
        //            MessId = model.bankContact.MessId ?? string.Empty,
        //            ContactMessType = model.bankContact.ContactMessType ?? string.Empty,
        //            IsDefault = model.bankContact.IsDefault,
        //            IsFinance = model.bankContact.IsFinance,
        //            IsSales = model.bankContact.IsSales,
        //            IsActive = model.bankContact.IsActive,
        //            CreateById = parsedUserId.Value,
        //            CreateDate = DateTime.Now,
        //            EditById = model.bankContact.EditById ?? 0,
        //            EditDate = DateTime.Now
        //        };

        //        var result = await _bankService.SaveBankContactAsync(companyIdShort, parsedUserId.Value, contactToSave);
        //        return Json(new { success = true, message = "Bank Contact saved successfully", data = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error saving bank contact");
        //        return Json(new { success = false, message = "An error occurred" });
        //    }
        //}

        //[HttpDelete]
        //public async Task<IActionResult> DeleteContact(int BankId, short contactId, string companyId)
        //{
        //    if (contactId <= 0)
        //        return Json(new { success = false, message = "Invalid Contact ID" });

        //    var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
        //    if (validationResult != null) return validationResult;

        //    var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
        //        (short)E_Modules.Master, (short)E_Master.Bank);

        //    if (permissions == null || !permissions.IsDelete)
        //        return Json(new { success = false, message = "No delete permission" });

        //    try
        //    {
        //        await _bankService.DeleteBankContactAsync(companyIdShort, parsedUserId.Value, BankId, contactId);
        //        return Json(new { success = true, message = "Bank Contact deleted successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error deleting bank contact");
        //        return Json(new { success = false, message = "An error occurred" });
        //    }
        //}

        //#endregion BankContact CRUD

        //#region BankAddress CRUD

        //[HttpGet]
        //public async Task<JsonResult> ListAddresses(int BankId, string companyId)
        //{
        //    var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
        //    if (validationResult != null) return validationResult;

        //    try
        //    {
        //        var data = await _bankService.GetBankAddressByBankIdAsync(companyIdShort, parsedUserId.Value,
        //            BankId);
        //        return Json(new { data = data });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error fetching bank address list");
        //        return Json(new { success = false, message = "An error occurred" });
        //    }
        //}

        //[HttpGet]
        //public async Task<JsonResult> GetAddressById(int BankId, short addressId, string companyId)
        //{
        //    if (addressId <= 0)
        //        return Json(new { success = false, message = "Invalid Address ID" });

        //    var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
        //    if (validationResult != null) return validationResult;

        //    try
        //    {
        //        var data = await _bankService.GetBankAddressByIdAsync(companyIdShort, parsedUserId.Value, BankId, addressId);
        //        return data == null
        //            ? Json(new { success = false, message = "Bank Address not found" })
        //            : Json(new { success = true, data });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error fetching bank address by ID");
        //        return Json(new { success = false, message = "An error occurred" });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> SaveAddress([FromBody] SaveBankAddressViewModel model)
        //{
        //    if (model == null || !ModelState.IsValid)
        //        return Json(new { success = false, message = "Invalid request data" });

        //    var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
        //    if (validationResult != null) return validationResult;

        //    try
        //    {
        //        var addressToSave = new M_BankAddress
        //        {
        //            AddressId = model.bankAddress.AddressId,
        //            BankId = model.bankAddress.BankId,
        //            Address1 = model.bankAddress.Address1 ?? string.Empty,
        //            Address2 = model.bankAddress.Address2 ?? string.Empty,
        //            Address3 = model.bankAddress.Address3 ?? string.Empty,
        //            Address4 = model.bankAddress.Address4 ?? string.Empty,
        //            PinCode = model.bankAddress.PinCode ?? string.Empty,
        //            CountryId = model.bankAddress.CountryId,
        //            PhoneNo = model.bankAddress.PhoneNo ?? string.Empty,
        //            FaxNo = model.bankAddress.FaxNo ?? string.Empty,
        //            EmailAdd = model.bankAddress.EmailAdd ?? string.Empty,
        //            WebUrl = model.bankAddress.WebUrl ?? string.Empty,
        //            IsDefaultAdd = model.bankAddress.IsDefaultAdd,
        //            IsDeliveryAdd = model.bankAddress.IsDeliveryAdd,
        //            IsFinAdd = model.bankAddress.IsFinAdd,
        //            IsSalesAdd = model.bankAddress.IsSalesAdd,
        //            IsActive = model.bankAddress.IsActive,
        //            CreateById = parsedUserId.Value,
        //            CreateDate = DateTime.Now,
        //            EditById = model.bankAddress.EditById ?? 0,
        //            EditDate = DateTime.Now
        //        };

        //        var result = await _bankService.SaveBankAddressAsync(companyIdShort, parsedUserId.Value, addressToSave);
        //        return Json(new { success = true, message = "Bank Address saved successfully", data = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error saving bank address");
        //        return Json(new { success = false, message = "An error occurred" });
        //    }
        //}

        //[HttpDelete]
        //public async Task<IActionResult> DeleteAddress(int BankId, short contactId, string companyId)
        //{
        //    if (contactId <= 0)
        //        return Json(new { success = false, message = "Invalid Address ID" });

        //    var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
        //    if (validationResult != null) return validationResult;

        //    var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
        //        (short)E_Modules.Master, (short)E_Master.Bank);

        //    if (permissions == null || !permissions.IsDelete)
        //        return Json(new { success = false, message = "No delete permission" });

        //    try
        //    {
        //        await _bankService.DeleteBankAddressAsync(companyIdShort, parsedUserId.Value, BankId, contactId);
        //        return Json(new { success = true, message = "Bank Address deleted successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error deleting bank contact");
        //        return Json(new { success = false, message = "An error occurred" });
        //    }
        //}

        //#endregion BankAddress CRUD
    }
}