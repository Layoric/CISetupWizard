using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ServiceStack.TeamCityClient.Types;
using ServiceStack.Web;

namespace ServiceStack.TeamCityClient
{
    public class TcClient
    {
        public JsonServiceClient ServiceClient { get; }
        public TcXmlServiceClient XmlServiceClient { get; }

        public TcClient(string baseUrl, string userName, string password)
        {
            ServiceClient = new JsonServiceClient(baseUrl)
            {
                UserName = userName,
                Password = password,
                StoreCookies = true
            };
            XmlServiceClient = new TcXmlServiceClient(baseUrl, userName, password) {StoreCookies = true};
        }

        public GetProjectsResponse GetProjects() =>
            ServiceClient.Get(new GetProjects());

        public GetProjectResponse GetProject(GetProject request) =>
            ServiceClient.Get(request);

        public GetVcsRootsResponse GetVcsRoots() =>
            ServiceClient.Get(new GetVcsRoots());

        public GetBuildsResponse GetBuilds() =>
            ServiceClient.Get(new GetBuilds());

        public GetBuildResponse GetBuild(GetBuild request) =>
            ServiceClient.Get(request);

        public GetUsersResponse GetUsers() =>
            ServiceClient.Get(new GetUsers());

        public GetUserResponse GetUser(GetUser request) =>
            ServiceClient.Get(request);

        public GetUserGroupsResponse GetUserGroups() =>
            ServiceClient.Get(new GetUserGroups());

        public GetUsersInGroupResponse GetUsersInGroup(GetUsersInGroup request) =>
            ServiceClient.Get(request);

        public GetProjectBuildConfigsResponse GetProjectGetBuildConfigs(GetProjectBuildConfigs request) =>
            ServiceClient.Get(request);

        public CreateProjectResponse CreateProject(CreateProject project) => 
            XmlServiceClient.Post(project);

        public CreateBuildConfigResponse CreateBuildConfig(CreateBuildConfig buildConfig) =>
            XmlServiceClient.Post(buildConfig);

        public void DeleteProject(DeleteProject request) =>
            XmlServiceClient.Delete(request);

        public CreateVcsRootResponse CreateVcsRoot(CreateVcsRoot vcsRoot) =>
            XmlServiceClient.Post(vcsRoot);

        public AttachVcsEntryResponse AttachVcsEntries(AttachVcsEntries request) =>
            XmlServiceClient.Put(request);

        public CreateBuildStepResponse CreateBuildStep(CreateBuildStep request) =>
            XmlServiceClient.Post(request);

    }

    public class TcXmlServiceClient : XmlServiceClient
    {
        public TcXmlServiceClient(string baseUrl, string userName, string password)
            : base(baseUrl)
        {
            UserName = userName;
            Password = password;
        }

        public override string Accept => "application/json";

        public override void SerializeToStream(IRequest requestContext, object request, Stream stream)
        {
            var ns = new XmlSerializerNamespaces();
            var ser = new XmlSerializer(request.GetType());
            var streamWriter = new XmlTextWriter(stream, Encoding.UTF8);
            string temp;
            using (StringWriter textWriter = new StringWriter())
            {
                ser.Serialize(textWriter, request);
                temp = textWriter.ToString();
            }
            ser.Serialize(streamWriter, request, ns);
        }

        public override T DeserializeFromStream<T>(Stream stream)
        {
            var response = stream.ReadFully();
            return response.FromUtf8Bytes().FromJson<T>();
        }
    }
}
