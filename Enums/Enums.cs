// Ignore Spelling: Admin

namespace AEMSWEB.Enums
{
    public enum E_Mode
    {
        Create = 1,
        Update = 2,
        Delete = 3,
        View = 4,
        Lookup = 5,
        Login = 6,
    }

    public enum E_Modules
    {
        Master = 1,
        Sales = 2,
        Purchase = 3,
        AR = 25,
        AP = 26,
        CB = 27,
        GL = 28,
        Setting = 99,
        Admin = 100,
        Project = 5
    }
    public enum E_Project
    {
        Job = 1,
        Report = 2,
        Department = 3,
        Tariff = 4
    }

    public enum E_Master
    {
        Country = 1,
        Currency = 2,
        Department = 3,
        COACategory1 = 5,
        COACategory2 = 6,
        COACategory3 = 7,
        ChartOfAccount = 8,
        CreditTerm = 9,
        Uom = 10,
        Employee = 11,
        Bank = 12,
        Designation = 13,
        Port = 14,
        PortRegion = 15,
        Tax = 16,
        TaxCategory = 17,
        Barge = 18,
        Vessel = 19,
        OrderType = 20,
        OrderTypeCategory = 21,
        CreditTermDt = 22,
        CurrencyDt = 23,
        CurrencyLocalDt = 24,
        Voyage = 25,
        Customer = 26,
        Supplier = 27,
        Gst = 28,
        GstCategory = 29,
        AccountSetup = 30,
        Product = 31,
        UomDt = 32,
        GstDt = 33,
        TaxDt = 34,
        Category = 35,
        SubCategory = 36,
        GroupCreditLimit = 37,
        CustomerGroupCreditLimit = 38,
        AccountSetupCategory = 39,
        PaymentType = 40,
        AccountType = 41,
        AccountGroup = 42,
        CustomerCreditLimit = 43,
        GroupCreditLimit_Customer = 44,
        SupplierContact = 45,
        AccountSetupDt = 46,
        DocumentType = 47,
        Task = 48,
        Charges = 49
    }

    public enum E_AR
    {
        Invoice = 1,
        DebitNote = 2,
        CreditNote = 3,
        Adjustment = 4,
        Receipt = 5,
        Refund = 6,
        DocSetoff = 7,
        Reports = 99,
    }

    public enum E_AP
    {
        Invoice = 1,
        DebitNote = 2,
        CreditNote = 3,
        Adjustment = 4,
        Payment = 5,
        Refund = 6,
        DocSetoff = 7,
        Reports = 99,
    }

    public enum E_CB
    {
        CBReceipt = 1,
        CBPayment = 2,
        CBPattyCash = 3,
        CBBankTransfer = 4,
        CBBankRecon = 5,
        Reports = 99,
    }

    public enum E_GL
    {
        JournalEntry = 1,
        ArApContra = 2,
        FixedAsset = 3,
        OpeningBalance = 4,
        YearEndProcess = 5,
        PeriodClose = 6,
        Reports = 99,
    }

    public enum E_Admin
    {
        User = 1,
        UserRights = 2,
        UserGroup = 3,
        UserGroupRights = 4,
        DocumentNo = 5,
        Modules = 6,
        Transaction = 7,
        Document = 8,
        Company = 8
    }

    public enum E_Setting
    {
        GridSetting = 1,
        DocumentNo = 2,
        DecSetting = 3,
        FinSetting = 4,
        MandatoryFields = 5,
        VisibleFields = 6,
        DynamicLookup = 7,
        DocSeqNo = 8,
        UserSetting = 9
    }

    public enum E_Task
    {
        PortExpenses = 1,
        LaunchServices = 2,
        EquipmentsUsed = 3,
        CrewSignOn = 4,
        CrewSignOff = 5,
        CrewMiscellaneous = 6,
        MedicalAssistance = 7,
        ConsignmentImport = 8,
        ConsignmentExport = 9,
        ThirdPartySupply = 10,
        FreshWaterSupply = 11,
        TechniciansSurveyors = 12,
        LandingItems = 13,
        OtherService = 14,
        AgencyRemuneration = 15
    }

    public enum OrderTypeCategory
    {
        VisaType = 1,
        AddressType = 2,
        GenderType = 3,
        StatusType = 4,
        BerthType = 5,
        ModeType = 6,
        JobType = 7,
        ConsignmentType = 8,
        CIDClearanceType = 9,
        PassType = 10,
        CarrierType = 11,
        LocationType = 12,
        CalculateType = 13
    }
}