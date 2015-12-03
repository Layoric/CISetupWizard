using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace CIWizard.ServiceModel
{
    [Route("/user/builds/{ProjectId}")]
    public class GetBuildStatus
    {
        public string ProjectId { get; set; }
    }

    public class GetBuildStatusResponse
    {
        public string Status { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    [Route("/user/builds")]
    public class GetAllBuildStatuses : IReturn<GetAllBuildStatusesResponse>
    {
        
    }

    public class GetAllBuildStatusesResponse
    {
        
    }
}
