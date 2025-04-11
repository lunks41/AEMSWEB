using AMESWEB.Areas.Account.Data.IServices.AR;
using AMESWEB.Areas.Account.Models.AR;
using AMESWEB.Controllers;
using AMESWEB.Entities.Accounts.AR;
using AMESWEB.Enums;
using AMESWEB.Helpers;
using AMESWEB.Hubs;
using AMESWEB.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace AMESWEB.Areas.Account.Controllers
{
    [Area("account")]
    [Authorize]
    public class ArInvoiceController : BaseController
    {
        private readonly ILogger<ArInvoiceController> _logger;
        private readonly IArInvoiceService _arInvoiceService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ArInvoiceController(ILogger<ArInvoiceController> logger, IBaseService baseService, IArInvoiceService arInvoiceService, IHubContext<NotificationHub> hubContext)
            : base(logger, baseService)
        {
            _logger = logger;
            _arInvoiceService = arInvoiceService;
            _hubContext = hubContext;
        }

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

            var permissions = await HasPermission((short)companyId, parsedUserId.Value, (short)E_Modules.AR, (short)E_AR.Invoice);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.IsExport = permissions?.IsExport ?? false;
            ViewBag.IsPrint = permissions?.IsPrint ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> List(int pageNumber, int pageSize, string searchString, string companyId, int customerId, string fromDate, string toDate, bool isShowAll)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _arInvoiceService.GetArInvoiceListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, fromDate, toDate, isShowAll);
                return Json(new { data = data.data, total = data.totalRecords });

                //var total = data.totalRecords;
                //var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                //return Json(new { data = paginatedData, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(string invoiceId, string invoiceNo, string companyId)
        {
            if (string.IsNullOrEmpty(invoiceId))
            {
                return Json(new { success = false, message = "Invalid Invoice ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID" });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            try
            {
                var data = await _arInvoiceService.GetArInvoiceByIdNoAsync(companyIdShort, parsedUserId, Convert.ToInt64(invoiceId), invoiceNo);

                if (data == null)
                {
                    return Json(new { success = false, message = "AR Invoice not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching AR invoice by ID.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveArInvoiceViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var arInvoice = model.ArInvoice;

            if (string.IsNullOrEmpty(model.CompanyId) || !short.TryParse(model.CompanyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var arInvoiceToSave = new ArInvoiceHd
            {
                CompanyId = companyIdShort,
                InvoiceId = Convert.ToInt64(arInvoice.InvoiceId ?? "0"),
                InvoiceNo = arInvoice.InvoiceNo ?? string.Empty,
                ReferenceNo = arInvoice.ReferenceNo ?? string.Empty,
                TrnDate = DateHelperStatic.ParseDBDate(arInvoice.TrnDate),
                AccountDate = DateHelperStatic.ParseDBDate(arInvoice.AccountDate),
                DeliveryDate = DateHelperStatic.ParseDBDate(arInvoice.DeliveryDate),
                DueDate = DateHelperStatic.ParseDBDate(arInvoice.DueDate),
                CustomerId = arInvoice.CustomerId,
                CurrencyId = arInvoice.CurrencyId,
                ExhRate = arInvoice.ExhRate,
                CtyExhRate = arInvoice.CtyExhRate,
                CreditTermId = arInvoice.CreditTermId,
                BankId = arInvoice.BankId,
                TotAmt = arInvoice.TotAmt,
                TotLocalAmt = arInvoice.TotLocalAmt,
                TotCtyAmt = arInvoice.TotCtyAmt,
                GstClaimDate = DateHelperStatic.ParseDBDate(arInvoice.GstClaimDate),
                GstAmt = arInvoice.GstAmt,
                GstLocalAmt = arInvoice.GstLocalAmt,
                GstCtyAmt = arInvoice.GstCtyAmt,
                TotAmtAftGst = arInvoice.TotAmtAftGst,
                TotLocalAmtAftGst = arInvoice.TotLocalAmtAftGst,
                TotCtyAmtAftGst = arInvoice.TotCtyAmtAftGst,
                BalAmt = arInvoice.BalAmt,
                BalLocalAmt = arInvoice.BalLocalAmt,
                PayAmt = arInvoice.PayAmt,
                PayLocalAmt = arInvoice.PayLocalAmt,
                ExGainLoss = arInvoice.ExGainLoss,
                SalesOrderId = Convert.ToInt64(arInvoice.SalesOrderId ?? "0"),
                SalesOrderNo = arInvoice.SalesOrderNo ?? string.Empty,
                OperationId = Convert.ToInt64(arInvoice.OperationId ?? "0"),
                OperationNo = arInvoice.OperationNo ?? string.Empty,
                Remarks = arInvoice.Remarks ?? string.Empty,
                Address1 = arInvoice.Address1 ?? string.Empty,
                Address2 = arInvoice.Address2 ?? string.Empty,
                Address3 = arInvoice.Address3 ?? string.Empty,
                Address4 = arInvoice.Address4 ?? string.Empty,
                PinCode = arInvoice.PinCode ?? string.Empty,
                CountryId = arInvoice.CountryId,
                PhoneNo = arInvoice.PhoneNo ?? string.Empty,
                FaxNo = arInvoice.FaxNo ?? string.Empty,
                ContactName = arInvoice.ContactName ?? string.Empty,
                MobileNo = arInvoice.MobileNo ?? string.Empty,
                EmailAdd = arInvoice.EmailAdd ?? string.Empty,
                ModuleFrom = arInvoice.ModuleFrom ?? string.Empty,
                SupplierName = arInvoice.SupplierName ?? string.Empty,
                SuppInvoiceNo = arInvoice.SuppInvoiceNo ?? string.Empty,
                APInvoiceId = Convert.ToInt64(arInvoice.APInvoiceId ?? "0"),
                APInvoiceNo = arInvoice.APInvoiceNo ?? string.Empty,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = parsedUserId,
                EditDate = DateTime.Now,
            };

            // Details Mapping
            var arInvoiceDtEntities = arInvoice.data_details?.Select(item => new ArInvoiceDt
            {
                InvoiceId = item.InvoiceId != null ? Convert.ToInt64(item.InvoiceId) : 0,
                InvoiceNo = item.InvoiceNo,
                ItemNo = item.ItemNo,
                SeqNo = item.SeqNo,
                DocItemNo = item.DocItemNo,
                ProductId = item.ProductId,
                GLId = item.GLId,
                QTY = item.QTY,
                BillQTY = item.BillQTY,
                UomId = item.UomId,
                UnitPrice = item.UnitPrice,
                TotAmt = item.TotAmt,
                TotLocalAmt = item.TotLocalAmt,
                TotCtyAmt = item.TotCtyAmt,
                Remarks = item.Remarks?.Trim() ?? string.Empty,
                GstId = item.GstId,
                GstPercentage = item.GstPercentage,
                GstAmt = item.GstAmt,
                GstLocalAmt = item.GstLocalAmt,
                GstCtyAmt = item.GstCtyAmt,
                DeliveryDate = item.DeliveryDate == "" ? null : DateHelperStatic.ParseClientDate(item.DeliveryDate),
                DepartmentId = item.DepartmentId,
                EmployeeId = item.EmployeeId,
                PortId = item.PortId,
                VesselId = item.VesselId,
                BargeId = item.BargeId,
                VoyageId = item.VoyageId,
                OperationId = Convert.ToInt64(item.OperationId),
                OperationNo = item.OperationNo?.Trim() ?? string.Empty,
                OPRefNo = item.OPRefNo?.Trim() ?? string.Empty,
                SalesOrderId = Convert.ToInt64(item.SalesOrderId),
                SalesOrderNo = item.SalesOrderNo?.Trim() ?? string.Empty,
                SupplyDate = item.SupplyDate == "" ? null : DateHelperStatic.ParseClientDate(item.SupplyDate),
                SupplierName = item.SupplierName?.Trim() ?? string.Empty,
                SuppInvoiceNo = item.SuppInvoiceNo?.Trim() ?? string.Empty,
                APInvoiceId = Convert.ToInt64(item.APInvoiceId),
                APInvoiceNo = item.APInvoiceNo?.Trim() ?? string.Empty,
                EditVersion = item.EditVersion,
            }).ToList();

            try
            {
                var data = await _arInvoiceService.SaveArInvoiceAsync(companyIdShort, parsedUserId, arInvoiceToSave, arInvoiceDtEntities);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save AR invoice." });
                }

                // 1. Save the invoice (this could be to a database)
                // Example: _invoiceRepository.Add(model);
                // For demonstration, assume the invoice is saved successfully.

                // 2. Send a real-time notification to UserB (the manager)
                // Option A: Send to a specific user (requires proper authentication configuration)
                // await _hubContext.Clients.User("UserBIdentifier")
                //    .SendAsync("ReceiveNotification", "New invoice submitted for approval.");

                // Option B: Send to a group (if all managers join the "Managers" group)
                await _hubContext.Clients.Group("Managers")
               .SendAsync("ReceiveNotification", "New invoice submitted for approval.");

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the AR invoice.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        public IActionResult PreviewInvoice()
        {
            return PartialView("~/Views/Shared/_PreviewInvoice.cshtml");
        }
    }
}