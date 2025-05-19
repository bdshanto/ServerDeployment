using System.Globalization;

namespace ServerDeployment.infrastructure.DataAccessor;

public abstract class DateTimeConstant
{
    public const int InvalidYear = 0001;
    public const string CsvValidDateFormatOne = "dd/MM/yyyy";
    public const string CsvValidDateFormatTwo = "yyyyMM";
    public const string GovtFileDateFormat = "MM/yyyy";
    public static DateTime EMPTY_DATE = new(1900, 1, 1, 0, 0, 0, 0);
    public const string JerseyDatetimeFormat = "yyyy'-'MM'-'dd HH':'mm':'ss";
    public const string JerseyDateFormat = "yyyy-MM-dd hh:mm:ss tt";

    // set culture info for date time format yyyy-MM-dd HH:mm:ss
    public static CultureInfo JERSEY_CULTURE_INFO = new("en-US", false);
}