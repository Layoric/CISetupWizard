using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.TeamCityClient.Types;

namespace ServiceStack.TeamCityClient
{
    [Route("/userGroups")]
    public class GetUserGroups : IReturn<GetUserGroupsResponse>
    {

    }

    [DataContract]
    public class GetUserGroupsResponse
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
        [DataMember(Name = "group")]
        public List<Group> Groups { get; set; }
    }

    [Route("/userGroups/{GroupLocator}")]
    public class GetUsersInGroup : IReturn<GetUsersInGroupResponse>
    {
        public string GroupLocator { get; set; }
    }

    [DataContract]
    public class GetUsersInGroupResponse
    {
        [DataMember(Name = "key")]
        public string Key { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "href")]
        public string Href { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "users")]
        public UsersResponse UsersResponse { get; set; }
        [DataMember(Name = "properties")]
        public PropertiesResponse PropertiesResponse { get; set; }
        [DataMember(Name = "parent-groups")]
        public ParentGroupsResponse ParentGroupsResponse { get; set; }
        [DataMember(Name = "child-groups")]
        public ChildGroupsResponse ChildGroupsResponse { get; set; }
        [DataMember(Name = "roles")]
        public RolesResponse RolesResponse { get; set; }
    }
}
