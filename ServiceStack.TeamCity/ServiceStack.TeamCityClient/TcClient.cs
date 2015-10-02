using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ServiceStack.TeamCityClient.Types;
using ServiceStack.Web;

namespace ServiceStack.TeamCityClient
{
    public class TcClient
    {
        public JsonServiceClient ServiceClient { get; }

        public TcClient(string baseUrl, string userName, string password)
        {
            ServiceClient = new JsonServiceClient(baseUrl)
            {
                UserName = userName,
                Password = password,
                StoreCookies = true
            };
        }

        public GetProjectsResponse GetProjects() =>
            ServiceClient.Get(new GetProjects());

        public GetProjectResponse GetProject(string locator) =>
            ServiceClient.Get(new GetProject { ProjectLocator = locator });

        public GetVcsRootsResponse GetVcsRoots() =>
            ServiceClient.Get(new GetVcsRoots());

        public GetBuildsResponse GetBuilds() =>
            ServiceClient.Get(new GetBuilds());

        public GetBuildResponse GetBuild(string locator) =>
            ServiceClient.Get(new GetBuild { BuildLocator = locator });

        public GetUsersResponse GetUsers() =>
            ServiceClient.Get(new GetUsers());

        public GetUserResponse GetUser(string locator) =>
            ServiceClient.Get(new GetUser { UserLocator = locator });

        public GetUserGroupsResponse GetUserGroups() =>
            ServiceClient.Get(new GetUserGroups());

        public GetUsersInGroupResponse GetUsersInGroup(string locator) =>
            ServiceClient.Get(new GetUsersInGroup { GroupLocator = locator });

        public CreateProjectResponse CreateProject(CreateProject project)
        {
            TcXmlServiceClient xmlServiceClient = new TcXmlServiceClient(
                ServiceClient.BaseUri, 
                ServiceClient.UserName,
                ServiceClient.Password);
            xmlServiceClient.CookieContainer = ServiceClient.CookieContainer;
            return xmlServiceClient.Post<CreateProjectResponse>("/projects/", project);
        }
    }

    public class TcXmlServiceClient : XmlServiceClient
    {

        public TcXmlServiceClient(string baseUrl, string userName, string password)
            : base(baseUrl)
        {
            this.UserName = userName;
            this.Password = password;
        }

        public override string Accept { get { return "application/json"; } }

        public override void SerializeToStream(IRequest requestContext, object request, Stream stream)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            XmlSerializer ser = new XmlSerializer(request.GetType());
            var streamWriter = new XmlTextWriter(stream, System.Text.Encoding.UTF8);
            ser.Serialize(streamWriter, request, ns);
        }

        public override T DeserializeFromStream<T>(Stream stream)
        {
            byte[] response = stream.ReadFully();
            return response.FromUtf8Bytes().FromJson<T>();
        }
    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }

    [XmlSerializerFormat]
    [XmlType(TypeName = "newProjectDescription", Namespace = "")]
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

    [XmlSerializerFormat]
    public class ProjectLocator
    {
        [XmlAttribute(AttributeName = "locator")]
        public string Locator { get; set; }
    }
}
