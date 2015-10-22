using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using ServiceStack.Configuration;
using ServiceStack.TeamCityClient;
using ServiceStack.TeamCityClient.Types;

namespace ServiceStack.TeamCity.Tests
{
    [TestFixture]
    public class TeamCityClientTests
    {
        public TcClient Client;
        public readonly IAppSettings Settings;
        private string testProjectName = "TestProjectUnderRoot";
        private string emptyRootProjectName = "TestProjectEmptyRoot";
        private string projectWithBuild = "TestprojectWithBuild";
        public TeamCityClientTests()
        {
            var fileInfo = new FileInfo("../../appsettings.txt");
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("Missing appsettings file that provides TeamCity credentials");
            }
            Settings = new TextFileSettings("../../appsettings.txt");
            Client = new TcClient(
                Settings.GetString("ServerApiBaseUrl"),
                Settings.GetString("UserName"),
                Settings.GetString("Password"));
            
        }

        [Test]
        public void CanGetProjects()
        {
            var getProjectsResponse = Client.GetProjects();
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
            var getProjectResponse = Client.GetProject("id:_Root");
            Assert.That(getProjectResponse, Is.Not.Null);
            Assert.That(getProjectResponse.Id, Is.EqualTo("_Root"));
            Assert.That(getProjectResponse.Name, Is.Not.Null);
            Assert.That(getProjectResponse.Name, Is.EqualTo("<Root project>"));
            Assert.That(getProjectResponse.Description, Is.Not.Null);
        }

        [Test]
        public void CanGetVcsRoots()
        {
            var getVcsRoots = Client.GetVcsRoots();
            Assert.That(getVcsRoots, Is.Not.Null);
            Assert.That(getVcsRoots.Count, Is.GreaterThan(0));
            Assert.That(getVcsRoots.Href, Is.Not.Null);
            Assert.That(getVcsRoots.VcsRoots, Is.Not.Null);
            Assert.That(getVcsRoots.VcsRoots.Count, Is.EqualTo(getVcsRoots.Count));
        }

        [Test]
        public void CanGetBuilds()
        {
            var getBuilds = Client.GetBuilds();
            Assert.That(getBuilds, Is.Not.Null);
            Assert.That(getBuilds.Count, Is.GreaterThan(0));
            Assert.That(getBuilds.Href, Is.Not.Null);
            Assert.That(getBuilds.Builds, Is.Not.Null);
            Assert.That(getBuilds.Builds.Count, Is.EqualTo(getBuilds.Count));
        }

        [Test]
        public void CanGetSingleBuild()
        {
            var getBuild = Client.GetBuild("number:1");
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
            //Assert.That(getBuild.LastChangesResponse, Is.Not.Null);
            Assert.That(getBuild.Number, Is.Not.Null);
            //Assert.That(getBuild.Properties, Is.Not.Null);
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
        public void CanGetProjectBuildConfigs()
        {
            var projResponse = Client.GetProjects();
            var response = Client.GetBuildConfigs("id:" + projResponse.Projects[1].Id);
            Assert.That(response,Is.Not.Null);
            Assert.That(response.Count, Is.GreaterThan(0));
            Assert.That(response.BuildTypes, Is.Not.Null);
            Assert.That(response.BuildTypes.Count, Is.GreaterThan(0));
            Assert.That(response.BuildTypes[0], Is.Not.Null);
            Assert.That(response.BuildTypes[0].Name,Is.Not.Null);
            Assert.That(response.BuildTypes[0].Id,Is.Not.Null);
            Assert.That(response.BuildTypes[0].Href,Is.Not.Null);
            Assert.That(response.BuildTypes[0].ProjectName,Is.Not.Null);
            Assert.That(response.BuildTypes[0].ProjectId,Is.Not.Null);
            Assert.That(response.BuildTypes[0].WebUrl,Is.Not.Null);
        }

        [Test]
        public void CanGetUsers()
        {
            var getUsers = Client.GetUsers();
            Assert.That(getUsers, Is.Not.Null);
            Assert.That(getUsers.Count, Is.GreaterThan(0));
            Assert.That(getUsers.Users, Is.Not.Null);
            Assert.That(getUsers.Users.Count, Is.EqualTo(getUsers.Count));
        }

        [Test]
        public void CanGetUser()
        {
            var getUser = Client.GetUser("username:" + Settings.GetString("UserName"));
            Assert.That(getUser, Is.Not.Null);
            Assert.That(getUser.Name, Is.Not.Null);
            Assert.That(getUser.Id, Is.Not.Null);
            Assert.That(getUser.GroupDetails, Is.Not.Null);
            Assert.That(getUser.Href, Is.Not.Null);
            Assert.That(getUser.LastLogin, Is.Not.Null);
            Assert.That(getUser.PropertyDetails, Is.Not.Null);
            Assert.That(getUser.Username, Is.Not.Null);
            Assert.That(getUser.Username.ToLowerInvariant(), Is.EqualTo(Settings.GetString("UserName").ToLowerInvariant()));
        }

        [Test]
        public void CanGetUserGroups()
        {
            var getGroups = Client.GetUserGroups();
            Assert.That(getGroups, Is.Not.Null);
            Assert.That(getGroups.Count, Is.GreaterThan(0));
            Assert.That(getGroups.Groups, Is.Not.Null);
            Assert.That(getGroups.Groups.Count, Is.EqualTo(getGroups.Count));
        }

        [Test]
        public void CanGetUsersInGroup()
        {
            var getUsersInGroup = Client.GetUsersInGroup("key:ALL_USERS_GROUP");
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
        public void CanCreateProjectUnderRoot()
        {
            var createProject = new CreateProject
            {
                Id = testProjectName,
                Name = testProjectName,
                ParentProject = new ProjectLocator
                {
                    Locator = "id:_Root"
                }
            };
            var response = Client.CreateProject(createProject);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Name, Is.Not.Null);
            Assert.That(response.BuildTypes, Is.Not.Null);
            Assert.That(response.Href, Is.Not.Null);
            Assert.That(response.Id, Is.Not.Null);
            Assert.That(response.Id, Is.EqualTo(createProject.Id));
            Assert.That(response.Parameters, Is.Not.Null);
            Assert.That(response.ParentProject, Is.Not.Null);
            Assert.That(response.ParentProject.Id, Is.EqualTo("_Root"));
            Assert.That(response.ParentProjectId, Is.EqualTo("_Root"));
            Assert.That(response.Templates, Is.Not.Null);
            Assert.That(response.Projects, Is.Not.Null);
            Assert.That(response.VcsRoots, Is.Not.Null);
            Assert.That(response.WebUrl, Is.Not.Null);
            Client.DeleteProject("id:" + response.Id);
        }

        [Test]
        public void CanCreateEmptyRootProject()
        {
            var createProject = new CreateProject
            {
                Id = emptyRootProjectName,
                Name = emptyRootProjectName
            };
            var response = Client.CreateProject(createProject);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Name, Is.Not.Null);
            Assert.That(response.BuildTypes, Is.Not.Null);
            Assert.That(response.Href, Is.Not.Null);
            Assert.That(response.Id, Is.Not.Null);
            Assert.That(response.Id, Is.EqualTo(createProject.Id));
            Assert.That(response.Parameters, Is.Not.Null);
            Assert.That(response.ParentProject, Is.Not.Null);
            Assert.That(response.Templates, Is.Not.Null);
            Assert.That(response.Projects, Is.Not.Null);
            Assert.That(response.VcsRoots, Is.Not.Null);
            Assert.That(response.WebUrl, Is.Not.Null);
            Client.DeleteProject("id:" + response.Id);
        }

        [Test]
        public void CanCreateBuildConfig()
        {
            var createProject = new CreateProject
            {
                Id = projectWithBuild,
                Name = projectWithBuild,
            };
            var response = Client.CreateProject(createProject);
            var createBuildConfig = new CreateBuildConfig
            {
                ProjectLocator = "id:" + createProject.Id,
                Name = "BuildMe"
            };
            var buildConfigResponse = Client.CreateBuildConfig(createBuildConfig);
            Assert.That(buildConfigResponse, Is.Not.Null);
            Assert.That(buildConfigResponse.Name, Is.Not.Null);
            Assert.That(buildConfigResponse.AgentRequirementsResponse, Is.Not.Null);
            Assert.That(buildConfigResponse.ArtifactDependenciesResponse, Is.Not.Null);
            Assert.That(buildConfigResponse.Builds, Is.Not.Null);
            Assert.That(buildConfigResponse.FeaturesResponse, Is.Not.Null);
            Assert.That(buildConfigResponse, Is.Not.Null);
            Assert.That(buildConfigResponse, Is.Not.Null);
            Assert.That(buildConfigResponse, Is.Not.Null);
            Assert.That(buildConfigResponse, Is.Not.Null);
            Assert.That(buildConfigResponse, Is.Not.Null);
            Assert.That(buildConfigResponse, Is.Not.Null);
            Assert.That(buildConfigResponse, Is.Not.Null);
            Assert.That(buildConfigResponse, Is.Not.Null);
            Assert.That(buildConfigResponse, Is.Not.Null);
            Assert.That(buildConfigResponse, Is.Not.Null);
            Assert.That(buildConfigResponse, Is.Not.Null);
            Assert.That(buildConfigResponse, Is.Not.Null);
            Client.DeleteProject("id:" + response.Id);
        }

        [Test]
        public void CanCreateNewVcsRoot()
        {
            var proj = new CreateProject
            {
                Name = "TestProj123"
            };
            var projRes = Client.CreateProject(proj);

            var createVcs = new CreateVcsRoot
            {
                Name = "TestVcs1",
                VcsName = "jetbrains.git",
                Project = new CreateVcsRootProject {  Id = projRes.Id },
                Properties = new CreateVcsRootProperties
                {
                    Properties = new List<CreateVcsRootProperty>
                    {
                        new CreateVcsRootProperty
                        {
                            Name = "url",
                            Value = "https://github.com/ServiceStackApps/StackApis.git"
                        },
                        new CreateVcsRootProperty
                        {
                            Name = "authMethod",
                            Value = "ANONYMOUS"
                        },
                        new CreateVcsRootProperty
                        {
                            Name = "branch",
                            Value = "refs/heads/master"
                        }
                    }
                }
            };
            CreateVcsRootResponse response = null;
            try
            {
                response = Client.CreateVcsRoot(createVcs);
            }
            catch (Exception e)
            {
                Client.DeleteProject("id:" + projRes.Id);
                throw;
            }
            
            Assert.That(response, Is.Not.Null);

            Client.DeleteProject("id:" + projRes.Id);
        }
    }
}
