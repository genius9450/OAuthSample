using OAuth.Sample.EF.Entity;
using OAuth.Sample.Schedule.Interface;
using OAuth.Sample.Service.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace OAuth.Sample.Schedule.Process
{
    /// <summary>
    /// 排程-測試塞Log
    /// </summary>
    public class AddLogProcess : IProcess
    {
        public ILogger<AddLogProcess> Logger { get; set; }

        public async Task Main()
        {
            await Task.Delay(0);
            Logger.LogInformation("Schedule / {Process}", "AddLogProcess");
        }
    }
}

