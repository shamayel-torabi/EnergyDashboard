
namespace Infrastructure.Common.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
        //public DateTime Now
        //{
        //    get
        //    {
        //        var timezone = TimeZoneInfo.FindSystemTimeZoneById("Iran");
        //        var utcDate = DateTime.UtcNow;
        //        var iranDate = TimeZoneInfo.ConvertTime(utcDate, timezone);
        //        return iranDate;
        //    }
        //}
    }
}
