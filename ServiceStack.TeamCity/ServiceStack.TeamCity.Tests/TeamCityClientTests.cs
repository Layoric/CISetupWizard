using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NUnit.Framework;
using ServiceStack.Configuration;
using ServiceStack.TeamCityClient;
using XmlSerializer = ServiceStack.Text.XmlSerializer;

namespace ServiceStack.TeamCity.Tests
{
    [TestFixture]
    public class TeamCityClientTests
    {
        public readonly JsonServiceClient Client = new JsonServiceClient("http://localhost:8484/app/rest")
        {
            StoreCookies = true
        };

        public readonly IAppSettings Settings;
        public TeamCityClientTests()
        {
            var fileInfo = new FileInfo("../../appsettings.txt");
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("Missing appsettings file that provides TeamCity credentials");
            }
            Settings = new TextFileSettings("../../appsettings.txt");
            Client.UserName = Settings.GetString("UserName");
            Client.Password = Settings.GetString("Password");
        }

        [Test]
        public void CanAuthenticate()
        {
            HttpWebResponse response = null;
            try
            {
                response = Client.Get("/projects");
            }
            catch (Exception)
            {
                Assert.Fail("Authentication with TeamCity failed.");
            }

            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void CanGetProjects()
        {
            var getProjectsResponse = Client.Get(new GetProjects());
            Assert.That(getProjectsResponse, Is.Not.Null);
            Assert.That(getProjectsResponse.Count, Is.GreaterThan(0));
            Assert.That(getProjectsResponse.Projects, Is.Not.Null);
            Assert.That(getProjectsResponse.Projects.Count, Is.EqualTo(getProjectsResponse.Count));
            Assert.That(getProjectsResponse.Projects[0].Description, Is.Not.Null);
            Assert.That(getProjectsResponse.Projects[0].Name, Is.Not.Null);
            Assert.That(getProjectsResponse.Projects[0].Id, Is.Not.Null);
        }

        [Test]
        public void CanGetRootProject()
        {
            var getProjectResponse = Client.Get(new GetProject { ProjectLocator = "id:_Root" });
            Assert.That(getProjectResponse, Is.Not.Null);
            Assert.That(getProjectResponse.Id, Is.EqualTo("_Root"));
            Assert.That(getProjectResponse.Name, Is.Not.Null);
            Assert.That(getProjectResponse.Name, Is.EqualTo("<Root project>"));
            Assert.That(getProjectResponse.Description, Is.Not.Null);
        }

        [Test]
        public void CanGetVcsRoots()
        {
            var getVcsRoots = Client.Get(new GetVcsRoots());
            Assert.That(getVcsRoots, Is.Not.Null);
            Assert.That(getVcsRoots.Count, Is.GreaterThan(0));
            Assert.That(getVcsRoots.Href, Is.Not.Null);
            Assert.That(getVcsRoots.VcsRoots, Is.Not.Null);
            Assert.That(getVcsRoots.VcsRoots.Count, Is.EqualTo(getVcsRoots.Count));
        }

        [Test]
        public void CanGetBuilds()
        {
            var getBuilds = Client.Get(new GetBuilds());
            Assert.That(getBuilds, Is.Not.Null);
            Assert.That(getBuilds.Count, Is.GreaterThan(0));
            Assert.That(getBuilds.Href, Is.Not.Null);
            Assert.That(getBuilds.Builds, Is.Not.Null);
            Assert.That(getBuilds.Builds.Count, Is.EqualTo(getBuilds.Count));
        }

        [Test]
        public void CanGetSingleBuild()
        {
            var getBuild = Client.Get(new GetBuild { BuildLocator = "number:9" });
            Assert.That(getBuild, Is.Not.Null);
            Assert.That(getBuild.Triggered, Is.Not.Null);
            Assert.That(getBuild.Agent, Is.Not.Null);
            Assert.That(getBuild.Artifacts, Is.Not.Null);
            Assert.That(getBuild.BuildType, Is.Not.Null);
            Assert.That(getBuild.BuildTypeId, Is.Not.Null);
            Assert.That(getBuild.Changes, Is.Not.Null);
            Assert.That(getBuild.FinishDate, Is.Not.Null);
            Assert.That(getBuild.Href, Is.Not.Null);
            Assert.That(getBuild.Id, Is.Not.Null);
            Assert.That(getBuild.LastChangesResponse, Is.Not.Null);
            Assert.That(getBuild.Number, Is.Not.Null);
            Assert.That(getBuild.Properties, Is.Not.Null);
            Assert.That(getBuild.QueuedDate, Is.Not.Null);
            Assert.That(getBuild.RelatedIssues, Is.Not.Null);
            Assert.That(getBuild.Revisions, Is.Not.Null);
            Assert.That(getBuild.StartDate, Is.Not.Null);
            Assert.That(getBuild.State, Is.Not.Null);
            Assert.That(getBuild.Statistics, Is.Not.Null);
            Assert.That(getBuild.Status, Is.Not.Null);
            Assert.That(getBuild.StatusText, Is.Not.Null);
        }

        [Test]
        public void CanGetUsers()
        {
            var getUsers = Client.Get(new GetUsers());
            Assert.That(getUsers, Is.Not.Null);
            Assert.That(getUsers.Count, Is.GreaterThan(0));
            Assert.That(getUsers.Users, Is.Not.Null);
            Assert.That(getUsers.Users.Count, Is.EqualTo(getUsers.Count));
        }

        [Test]
        public void CanGetUser()
        {
            var getUser = Client.Get(new GetUser {UserLocator = "username:" + Settings.GetString("UserName")});
            Assert.That(getUser, Is.Not.Null);
            Assert.That(getUser.Name, Is.Not.Null);
            Assert.That(getUser.Id, Is.Not.Null);
            Assert.That(getUser.GroupDetails, Is.Not.Null);
            Assert.That(getUser.Href, Is.Not.Null);
            Assert.That(getUser.LastLogin, Is.Not.Null);
            Assert.That(getUser.PropertyDetails, Is.Not.Null);
            Assert.That(getUser.RoleDetails, Is.Not.Null);
            Assert.That(getUser.Username, Is.Not.Null);
            Assert.That(getUser.Username, Is.EqualTo(Settings.GetString("UserName")));
        }

        [Test]
        public void CanGetUserGroups()
        {
            var getGroups = Client.Get(new GetUserGroups());
            Assert.That(getGroups, Is.Not.Null);
            Assert.That(getGroups.Count, Is.GreaterThan(0));
            Assert.That(getGroups.Groups, Is.Not.Null);
            Assert.That(getGroups.Groups.Count, Is.EqualTo(getGroups.Count));
        }

        [Test]
        public void CanGetUsersInGroup()
        {
            var getUsersInGroup = Client.Get(new GetUsersInGroup { GroupLocator = "key:ALL_USERS_GROUP"});
            Assert.That(getUsersInGroup, Is.Not.Null);
            Assert.That(getUsersInGroup.Name, Is.Not.Null);
            Assert.That(getUsersInGroup.ChildGroupsResponse, Is.Not.Null);
            Assert.That(getUsersInGroup.Description, Is.Not.Null);
            Assert.That(getUsersInGroup.Href, Is.Not.Null);
            Assert.That(getUsersInGroup.Key, Is.Not.Null);
            Assert.That(getUsersInGroup.ParentGroupsResponse, Is.Not.Null);
            Assert.That(getUsersInGroup.PropertiesResponse, Is.Not.Null);
            Assert.That(getUsersInGroup.RolesResponse, Is.Not.Null);
            Assert.That(getUsersInGroup.UsersResponse, Is.Not.Null);
        }

        [Test]
        public void CanCreateProject()
        {
            XmlServiceClient serviceClient = new XmlServiceClient("http://localhost:8484/app/rest");
            serviceClient.UserName = Client.UserName;
            serviceClient.Password = Client.Password;
            serviceClient.StoreCookies = true;
            serviceClient.RequestFilter += request =>
            {
                Console.WriteLine("Fo1");
            };
            serviceClient.ResponseFilter += response =>
            {
                Console.WriteLine(response.ReadToEnd());
            };
            var req = new CreateProject {Name = "FooBar1"};
            var rawReq = XmlSerializer.SerializeToString(req);

            //Assert.That(createResponse, Is.Not.Null);
        }
    }

    [Route("/projects")]
    [DataContract(Name = "newProjectDescription", Namespace = "")]
    [XmlSerializerFormat]
    public class CreateProject : IReturn<CreateProjectResponse>
    {
        [DataMember(Name = "name")]
        [XmlAttribute]
        public string Name { get; set; }
    }

    public class CreateProjectResponse
    {
    }

    [DataContract]
    public class CreateBuildConfig
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "projectId")]
        public string ProjectId { get; set; }
    }
}
