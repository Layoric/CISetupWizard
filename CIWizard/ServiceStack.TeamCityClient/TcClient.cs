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
        public JsonServiceClient ServiceClient { get; private set; }
        public TcXmlServiceClient XmlServiceClient { get; private set; }

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

        public GetProjectsResponse GetProjects()
        {
            return ServiceClient.Get(new GetProjects());
        }

        public GetProjectResponse GetProject(GetProject request)
        {
            return ServiceClient.Get(request);
        }

        public GetVcsRootsResponse GetVcsRoots()
        {
            return ServiceClient.Get(new GetVcsRoots());
        }

        public GetBuildsResponse GetBuilds()
        {
            return ServiceClient.Get(new GetBuilds());
        }

        public GetBuildResponse GetBuild(GetBuild request)
        {
            return ServiceClient.Get(request);
        }

        public GetUsersResponse GetUsers()
        {
            return ServiceClient.Get(new GetUsers());
        }

        public GetUserResponse GetUser(GetUser request)
        {
            return ServiceClient.Get(request);
        }

        public GetUserGroupsResponse GetUserGroups()
        {
            return ServiceClient.Get(new GetUserGroups());
        }

        public GetUsersInGroupResponse GetUsersInGroup(GetUsersInGroup request)
        {
            return ServiceClient.Get(request);
        }

        public GetProjectBuildConfigsResponse GetProjectGetBuildConfigs(GetProjectBuildConfigs request)
        {
            return ServiceClient.Get(request);
        }

        public CreateProjectResponse CreateProject(CreateProject project)
        {
            return XmlServiceClient.Post(project);
        }

        public CreateBuildConfigResponse CreateBuildConfig(CreateBuildConfig buildConfig)
        {
            return XmlServiceClient.Post(buildConfig);
        }

        public void DeleteProject(DeleteProject request)
        {
            XmlServiceClient.Delete(request);
        }

        public CreateVcsRootResponse CreateVcsRoot(CreateVcsRoot vcsRoot)
        {
            return XmlServiceClient.Post(vcsRoot);
        }

        public AttachVcsEntryResponse AttachVcsEntries(AttachVcsEntries request)
        {
            return XmlServiceClient.Put(request);
        }

        public CreateBuildStepResponse CreateBuildStep(CreateBuildStep request)
        {
            return XmlServiceClient.Post(request);
        }

        public CreateTriggerResponse CreateTrigger(CreateTrigger request)
        {
            return XmlServiceClient.Post(request);
        }

        public UpdateBuildConfigSettingResponse UpdateBuildConfigSettings(UpdateBuildConfigSetting request)
        {
            var url = XmlServiceClient.BaseUri + request.ToPutUrl();
            return url.PutStringToUrl(request.Value, "application/xml", "*/*", webRequest =>
            {
                webRequest.CookieContainer = XmlServiceClient.CookieContainer;
            }).FromJson<UpdateBuildConfigSettingResponse>();
        }
    }

    public class TcXmlServiceClient : XmlServiceClient
    {
        public TcXmlServiceClient(string baseUrl, string userName, string password)
            : base(baseUrl)
        {
            UserName = userName;
            Password = password;
        }

        public override string Accept
        {
            get { return "application/json"; }
        }

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
