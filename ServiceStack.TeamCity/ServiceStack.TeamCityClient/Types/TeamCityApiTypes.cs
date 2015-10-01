using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace ServiceStack.TeamCityClient.Types
{
    [DataContract]
    public class Project
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "href")]
        public string Href { get; set; }
        [DataMember(Name = "webUrl")]
        public string WebUrl { get; set; }
        [DataMember(Name = "parentProjectId")]
        public string ParentProjectId { get; set; }
    }

    [DataContract]
    public class VcsRoot
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "href")]
        public string Href { get; set; }
    }

    [DataContract]
    public class BuildTypesResponse
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
        //TODO
        [DataMember(Name = "buildType")]
        public List<object> BuildTypes { get; set; }
    }

    [DataContract]
    public class TemplatesResponse
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
        //TODO
        [DataMember(Name = "buildType")]
        public List<object> BuildTypes { get; set; }
    }

    public class ParametersResponse
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
        [DataMember(Name = "href")]
        public string Href { get; set; }
        //TODO
        [DataMember(Name = "property")]
        public List<object> Properties { get; set; }
    }

    public class VcsRootsResponse
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
        [DataMember(Name = "href")]
        public string Href { get; set; }
    }

    [DataContract]
    public class ProjectsResponse
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
        [DataMember(Name = "project")]
        public List<Project> Projects { get; set; }
    }
}
