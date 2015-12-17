using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using CIWizard.ServiceModel;
using CIWizard.ServiceModel.Types;
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
            CreateVcsTrigger(emptyBuildConfigResponse);

            CreateNpmInstallStep(request, emptyBuildConfigResponse);
            CreateBowerInstallStep(request, emptyBuildConfigResponse);
            CreateNuGetRestoreStep(request, emptyBuildConfigResponse);
            CreateGruntStep(request, emptyBuildConfigResponse);

            return new CreateSpaBuildProjectResponse
            {
                ProjectId = createProjResponse.Id
            };
        }

        public void Delete(DeleteTeamCityProject request)
        {
            TeamCityClient.DeleteProject(new DeleteProject { Locator = "id:" + request.ProjectId});
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
                build = TeamCityClient.GetBuild(new GetBuild {BuildLocator = "project:id:" + request.ProjectId});
            }
            catch (WebException e)
            {
                if (!e.HasStatus(HttpStatusCode.BadRequest))
                    throw;
            }
            catch (WebServiceException wse)
            {
                if (!wse.IsAny400())
                    throw;
            }

            build = build ?? new GetBuildResponse();

            var response = new GetTeamCityProjectDetailsResponse
            {
                ProjectName = teamCityResponse.Name,
                ProjectId = teamCityResponse.Id,
                BuildNumber = build.Number,
                BuildState = build.State,
                BuildStatus = build.StatusText
            };
            return response;
        }

        [Authenticate]
        public object Get(GetAllGeneratedTeamCityProjects request)
        {
            var gitHubToken = SessionAs<AuthUserSession>().GetGitHubAccessToken();
            var allTcProjs = TeamCityClient.GetProjects();
            var allGhProjects = GitHubHelper.GetGitHubRepositories(gitHubToken);

            var configuredRepos = GetConfiguredRepos(allGhProjects, allTcProjs);

            return new GetAllGeneratedTeamCityProjectsResponse
            {
                Projects = configuredRepos
            };
        }

        private static List<GitHubRepository> GetConfiguredRepos(List<GitHubRepository> allGhProjects, GetProjectsResponse allTcProjs)
        {
            // Generated projects are created with "SS_" prefix and end with project name, owners can vary
            return allGhProjects.Where(x =>
            {
                return allTcProjs.Projects.Any(y =>
                {
                    if (!y.Id.StartsWith("SS_")) return false;
                    string projName = y.Id.Substring(y.Id.LastIndexOf("_", StringComparison.Ordinal) + 1);
                    return x.Name == projName;
                });
            }).ToList();
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
            //Parse datetime from TeamCity
            string pattern = "yyyyMMddTHHmmssK";
            DateTime? startDateTime = DateTime.ParseExact(builds.StartDate, pattern, CultureInfo.InvariantCulture);
            DateTime? finishDateTime = DateTime.ParseExact(builds.FinishDate, pattern, CultureInfo.InvariantCulture);

            return new GetBuildStatusResponse
            {
                Status = builds.Status,
                LastUpdate = builds.State == "finished" ? finishDateTime : startDateTime
            };
        }

        private CreateBuildStepResponse CreateNpmInstallStep(CreateSpaBuildProject request,
            CreateBuildConfigResponse buildConfigResponse)
        {
            var npmStepResponse = TeamCityClient.CreateBuildStep(
                TeamCityRequestBuilder.GetNpmInstallStepRequest(buildConfigResponse.Id,request.WorkingDirectory));
            return npmStepResponse;
        }

        private AttachVcsEntryResponse AttachVcsToProject(CreateBuildConfigResponse buildConfigResponse,
            CreateVcsRootResponse vcsResponse)
        {
            var attachResponse = TeamCityClient.AttachVcsEntries(
                TeamCityRequestBuilder.GetAttachVcsEntriesRequest(buildConfigResponse.Id,vcsResponse.Id));
            return attachResponse;
        }

        private CreateTriggerResponse CreateVcsTrigger(CreateBuildConfigResponse buildConfigResponse)
        {
            var triggerResponse =
                TeamCityClient.CreateTrigger(TeamCityRequestBuilder.GetCreateVcsTrigger(buildConfigResponse.Id));
            return triggerResponse;
        }

        private CreateBuildStepResponse CreateBowerInstallStep(CreateSpaBuildProject request,
            CreateBuildConfigResponse buildConfigResponse)
        {
            var bowerInstallStep = TeamCityClient.CreateBuildStep(
                TeamCityRequestBuilder.GetBowerInstallBuildStep(buildConfigResponse.Id, request.WorkingDirectory));
            return bowerInstallStep;
        }

        private CreateBuildStepResponse CreateNuGetRestoreStep(CreateSpaBuildProject request, CreateBuildConfigResponse buildConfigResponse)
        {
            var nuGetRestoreStep = TeamCityClient.CreateBuildStep(
               TeamCityRequestBuilder.GetNuGetRestoreBuildStep(buildConfigResponse.Id, request.SolutionPath));
            return nuGetRestoreStep;
        }

        private CreateBuildStepResponse CreateGruntStep(CreateSpaBuildProject request,
            CreateBuildConfigResponse buildConfigResponse)
        {
            var bowerInstallStep = TeamCityClient.CreateBuildStep(
               TeamCityRequestBuilder.GetGruntBuildStep(buildConfigResponse.Id, request.WorkingDirectory));
            return bowerInstallStep;
        }

        private CreateVcsRootResponse CreateVcsRoot(CreateSpaBuildProject request, CreateProjectResponse createProjResponse,
            string gitHubToken)
        {
            var createVcs = TeamCityRequestBuilder.GetCreateVcsRootRequest(
                request.ProjectName,
                createProjResponse.Id,
                request.RepositoryUrl,
                request.PrivateRepository, request.Branch);
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
                },
                Id = "SS_" + request.OwnerName + "_" + request.Name
            };
            var createProjResponse = TeamCityClient.CreateProject(createProject);
            return createProjResponse;
        }
    }

    [Route("/user/projects/{OwnerName}/{RepositoryName}", Verbs = "DELETE")]
    public class DeleteTeamCityProject
    {
        public string RepositoryName { get; set; }
        public string OwnerName { get; set; }

        public string ProjectId
        {
            get { return "SS_" + OwnerName + "_" + RepositoryName; }
        }
    }
}
