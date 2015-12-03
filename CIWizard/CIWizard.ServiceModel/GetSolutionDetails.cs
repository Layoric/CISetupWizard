using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace CIWizard.ServiceModel
{
    [Route("/user/repos/{OwnerName}/{RepositoryName}/solution")]
    public class GetSolutionDetails : IReturn<GetSolutionFilePathResponse>
    {
        public string RepositoryName { get; set; }
        public string OwnerName { get; set; }
        public string Branch { get; set; }
    }

    public class GetSolutionFilePathResponse
    {
        public string SolutionPath { get; set; }
        public string Branch { get; set; }
        public string RepositoryUrl { get; set; }
        public string ProjectWorkingDirectory { get; set; }
        public ServiceStackTemplateType TemplateType { get; set; }
        public string ProjectName { get; set; }
        public bool PrivateRepository { get; set; }
    }

    public enum ServiceStackTemplateType
    {
        Unknown,
        AngularSpa,
        ReactSpa,
        ReactDesktop,
        Web,
        SelfHost
    }
}
