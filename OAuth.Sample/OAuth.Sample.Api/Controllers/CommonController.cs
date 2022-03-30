using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace OAuth.Sample.Api.Controllers
{
    /// <summary>
    /// API資訊
    /// </summary>
    public class CommonController : ControllerBase
    {
        protected int? UserId
        {
            get
            {
                var principal = HttpContext.User;
                var txtUserId = principal?.Claims?.SingleOrDefault(p => p.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

                if (int.TryParse(txtUserId, out int userId))
                {
                    return userId;
                }

                return null;
            }
        }
    }
}

