namespace AMESWEB.Models
{
    public static class SqlErrorCodes
    {
        public const int PrimaryKeyViolation = 2627;
        public const string PrimaryKeyViolationMessage = "Violation of primary key constraint.";

        public const int UniqueConstraintViolation = 2601;
        public const string UniqueConstraintViolationMessage = "Violation of unique constraint.";

        public const int ForeignKeyViolation = 547;
        public const string ForeignKeyViolationMessage = "Foreign key constraint violation. \r\n Check constraint violation.\r\n Default constraint violation.";

        public const int DeadlockVictim = 1205;
        public const string DeadlockVictimMessage = "Transaction (Process ID) was deadlocked on resources.";

        public const int Timeout = -2;
        public const string TimeoutMessage = "Query timeout.";

        public const int LoginFailed = 18456;
        public const string LoginFailedMessage = "Login failed for user.";

        public const int SyntaxError = 156;
        public const string SyntaxErrorMessage = "Incorrect syntax.";

        public const int InvalidColumnName = 207;
        public const string InvalidColumnNameMessage = "Invalid column name.";

        public const int InvalidObjectName = 208;
        public const string InvalidObjectNameMessage = "Invalid object name.";
    }
}