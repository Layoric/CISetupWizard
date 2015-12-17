using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIWizard.ServiceModel.Types;
using ServiceStack;

namespace CIWizard.ServiceModel
{
    [Route("/user/projects/{OwnerName}/{RepositoryName}")]
    public class GetTeamCityProjectDetails : IReturn<GetTeamCityProjectDetailsResponse>
    {
        public string RepositoryName { get; set; }
        public string OwnerName { get; set; }

        public string ProjectId
        {
            get { return "SS_" + OwnerName + "_" + RepositoryName; }
        }
    }

    public class GetTeamCityProjectDetailsResponse
    {
        public string BuildNumber { get; set; }
        public string BuildStatus { get; set; }
        public string BuildState { get; set; }
        public string ProjectName { get; set; }
        public string ProjectId { get; set; }
    }

    [Route("/user/projects")]
    public class GetAllGeneratedTeamCityProjects : IReturn<GetAllGeneratedTeamCityProjectsResponse>
    {

    }

    public class GetAllGeneratedTeamCityProjectsResponse
    {
        public List<GitHubRepository> Projects { get; set; }
    }
}
