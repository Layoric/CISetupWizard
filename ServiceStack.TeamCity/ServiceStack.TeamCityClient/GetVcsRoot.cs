using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.TeamCityClient.Types;

namespace ServiceStack.TeamCityClient
{
    [Route("/vcs-roots")]
    public class GetVcsRoots : IReturn<GetVcsRootsResponse>
    {

    }

    [DataContract]
    public class GetVcsRootsResponse
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
        [DataMember(Name = "href")]
        public string Href { get; set; }
        [DataMember(Name = "vcs-root")]
        public List<VcsRoot> VcsRoots { get; set; }
}
}
