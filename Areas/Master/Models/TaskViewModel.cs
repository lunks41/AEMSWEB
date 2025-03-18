namespace AEMSWEB.Models.Masters
{
    public class TaskViewModel
    {
        public Int32 TaskId { get; set; }
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public byte TaskOrder { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveTaskViewModel
    {
        public TaskViewModel Task { get; set; }
        public string CompanyId { get; set; }
    }

    public class TaskViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<TaskViewModel> data { get; set; }
    }
}