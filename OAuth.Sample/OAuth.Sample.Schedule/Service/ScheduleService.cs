using OAuth.Sample.Schedule.Interface;
using OAuth.Sample.Schedule.Process;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth.Sample.Schedule.Service
{
    public class ScheduleService : IScheduleService
    {
        /// <summary>
        /// 註冊排程
        /// </summary>
        public void Start()
        {
            // 測試塞Log
            AddSingleLaunchJob<SingleLaunchProcess>();

            // 測試每分鐘塞Log
            AddRecurringJob<AddLogProcess>(Cron.MinuteInterval(1), TimeZoneInfo.Local);
        }

        /// <summary>
        /// 註冊RecurringJob
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cron"></param>
        /// <param name="ts"></param>
        private void AddRecurringJob<T>(string cron, TimeZoneInfo ts) where T : IProcess
        {
            var taskName = typeof(T).GetType().Name + cron;
            RecurringJob.RemoveIfExists(taskName);  // 清除Job
            RecurringJob.AddOrUpdate<T>(taskName, (x) => x.Main(), cron, ts);
        }

        /// <summary>
        /// 註冊一次性Job
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cron"></param>
        /// <param name="ts"></param>
        private void AddSingleLaunchJob<T>() where T : IProcess
        {
            BackgroundJob.Enqueue<T>((x) => x.Main());
        }

    }
}

