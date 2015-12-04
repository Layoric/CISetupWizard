using System;
using System.Collections.Generic;
using System.Net;
using CIWizard.ServiceModel;
using ServiceStack;
using ServiceStack.TeamCityClient;
using ServiceStack.TeamCityClient.Types;

namespace CIWizard.ServiceInterface
{
    public class TeamCityWizardService : Service
    {
        public TcClient TeamCityClient { get; set; }

        public CreateSpaBuildProjectResponse Post(CreateSpaBuildProject request)
        {
            var session = SessionAs<AuthUserSession>();
            var gitHubToken = session.GetGitHubAccessToken();

            var createProjResponse = CreateTeamCityProject(request);

            var vcsResponse = CreateVcsRoot(request, createProjResponse, gitHubToken);

            var createEmptyBuild = new CreateBuildConfig { Locator = "id:" + createProjResponse.Id, Name = "Build" };
            var emptyBuildConfigResponse = TeamCityClient.CreateBuildConfig(createEmptyBuild);

            AttachVcsToProject(emptyBuildConfigResponse, vcsResponse);

            CreateNpmBuildStep(request, emptyBuildConfigResponse);

            return new CreateSpaBuildProjectResponse
            {
                ProjectId = createProjResponse.Id
            };
        }

        public GetTeamCityProjectDetailsResponse Get(GetTeamCityProjectDetails request)
        {
            GetProjectResponse teamCityResponse = null;
            try
            {
                teamCityResponse = TeamCityClient.GetProject(new GetProject
                {
                    Locator = "id:" + request.ProjectId
                });
            }
            catch (WebException e)
            {
                if (e.HasStatus(HttpStatusCode.NotFound))
                {
                    throw HttpError.NotFound("Project not found");
                }
            }

            if (teamCityResponse == null)
            {
                throw new HttpError(HttpStatusCode.InternalServerError, "Invalid response from TeamCity");
            }

            GetBuildResponse build = null;
            try
            {
                build = TeamCityClient.GetBuild(new GetBuild { BuildLocator = "project:id:" + request.ProjectId });
            }
            catch (WebException e)
            {
                if (!e.HasStatus(HttpStatusCode.BadRequest))
                    throw;
            }

            build = build ?? new GetBuildResponse();

            var proj = teamCityResponse.ProjectsResponse.Projects.FirstNonDefault();
            var response = new GetTeamCityProjectDetailsResponse
            {
                Project = proj,
                BuildNumber = build.Number,
                BuildState = build.State,
                BuildStatus = build.StatusText
            };
            return response;
        }

        public GetBuildStatusResponse Get(GetBuildStatus request)
        {
            GetBuildResponse builds;
            try
            {
                builds = TeamCityClient.GetBuild(new GetBuild { BuildLocator = "project:id:" + request.ProjectId });
            }
            catch (WebException e)
            {
                if (e.HasStatus(HttpStatusCode.BadRequest))
                {
                    throw HttpError.NotFound("Project Not found");
                }
                throw;
            }
            return new GetBuildStatusResponse
            {
                Status = builds.Status,
                LastUpdate = builds.State == "finished" ? DateTime.Parse(builds.FinishDate) : DateTime.Parse(builds.StartDate)
            };
        }

        private CreateBuildStepResponse CreateNpmBuildStep(CreateSpaBuildProject request,
            CreateBuildConfigResponse buildConfigResponse)
        {
            var npmStepRequest = new CreateBuildStep
            {
                BuildTypeLocator = "id:" + buildConfigResponse.Id,
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
            return npmStepResponse;
        }

        private AttachVcsEntryResponse AttachVcsToProject(CreateBuildConfigResponse emptyBuildConfigResponse,
            CreateVcsRootResponse vcsResponse)
        {
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
            return attachResponse;
        }

        private CreateVcsRootResponse CreateVcsRoot(CreateSpaBuildProject request, CreateProjectResponse createProjResponse,
            string gitHubToken)
        {
            var createVcs = new CreateVcsRoot
            {
                Name = "GitHub_{0}".Fmt(request.Name),
                VcsName = VcsRootTypes.Git,
                Project = new CreateVcsRootProject {Id = createProjResponse.Id},
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
            return vcsResponse;
        }

        private CreateProjectResponse CreateTeamCityProject(CreateSpaBuildProject request)
        {
            var createProject = new CreateProject
            {
                Name = request.Name,
                ParentProject = new ProjectLocator
                {
                    Locator = "id:_Root"
                }
            };
            var createProjResponse = TeamCityClient.CreateProject(createProject);
            return createProjResponse;
        }
        
    }

    [Route("/user/projects/{ProjectId}")]
    public class GetTeamCityProjectDetails
    {
        public string ProjectId { get; set; }
    }

    public class GetTeamCityProjectDetailsResponse
    {
        public ServiceStack.TeamCityClient.Types.Project Project { get; set; }
        public string BuildNumber { get; set; }
        public string BuildStatus { get; set; }
        public string BuildState { get; set; }
    }
}
