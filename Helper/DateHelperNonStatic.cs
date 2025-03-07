namespace AEMSWEB.Helper
{
    public class DateHelperNonStatic
    {
        private readonly IConfiguration _configuration;

        public DateHelperNonStatic(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Static method to parse a date string into a DateTime object
        public DateTime ParseClientDate(string dateString)
        {
            //string formats = "yyyy-MMM-dd";
            string formats = _configuration.GetSection("DateFormatSettings").Value;

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
        public string GetFormattedDate(DateTime date)
        {
            string formats = _configuration.GetSection("DateFormatSettings").Value;

            //return date.ToString("yyyy-MMM-dd");
            return date.ToString(formats);
        }
    }
}