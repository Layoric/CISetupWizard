using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIWizard.ServiceModel.Types;
using ServiceStack;

namespace CIWizard.ServiceModel
{
    [Route("/user/repos/{OwnerName}/{RepositoryName}")]
    public class GetGitHubRepository
    {
        public string RepositoryName { get; set; }
        public string OwnerName { get; set; }
    }

    public class GetGitHubRepositoriesResponse
    {
        public List<GitHubRepository> Repos { get; set; }
    }
}
