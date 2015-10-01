using System;
using System.IO;
using System.Net;
using NUnit.Framework;
using ServiceStack.Configuration;
using ServiceStack.TeamCityClient;

namespace ServiceStack.TeamCity.Tests
{
    [TestFixture]
    public class TeamCityClientTests
    {
        private readonly JsonServiceClient _client = new JsonServiceClient("http://localhost:8484/app/rest")
        {
            StoreCookies = true
        };

        private IAppSettings Settings;
        public TeamCityClientTests()
        {
            var fileInfo = new FileInfo("../../appsettings.txt");
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("Missing appsettings file that provides TeamCity credentials");
            }
            Settings = new TextFileSettings("../../appsettings.txt");
            _client.UserName = Settings.GetString("UserName");
            _client.Password = Settings.GetString("Password");
        }

        [Test]
        public void CanAuthenticate()
        {
            HttpWebResponse response = null;
            try
            {
                response = _client.Get("/projects");
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
            var getProjectsResponse = _client.Get(new GetProjects());
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
            var getProjectResponse = _client.Get(new GetProject { ProjectLocator = "id:_Root" });
            Assert.That(getProjectResponse, Is.Not.Null);
            Assert.That(getProjectResponse.Id, Is.EqualTo("_Root"));
            Assert.That(getProjectResponse.Name, Is.Not.Null);
            Assert.That(getProjectResponse.Name, Is.EqualTo("<Root project>"));
            Assert.That(getProjectResponse.Description, Is.Not.Null);
        }

        [Test]
        public void CanGetVcsRoots()
        {
            var getVcsRoots = _client.Get(new GetVcsRoots());
            Assert.That(getVcsRoots, Is.Not.Null);
            Assert.That(getVcsRoots.Count, Is.GreaterThan(0));
            Assert.That(getVcsRoots.Href, Is.Not.Null);
            Assert.That(getVcsRoots.VcsRoots, Is.Not.Null);
            Assert.That(getVcsRoots.VcsRoots.Count, Is.EqualTo(getVcsRoots.Count));
        }

        [Test]
        public void CanGetBuilds()
        {
            var getBuilds = _client.Get(new GetBuilds());
            Assert.That(getBuilds, Is.Not.Null);
            Assert.That(getBuilds.Count, Is.GreaterThan(0));
            Assert.That(getBuilds.Href, Is.Not.Null);
            Assert.That(getBuilds.Builds, Is.Not.Null);
            Assert.That(getBuilds.Builds.Count, Is.EqualTo(getBuilds.Count));
        }

        [Test]
        public void CanGetSingleBuild()
        {
            var getBuild = _client.Get(new GetBuild { BuildLocator = "number:9" });
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
            Assert.That(getBuild.LastChanges, Is.Not.Null);
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
            var getUsers = _client.Get(new GetUsers());
            Assert.That(getUsers, Is.Not.Null);
            Assert.That(getUsers.Count, Is.GreaterThan(0));
            Assert.That(getUsers.Users, Is.Not.Null);
            Assert.That(getUsers.Users.Count, Is.EqualTo(getUsers.Count));
        }
    }
}
