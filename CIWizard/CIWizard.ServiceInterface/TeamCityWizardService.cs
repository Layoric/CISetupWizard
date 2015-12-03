using System.Collections.Generic;
using CIWizard.ServiceModel;
using ServiceStack;
using ServiceStack.TeamCityClient;

namespace CIWizard.ServiceInterface
{
    public class TeamCityWizardService : Service
    {
        public TcClient TeamCityClient { get; set; }

        public CreateSpaBuildProjectResponse Post(CreateSpaBuildProject request)
        {
            var response = new CreateSpaBuildProjectResponse();
            var session = SessionAs<AuthUserSession>();
            var gitHubToken = session.GetGitHubAccessToken();
            var createProject = new CreateProject
            {
                Name = request.Name,
                ParentProject = new ProjectLocator
                {
                    Locator = "id:_Root"
                }
            };
            var createProjResponse = TeamCityClient.CreateProject(createProject);
            var createVcs = new CreateVcsRoot
            {
                Name = "GitHub_{0}".Fmt(request.Name),
                VcsName = VcsRootTypes.Git,
                Project = new CreateVcsRootProject { Id = createProjResponse.Id },
                Properties = new CreateVcsRootProperties
                {
                    Properties = new List<CreateVcsRootProperty>
                    {
                        new CreateVcsRootProperty
                        {
                            Name = "url",
                            Value = request.RepositoryUrl
                        },
                        new CreateVcsRootProperty
                        {
                            Name = "authMethod",
                            Value = request.PrivateRepository ? "PASSWORD" : "ANONYMOUS"
                        },
                        new CreateVcsRootProperty
                        {
                            Name = "branch",
                            Value = "refs/heads/{0}".Fmt(request.Branch ?? "master")
                        }
                    }
                }
            };
            // Use OAuth access_token as username as per https://github.com/blog/1270-easier-builds-and-deployments-using-git-over-https-and-oauth#using-oauth-with-git
            if (request.PrivateRepository)
            {
                createVcs.Properties.Properties.Add(new CreateVcsRootProperty
                {
                    Name = "username",
                    Value = gitHubToken
                });
            }
            

            var vcsResponse = TeamCityClient.CreateVcsRoot(createVcs);

            var createEmptyBuild = new CreateBuildConfig { Locator = "id:" + createProjResponse.Id, Name = "Build" };
            var emptyBuildConfigResponse = TeamCityClient.CreateBuildConfig(createEmptyBuild);
            var attachRequest = new AttachVcsEntries
            {
                BuildTypeLocator = "id:" + emptyBuildConfigResponse.Id,
                VcsRootEntries = new List<AttachVcsRootEntry>
                {
                    new AttachVcsRootEntry
                    {
                        Id = vcsResponse.Id,
                        VcsRoot = new AttachVcsRoot
                        {
                            Id = vcsResponse.Id
                        }
                    }
                }
            };
            var attachResponse = TeamCityClient.AttachVcsEntries(attachRequest);

            //Create build steps
            var npmStepRequest = new CreateBuildStep
            {
                BuildTypeLocator = "id:" + emptyBuildConfigResponse.Id,
                Name = "NPM Install",
                TypeId = BuidStepTypes.Npm,
                StepProperies = new CreateBuildStepProperies
                {
                    Properties = new List<CreateBuildStepProperty>
                    {
                        new CreateBuildStepProperty
                        {
                            Name = "npm_commands",
                            Value = "install\ninstall bower\ninstall grunt\ninstall grunt-cli"
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "teamcity.build.workingDir",
                            Value = request.WorkingDirectory
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "teamcity.step.mode",
                            Value = "default"
                        }
                    }
                }
            };

            var npmStepResponse = TeamCityClient.CreateBuildStep(npmStepRequest);


            return response;
        }
    }
}
