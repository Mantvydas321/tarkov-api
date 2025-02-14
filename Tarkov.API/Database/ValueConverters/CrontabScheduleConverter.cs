using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NCrontab;

namespace Tarkov.API.Database.ValueConverters;

public class CrontabScheduleConverter : ValueConverter<CrontabSchedule, string>
{
    public CrontabScheduleConverter() : base(
        status => status.ToString(),
        value => CrontabSchedule.Parse(value, new CrontabSchedule.ParseOptions() { IncludingSeconds = false })
    )
    {
    }
}