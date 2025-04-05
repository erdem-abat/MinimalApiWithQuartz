using Microsoft.Extensions.Options;
using Quartz;

namespace Infrastructure
{
    public class GetDataBackgroundJobSetup : IConfigureOptions<QuartzOptions>
    {
        public void Configure(QuartzOptions options)
        {
            var jobKey = JobKey.Create(nameof(GetDataBackgroundJob));

            options
            .AddJob<GetDataBackgroundJob>(JobBuilder => JobBuilder.WithIdentity(jobKey))
            .AddTrigger(trigger =>
                trigger
                    .ForJob(jobKey)
                    //.WithCronSchedule("*/1 * * * *")
                    .WithSimpleSchedule(schedule =>
                    schedule.WithIntervalInSeconds(5).RepeatForever()));
        }
    }
}
