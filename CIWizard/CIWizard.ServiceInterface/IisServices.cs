using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;
using ServiceStack;

namespace CIWizard.ServiceInterface
{
    public class IisServices : Service
    {
        public object Get(GetIisSiteSettings request)
        {
            var result = new GetIisSiteSettingsResponse();
            using (var sm = new ServerManager())
            {
                var site = sm.Sites["{0}_{1}".Fmt(request.OwnerName, request.RepoName)];
                if (site == null)
                    throw HttpError.NotFound("SiteNotFound");

                if (site.Bindings.Count > 0)
                {
                    var binding = site.Bindings.First();
                    string bi = binding.BindingInformation;
                    result.BindingInfo = bi;
                    result.Host = bi.Substring(bi.LastIndexOf(":", StringComparison.Ordinal) + 1);
                }
            }
            return result;
        }
    }

    [Route("/user/projects/{OwnerName}/{RepoName}/iis")]
    public class GetIisSiteSettings : IReturn<GetIisSiteSettingsResponse>
    {
        public string OwnerName { get; set; }
        public string RepoName { get; set; }
    }

    public class GetIisSiteSettingsResponse
    {
        public string Host { get; set; }
        public string BindingInfo { get; set; }
    }
}
