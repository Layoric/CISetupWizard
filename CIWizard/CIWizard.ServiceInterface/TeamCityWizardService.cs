using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            var createProject = new CreateProject { Name = "TestTS" };
            var createProjResponse = TeamCityClient.CreateProject(createProject);
            var createVcs = new CreateVcsRoot
            {
                Name = "GitHub_Test1",
                VcsName = VcsRootTypes.Git,
                Project = new CreateVcsRootProject { Id = createProjResponse.Id },
                Properties = new CreateVcsRootProperties
                {
                    Properties = new List<CreateVcsRootProperty>
                    {
                        new CreateVcsRootProperty
                        {
                            Name = "Url",
                            Value = "https://github.com/ServiceStackApps/TechStacks.git"
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

            var vcsResponse = TeamCityClient.CreateVcsRoot(createVcs);

            var createEmptyBuild = new CreateBuildConfig { Locator = "Id:" + createProjResponse.Id, Name = "Build" };
            var emptyBuildConfigResponse = TeamCityClient.CreateBuildConfig(createEmptyBuild);
            var attachRequest = new AttachVcsEntries
            {
                BuildTypeLocator = "Id:" + emptyBuildConfigResponse.Id,
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
                BuildTypeLocator = "Id:" + emptyBuildConfigResponse.Id,
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
                            Value = "src/TechStacks/TechStacks"
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
