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
        public void TeamCityClientCanAuthenticate()
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
        public void TeamCityClientCanGetProjects()
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
        public void TeamCityClientCanGetRootProject()
        {
            var getProjectResponse = _client.Get(new GetProject { ProjectLocator = "id:_Root" });
            Assert.That(getProjectResponse, Is.Not.Null);
            Assert.That(getProjectResponse.Id, Is.EqualTo("_Root"));
            Assert.That(getProjectResponse.Name, Is.Not.Null);
            Assert.That(getProjectResponse.Name, Is.EqualTo("<Root project>"));
            Assert.That(getProjectResponse.Description, Is.Not.Null);
        }

        [Test]
        public void TeamCityClientCanGetVcsRoots()
        {
            var getVcsRoots = _client.Get(new GetVcsRoots());
            Assert.That(getVcsRoots, Is.Not.Null);
            Assert.That(getVcsRoots.Count, Is.GreaterThan(0));
            Assert.That(getVcsRoots.Href, Is.Not.Null);
            Assert.That(getVcsRoots.VcsRoots, Is.Not.Null);
            Assert.That(getVcsRoots.VcsRoots.Count, Is.EqualTo(getVcsRoots.Count));
        }
    }
}
