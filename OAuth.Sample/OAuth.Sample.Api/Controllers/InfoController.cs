using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OAuth.Sample.Api.Controllers
{
    /// <summary>
    /// API資訊
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        public object Get()
        {

            dynamic resultOutput = new
            {
                Environment = Const.EnvironmentName,
                Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString(),
                HostName = Dns.GetHostName(),
                HostIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork).ToString(),
            };

            return resultOutput;
        }
    }
}

