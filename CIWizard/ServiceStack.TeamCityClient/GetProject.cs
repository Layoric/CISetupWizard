using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.TeamCityClient.Types;
using ServiceStack.Text;

namespace ServiceStack.TeamCityClient
{
    [Route("/projects")]
    public class GetProjects : IReturn<GetProjectsResponse>
    {

    }

    [DataContract]
    public class GetProjectsResponse
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
        [DataMember(Name = "project")]
        public List<Project> Projects { get; set; }
    }

    [Route("/projects/{Locator}")]
    public class GetProject : IReturn<GetProjectResponse>
    {
        /// <summary>
        /// Example id:project1
        /// </summary>
        public string Locator { get; set; }
    }

    [DataContract]
    public class GetProjectResponse
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "parentProjectId")]
        public string ParentProjectId { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "href")]
        public string Href { get; set; }
        [DataMember(Name = "webUrl")]
        public string WebUrl { get; set; }
        [DataMember(Name = "buildTypes")]
        public BuildTypesResponse BuildTypeResponse { get; set; }
        [DataMember(Name = "templates")]
        public TemplatesResponse TemplatesResponse { get; set; }
        [DataMember(Name = "parameters")]
        public ParametersResponse ParametersResponse { get; set; }
        [DataMember(Name = "vcsRoots")]
        public VcsRootsResponse VcsRootsResponse { get; set; }
        [DataMember(Name = "projects")]
        public ProjectsResponse ProjectsResponse { get; set; }
        [DataMember(Name = "parentProject")]
        public Project ParentProject { get; set; }
    }
}
