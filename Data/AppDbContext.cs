using AMESWEB.Areas.HRM.Models;
using AMESWEB.Entities.Accounts.AP;
using AMESWEB.Entities.Accounts.AR;
using AMESWEB.Entities.Accounts.CB;
using AMESWEB.Entities.Accounts.GL;
using AMESWEB.Entities.Admin;
using AMESWEB.Entities.Masters;
using AMESWEB.Entities.Project;
using AMESWEB.Entities.Setting;
using AMESWEB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AMESWEB.Data
{
    public class ApplicationDbContext : IdentityDbContext<AdmUser, AdmUserGroup, short>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename the Identity tables
            builder.Entity<AdmUser>().ToTable("AdmUser");
            builder.Entity<AdmUserGroup>().ToTable("AdmUserGroup");

            // Optional: Rename other Identity tables (if needed)
            builder.Entity<IdentityUserClaim<short>>().ToTable("AdmUserClaim");
            builder.Entity<IdentityUserLogin<short>>().ToTable("AdmUserLogin");
            builder.Entity<IdentityUserToken<short>>().ToTable("AdmUserToken");
            builder.Entity<IdentityRoleClaim<short>>().ToTable("AdmRoleClaim");
            builder.Entity<IdentityUserRole<short>>().ToTable("AdmUserRole");

            // Configure AdmUserRights composite primary key
            builder.Entity<AdmUserRights>()
                .HasKey(ur => new { ur.CompanyId, ur.UserId });

            // Map UserGroup's Name property to UserGroupName column
            builder.Entity<AdmUserGroup>(b =>
            {
                b.Property(r => r.UserGroupCode).HasMaxLength(50).IsRequired();
                // Map Identity's NormalizedName if needed
                b.Property(r => r.NormalizedName).HasColumnName("NormalizedName");
            });

            // Map AdmUser's Email to UserEmail
            builder.Entity<AdmUser>(b =>
            {
                b.Property(u => u.UserCode).HasMaxLength(50).IsRequired();
                b.Property(u => u.Email).HasColumnName("UserEmail");
            });

            // Additional configurations if needed
        }

        public DbSet<AdmCompany> AdmCompany { get; set; }
        public DbSet<AdmUserRights> AdmUserRights { get; set; }
        public DbSet<AdmModule> AdmModule { get; set; }
        public DbSet<AdmTransactionCategory> AdmTransactionCategory { get; set; }
        public DbSet<AdmTransaction> AdmTransaction { get; set; }
        public DbSet<AdmUserGroupRights> AdmUserGroupRights { get; set; }

        #region Admin

        public DbSet<AdmAuditLog> AdmAuditLog { get; set; }
        public DbSet<AdmErrorLog> AdmErrorLog { get; set; }

        public DbSet<AdmShareData> AdmShareData { get; set; }

        public DbSet<AdmUser> AdmUser { get; set; }
        public DbSet<AdmUserGroup> AdmUserGroup { get; set; }

        public DbSet<AdmUserLog> AdmUserLog { get; set; }
        public DbSet<AdmDocuments> AdmDocuments { get; set; }

        #endregion Admin

        #region Masters

        public DbSet<M_AccountGroup> M_AccountGroup { get; set; }
        public DbSet<M_DocumentType> M_DocumentType { get; set; }
        public DbSet<M_AccountType> M_AccountType { get; set; }
        public DbSet<M_Product> M_Product { get; set; }
        public DbSet<M_Country> M_Country { get; set; }
        public DbSet<M_AccountSetup> M_AccountSetup { get; set; }
        public DbSet<M_AccountSetupDt> M_AccountSetupDt { get; set; }
        public DbSet<M_AccountSetupCategory> M_AccountSetupCategory { get; set; }
        public DbSet<M_Bank> M_Bank { get; set; }
        public DbSet<M_BankAddress> M_BankAddress { get; set; }
        public DbSet<M_BankContact> M_BankContact { get; set; }
        public DbSet<M_Barge> M_Barge { get; set; }
        public DbSet<M_Category> M_Category { get; set; }
        public DbSet<M_ChartOfAccount> M_ChartOfAccount { get; set; }
        public DbSet<M_COACategory1> M_COACategory1 { get; set; }
        public DbSet<M_COACategory2> M_COACategory2 { get; set; }
        public DbSet<M_COACategory3> M_COACategory3 { get; set; }
        public DbSet<M_CreditTerm> M_CreditTerm { get; set; }
        public DbSet<M_CreditTermDt> M_CreditTermDt { get; set; }
        public DbSet<M_Currency> M_Currency { get; set; }
        public DbSet<M_CurrencyDt> M_CurrencyDt { get; set; }
        public DbSet<M_CurrencyLocalDt> M_CurrencyLocalDt { get; set; }
        public DbSet<M_CustomerGroupCreditLimit> M_CustomerGroupCreditLimit { get; set; }
        public DbSet<M_Customer> M_Customer { get; set; }
        public DbSet<M_CustomerAddress> M_CustomerAddress { get; set; }
        public DbSet<M_CustomerContact> M_CustomerContact { get; set; }
        public DbSet<M_CustomerCreditLimit> M_CustomerCreditLimit { get; set; }
        public DbSet<M_Department> M_Department { get; set; }
        public DbSet<M_Designation> M_Designation { get; set; }
        public DbSet<M_Employee> M_Employee { get; set; }
        public DbSet<M_GroupCreditLimit> M_GroupCreditLimit { get; set; }
        public DbSet<M_GroupCreditLimit_Customer> M_GroupCreditLimit_Customer { get; set; }
        public DbSet<M_GroupCreditLimitDt> M_GroupCreditLimitDt { get; set; }
        public DbSet<M_Gst> M_Gst { get; set; }
        public DbSet<M_GstCategory> M_GstCategory { get; set; }
        public DbSet<M_GstDt> M_GstDt { get; set; }
        public DbSet<M_OrderType> M_OrderType { get; set; }
        public DbSet<M_OrderTypeCategory> M_OrderTypeCategory { get; set; }
        public DbSet<M_PaymentType> M_PaymentType { get; set; }
        public DbSet<M_Port> M_Port { get; set; }
        public DbSet<M_PortRegion> M_PortRegion { get; set; }
        public DbSet<M_SubCategory> M_SubCategory { get; set; }
        public DbSet<M_Supplier> M_Supplier { get; set; }
        public DbSet<M_SupplierAddress> M_SupplierAddress { get; set; }
        public DbSet<M_SupplierContact> M_SupplierContact { get; set; }
        public DbSet<M_SupplierBank> M_SupplierBank { get; set; }
        public DbSet<M_Uom> M_Uom { get; set; }
        public DbSet<M_UomDt> M_UomDt { get; set; }
        public DbSet<M_Vessel> M_Vessel { get; set; }
        public DbSet<M_Voyage> M_Voyage { get; set; }
        public DbSet<M_Task> M_Task { get; set; }
        public DbSet<M_Charges> M_Charges { get; set; }

        #endregion Masters

        #region Setting

        public DbSet<S_DecSettings> S_DecSettings { get; set; }
        public DbSet<S_FinSettings> S_FinSettings { get; set; }
        public DbSet<S_UserSettings> S_UserSettings { get; set; }
        public DbSet<S_NumberFormat> S_NumberFormat { get; set; }
        public DbSet<S_MandatoryFields> S_MandatoryFields { get; set; }
        public DbSet<S_VisibleFields> S_VisibleFields { get; set; }
        public DbSet<S_DynamicLookup> S_DynamicLookup { get; set; }
        public DbSet<S_UserGrdFormat> S_UserGrdFormat { get; set; }

        #endregion Setting

        #region Account

        #region AR

        public DbSet<ArInvoiceHd> ArInvoiceHd { get; set; }
        public DbSet<ArInvoiceDt> ArInvoiceDt { get; set; }
        public DbSet<ArCreditNoteHd> ArCreditNoteHd { get; set; }
        public DbSet<ArCreditNoteDt> ArCreditNoteDt { get; set; }
        public DbSet<ArDebitNoteHd> ArDebitNoteHd { get; set; }
        public DbSet<ArDebitNoteDt> ArDebitNoteDt { get; set; }
        public DbSet<ArReceiptHd> ArReceiptHd { get; set; }
        public DbSet<ArReceiptDt> ArReceiptDt { get; set; }
        public DbSet<ArRefundHd> ArRefundHd { get; set; }
        public DbSet<ArRefundDt> ArRefundDt { get; set; }
        public DbSet<ArAdjustmentHd> ArAdjustmentHd { get; set; }
        public DbSet<ArAdjustmentDt> ArAdjustmentDt { get; set; }
        public DbSet<ArDocSetOffHd> ArDocSetOffHd { get; set; }
        public DbSet<ArDocSetOffDt> ArDocSetOffDt { get; set; }

        #endregion AR

        #region AP

        public DbSet<ApInvoiceHd> ApInvoiceHd { get; set; }
        public DbSet<ApInvoiceDt> ApInvoiceDt { get; set; }
        public DbSet<ApCreditNoteHd> ApCreditNoteHd { get; set; }
        public DbSet<ApCreditNoteDt> ApCreditNoteDt { get; set; }
        public DbSet<ApDebitNoteHd> ApDebitNoteHd { get; set; }
        public DbSet<ApDebitNoteDt> ApDebitNoteDt { get; set; }
        public DbSet<ApAdjustmentHd> ApAdjustmentHd { get; set; }
        public DbSet<ApAdjustmentDt> ApAdjustmentDt { get; set; }
        public DbSet<ApPaymentHd> ApPaymentHd { get; set; }
        public DbSet<ApPaymentDt> ApPaymentDt { get; set; }
        public DbSet<ApRefundHd> ApRefundHd { get; set; }
        public DbSet<ApRefundDt> ApRefundDt { get; set; }
        public DbSet<ApDocSetOffHd> ApDocSetOffHd { get; set; }
        public DbSet<ApDocSetOffDt> ApDocSetOffDt { get; set; }

        #endregion AP

        #region CB

        public DbSet<CBGenPaymentHd> CBGenPaymentHd { get; set; }
        public DbSet<CBGenPaymentDt> CBGenPaymentDt { get; set; }
        public DbSet<CBGenReceiptHd> CBGenReceiptHd { get; set; }
        public DbSet<CBGenReceiptDt> CBGenReceiptDt { get; set; }
        public DbSet<CBPettyCashHd> CBPettyCashHd { get; set; }
        public DbSet<CBPettyCashDt> CBPettyCashDt { get; set; }
        public DbSet<CBBankReconHd> CBBankReconHd { get; set; }
        public DbSet<CBBankReconDt> CBBankReconDt { get; set; }
        public DbSet<CBBankTransfer> CBBankTransfer { get; set; }

        #endregion CB

        #region GL

        public DbSet<GLPeriodClose> GLPeriodClose { get; set; }
        public DbSet<GLOpeningBalance> GLOpeningBalance { get; set; }
        public DbSet<GLJournalHd> GLJournalHd { get; set; }
        public DbSet<GLJournalDt> GLJournalDt { get; set; }
        public DbSet<GLContraHd> GLContraHd { get; set; }
        public DbSet<GLContraDt> GLContraDt { get; set; }

        #endregion GL

        #endregion Account

        #region Project

        //public DbSet<Ser_JobOrderHd> Ser_JobOrderHd { get; set; }
        //public DbSet<Ser_JobOrderDt> Ser_JobOrderDt { get; set; }
        public DbSet<Ser_Tariff> Ser_Tariff { get; set; }

        public DbSet<Ser_PortExpenses> Ser_PortExpenses { get; set; }
        public DbSet<Ser_LaunchService> Ser_LaunchService { get; set; }
        public DbSet<Ser_EquipmentsUsed> Ser_EquipmentsUsed { get; set; }
        public DbSet<Ser_CrewSignOn> Ser_CrewSignOn { get; set; }
        public DbSet<Ser_CrewSignOff> Ser_CrewSignOff { get; set; }
        public DbSet<Ser_CrewMiscellaneous> Ser_CrewMiscellaneous { get; set; }
        public DbSet<Ser_MedicalAssistance> Ser_MedicalAssistance { get; set; }
        public DbSet<Ser_ConsignmentImport> Ser_ConsignmentImport { get; set; }
        public DbSet<Ser_ConsignmentExport> Ser_ConsignmentExport { get; set; }
        public DbSet<Ser_ThirdParty> Ser_ThirdParty { get; set; }
        public DbSet<Ser_FreshWater> Ser_FreshWater { get; set; }
        public DbSet<Ser_TechnicianSurveyor> Ser_TechnicianSurveyor { get; set; }
        public DbSet<Ser_LandingItems> Ser_LandingItems { get; set; }
        public DbSet<Ser_OtherService> Ser_OtherService { get; set; }
        public DbSet<Ser_AgencyRemuneration> Ser_AgencyRemuneration { get; set; }

        #endregion Project

        #region HRMS

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }

        #endregion HRMS
    }
}