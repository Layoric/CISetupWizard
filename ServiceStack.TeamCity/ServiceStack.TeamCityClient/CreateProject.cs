using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Serialization;
using ServiceStack.TeamCityClient.Types;

namespace ServiceStack.TeamCityClient
{
    /// <summary>
    /// XmlSerializer is used due to TC only accepting XML for some of the commands.
    /// Can still respond with JSON so TcXmlServiceClient is used to manage different serializations.
    ///  </summary>
    [XmlSerializerFormat]
    [XmlType(TypeName = "newProjectDescription", Namespace = "")]
    [Route("/projects", "POST")]
    public class CreateProject : IReturn<CreateProjectResponse>
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "parentProject")]
        public ProjectLocator ParentProject { get; set; }
        [XmlElement(ElementName = "sourceProject")]
        public ProjectLocator SourceProject { get; set; }
    }

    [XmlSerializerFormat]
    public class ProjectLocator
    {
        [XmlAttribute(AttributeName = "locator")]
        public string Locator { get; set; }
    }

    [DataContract]
    public class CreateProjectResponse
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "parentProjectId")]
        public string ParentProjectId { get; set; }
        [DataMember(Name = "href")]
        public string Href { get; set; }
        [DataMember(Name = "webUrl")]
        public string WebUrl { get; set; }
        [DataMember(Name = "parentProject")]
        public Project ParentProject { get; set; }
        [DataMember(Name = "buildTypes")]
        public BuildTypesResponse BuildTypes { get; set; }
        [DataMember(Name = "templates")]
        public TemplatesResponse Templates { get; set; }
        [DataMember(Name = "parameters")]
        public ParametersResponse Parameters { get; set; }
        [DataMember(Name = "vcsRoots")]
        public VcsRootsResponse VcsRoots { get; set; }
        [DataMember(Name = "projects")]
        public ProjectsResponse Projects { get; set; }
    }
}
