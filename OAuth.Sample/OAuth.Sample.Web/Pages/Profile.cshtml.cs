using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OAuth.Sample.Web.Pages
{
    public class OAuthSetting
    {
        public string ProviderType { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }
    }
    public class ProfileModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
