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

    [Route("/users/{UserLocator}")]
    public class GetUser : IReturn<GetUserResponse>
    {
        public string UserLocator { get; set; }
    }

    [DataContract]
    public class GetUserResponse
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "lastLogin")]
        public string LastLogin { get; set; }
        [DataMember(Name = "href")]
        public string Href { get; set; }
        [DataMember(Name = "properties")]
        public PropertiesResponse PropertyDetails { get; set; }
        [DataMember(Name = "roles")]
        public RolesResponse RoleDetails { get; set; }
        [DataMember(Name = "groups")]
        public GroupsResponse GroupDetails { get; set; }
    }
}
