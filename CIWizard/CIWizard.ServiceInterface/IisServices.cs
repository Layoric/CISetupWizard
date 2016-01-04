using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace CIWizard.ServiceInterface
{
    public class IisServices : Service
    {

    }

    public class GetIisSiteSettings
    {
        public string OwnerName { get; set; }
        public string RepoName { get; set; }
    }
}
