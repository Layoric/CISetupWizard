using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace CIWizard.ServiceModel
{
    [Route("/user/builds/{OwnerName}/{RepositoryName}")]
    public class GetBuildStatus
    {
        public string RepositoryName { get; set; }
        public string OwnerName { get; set; }

        public string ProjectId
        {
            get { return "SS_" + OwnerName + "_" + RepositoryName; }
        }
    }

    public class GetBuildStatusResponse
    {
        public string Status { get; set; }
        public DateTime? LastUpdate { get; set; }
    }

    [Route("/user/builds")]
    public class GetAllBuildStatuses : IReturn<GetAllBuildStatusesResponse>
    {
        
    }

    public class GetAllBuildStatusesResponse
    {
        
    }
}
