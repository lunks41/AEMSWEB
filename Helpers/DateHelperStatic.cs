namespace AEMSWEB.Helpers
{
    public static class DateHelperStatic
    {
        //public static string FormatDate(DateTime date, DateHelperNonStatic dateHelperNonStatic)
        //{
        //    // Call the non-static FormatDate method on the DateHelper instance
        //    return dateHelperNonStatic.GetFormattedDate(date);
        //}

        // Static method to parse a date string into a DateTime object
        public static DateTime ParseClientDate(string dateString)
        {
            string[] formats = { "yyyy-MM-dd", "yyyy-dd-MM", "yyyy/MM/dd", "yyyy-MMM-dd", "yyyy/MMM/dd", "dd-MMM-yyyy", "dd/MMM/yyyy", "MM-dd-yyyy", "MM/dd/yyyy", "yyyy-MM-ddThh:mm:ss", "yyyy/MM/ddThh:mm:ss", "dd-MM-yyyy hh:mm:ss tt", "dd/MM/yyyy hh:mm:ss tt", "MM-dd-yyyy hh:mm:ss", "MM/dd/yyyy hh:mm:ss" };

            if (DateTime.TryParseExact(dateString, formats,
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None,
                                       out DateTime date))
            {
                return date;
            }
            else
            {
                throw new FormatException("Invalid date format. Please use one of the supported formats.");
            }
        }

        // Static method to parse a date string into a DateTime object
        public static DateTime ParseDBDate(string dateString)
        {
            string[] formats = { "yyyy-MM-dd", "yyyy-dd-MM", "yyyy/MM/dd", "yyyy-MMM-dd", "yyyy/MMM/dd", "dd-MMM-yyyy", "dd/MMM/yyyy", "MM-dd-yyyy", "MM/dd/yyyy", "yyyy-MM-ddThh:mm:ss", "yyyy/MM/ddThh:mm:ss", "dd-MM-yyyy hh:mm:ss tt", "dd/MM/yyyy hh:mm:ss tt", "MM-dd-yyyy hh:mm:ss", "MM/dd/yyyy hh:mm:ss", "MM/dd/yyyy HH:mm:ss" };

            if (DateTime.TryParseExact(dateString, formats,
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None,
                                       out DateTime date))
            {
                return date;
            }
            else
            {
                throw new FormatException("Invalid date format. Please use one of the supported formats.");
            }
        }

        // Static method to format a DateTime object into a string
        public static string FormatDate(DateTime date)
        {
            return date.ToString("yyyy-MMM-dd");
        }
    }
}