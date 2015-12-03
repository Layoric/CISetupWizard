using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace CIWizard.ServiceModel
{
    [Route("/user/build", Verbs = "POST")]
    public class CreateSpaBuildProject : IReturn<CreateSpaBuildProjectResponse>
    {
        public string Name { get; set; }
        public string RepositoryUrl { get; set; }
        public string SolutionPath { get; set; }
        public ServiceStackTemplateType TemplateType { get; set; }
        public bool PrivateRepository { get; set; }
        public string Branch { get; set; }
        public string WorkingDirectory { get; set; }
        public string ProjectName { get; set; }
    }

    public class CreateSpaBuildProjectResponse
    {
        
    }
}
