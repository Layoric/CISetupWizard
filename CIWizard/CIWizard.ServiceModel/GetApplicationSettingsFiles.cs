using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace CIWizard.ServiceModel
{
    [Route("/user/projects/{OwnerName}/{RepositoryName}/files")]
    public class GetApplicationSettingsFiles
    {
        public string OwnerName { get; set; }
        public string RepositoryName { get; set; }
    }

    public class GetApplicationSettingsFilesResponse
    {
        public List<string> FileNames { get; set; } 
    }
}
