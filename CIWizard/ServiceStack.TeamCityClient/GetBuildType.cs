using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.TeamCityClient.Types;

namespace ServiceStack.TeamCityClient
{
    [Route("/buildTypes/{BuildTypeLocator}")]
    public class GetBuildType : IReturn<GetBuildTypeResponse>
    {
        public string BuildTypeLocator { get; set; }
    }

    [DataContract]
    public class GetBuildTypeResponse : BuildTypeResponse
    {
        
    }
}
