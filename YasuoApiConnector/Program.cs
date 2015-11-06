using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;
using System.Timers;
using System.Threading;
using Quartz;
using Quartz.Impl;
namespace YasuoApiConnector
{
    class Program
    {
        static void Main(string[] args)
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();


            scheduler.Start();

            IJobDetail pollingJob = JobBuilder.Create<PollingJob>().WithIdentity("PollingExactTarget", "GroupOne").Build();
            ITrigger trigger= TriggerBuilder.Create().WithIdentity("TriggerOne", "GroupOne").StartNow().WithSimpleSchedule(x => x.WithIntervalInMinutes(60).RepeatForever()).Build();

            scheduler.ScheduleJob(pollingJob, trigger);
            Thread.Sleep(TimeSpan.FromSeconds(60));


            


        }

   

    }
}
