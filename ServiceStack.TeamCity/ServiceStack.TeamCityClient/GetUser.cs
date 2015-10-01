using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.TeamCityClient.Types;

namespace ServiceStack.TeamCityClient
{
    [Route("/users")]
    public class GetUsers : IReturn<GetUsersResponse>
    {

    }

    [DataContract]
    public class GetUsersResponse
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
        
        [DataMember(Name = "user")]
        public List<User> Users { get; set; } 
    }
}
