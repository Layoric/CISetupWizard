using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIWizard.ServiceModel.Types;
using ServiceStack;

namespace CIWizard.ServiceModel
{
    [Route("/user/repos")]
    public class GetGitHubRepositories : IReturn<GetGitHubRepositoryResponse>
    {
        public string PerPage { get; set; }
    }

    public class GetGitHubRepositoryResponse
    {
        public GitHubRepository Repo { get; set; }
    }
}
