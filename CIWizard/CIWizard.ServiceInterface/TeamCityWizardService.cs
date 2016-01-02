using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using CIWizard.ServiceModel;
using CIWizard.ServiceModel.Types;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Logging;
using ServiceStack.TeamCityClient;

namespace CIWizard.ServiceInterface
{
    public class TeamCityWizardService : Service
    {
        public TcClient TeamCityClient { get; set; }
        public IAppSettings AppSettings { get; set; }
        public static ILog Log = LogManager.GetLogger(typeof (TeamCityWizardService));

        public CreateSpaBuildProjectResponse Post(CreateSpaBuildProject request)
        {
            Log.Info(request.ToJson());
            var session = SessionAs<AuthUserSession>();
            var gitHubToken = session.GetGitHubAccessToken();

            var createProjResponse = CreateTeamCityProject(request);

            var vcsResponse = CreateVcsRoot(request, createProjResponse, gitHubToken);

            try
            {
                if(!request.LocalOnlyApp)
                    IisHelper.AddSite(request.Name, request.HostName);
                else
                    IisHelper.AddLocalOnlySite(request.Name, request.Port);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }

            var createBuildConfig = new CreateBuildConfig
            {
                Locator = "id:" + createProjResponse.Id,
                Name = "Build and Deploy"
            };
            var buildConfigResponse = TeamCityClient.CreateBuildConfig(createBuildConfig);

            TeamCityClient.UpdateBuildConfigParameters(new UpdateBuildConfigParameters
            {
                Locator = "id:" + buildConfigResponse.Id,
                Properties = new List<CreateTeamCityBuildParameter>
                {
                    TeamCityRequestBuilder
                        .CreateTeamCityBuildParameter(buildConfigResponse.Id, "ss.msdeploy.serverAddress", "localhost",
                            "text validationMode='any' display='normal'"),
                    TeamCityRequestBuilder
                        .CreateTeamCityBuildParameter(buildConfigResponse.Id, "ss.msdeploy.iisApp", request.Name,
                            "text validationMode='any' display='normal'")
                }
            });

            TeamCityClient.UpdateBuildConfigSettings(new UpdateBuildConfigSetting
            {
                Locator = "id:" + buildConfigResponse.Id,
                SettingId = "artifactRules",
                Value = request.WorkingDirectory + "/wwwroot => " + request.WorkingDirectory + "/wwwroot"
            });

            AttachVcsToProject(buildConfigResponse, vcsResponse);
            CreateVcsTrigger(buildConfigResponse);

            CreateNpmInstallStep(request, buildConfigResponse);
            CreateBowerInstallStep(request, buildConfigResponse);
            CreateNuGetRestoreStep(request, buildConfigResponse);
            CreateGruntStep(request, buildConfigResponse);
            CreateCopyAppSettingsStep(request, buildConfigResponse);
            CreateWebDeployPackStep(request, buildConfigResponse);
            CreateWebDeployPushStep(request, buildConfigResponse);

            TeamCityClient.TriggerBuild(new TriggerBuild
            {
                TriggerBuildType = new TriggerBuildType
                {
                    Id = buildConfigResponse.Id
                }
            });

            return new CreateSpaBuildProjectResponse
            {
                ProjectId = createProjResponse.Id
            };
        }

        public void Delete(DeleteTeamCityProject request)
        {
            TeamCityClient.DeleteProject(new DeleteProject {Locator = "id:" + request.ProjectId});
        }

        public GetTeamCityUrlResponse Get(GetTeamCityUrl request)
        {
            return new GetTeamCityUrlResponse
            {
                //Strip app/rest out of configured TC api url.
                Url = AppSettings.GetString("ServerApiBaseUrl").Replace("app/rest", "")
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
            catch (WebServiceException e)
            {
                if (e.IsAny400())
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

        public GetApplicationSettingsFilesResponse Get(GetApplicationSettingsFiles request)
        {
            List<string> files = new List<string>();
            DirectoryInfo projectFolder =
                new DirectoryInfo("{0}{1}\\{2}\\".Fmt(AppSettings.GetString("ApplicationSettingsBaseFolder"),
                    request.OwnerName, request.RepositoryName));
            if (projectFolder.Exists)
            {
                files.AddRange(projectFolder.GetFiles().Select(x => x.Name));
            }

            return new GetApplicationSettingsFilesResponse
            {
                FileNames = files
            };
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

        private static List<GitHubRepository> GetConfiguredRepos(List<GitHubRepository> allGhProjects,
            GetProjectsResponse allTcProjs)
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
                builds = TeamCityClient.GetBuild(new GetBuild {BuildLocator = "project:id:" + request.ProjectId});
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

        private void CreateNpmInstallStep(CreateSpaBuildProject request, CreateBuildConfigResponse buildConfigResponse)
        {
            TeamCityClient.CreateBuildStep(
                TeamCityRequestBuilder.GetNpmInstallStepRequest(buildConfigResponse.Id, request.WorkingDirectory));
        }

        private void AttachVcsToProject(CreateBuildConfigResponse buildConfigResponse, CreateVcsRootResponse vcsResponse)
        {
            TeamCityClient.AttachVcsEntries(
                TeamCityRequestBuilder.GetAttachVcsEntriesRequest(buildConfigResponse.Id, vcsResponse.Id));
        }

        private void CreateVcsTrigger(CreateBuildConfigResponse buildConfigResponse)
        {
            TeamCityClient.CreateTrigger(TeamCityRequestBuilder.GetCreateVcsTrigger(buildConfigResponse.Id));
        }

        private void CreateBowerInstallStep(CreateSpaBuildProject request, CreateBuildConfigResponse buildConfigResponse)
        {
            TeamCityClient.CreateBuildStep(
                TeamCityRequestBuilder.GetBowerInstallBuildStep(buildConfigResponse.Id, request.WorkingDirectory));
        }

        private void CreateNuGetRestoreStep(CreateSpaBuildProject request, CreateBuildConfigResponse buildConfigResponse)
        {
            TeamCityClient.CreateBuildStep(
                TeamCityRequestBuilder.GetNuGetRestoreBuildStep(buildConfigResponse.Id, request.SolutionPath));
        }

        private void CreateGruntStep(CreateSpaBuildProject request, CreateBuildConfigResponse buildConfigResponse)
        {
            TeamCityClient.CreateBuildStep(
                TeamCityRequestBuilder.GetGruntBuildStep(buildConfigResponse.Id, request.WorkingDirectory));
        }

        private void CreateCopyAppSettingsStep(CreateSpaBuildProject request, CreateBuildConfigResponse buildConfigResponse)
        {
            TeamCityClient.CreateBuildStep(
                TeamCityRequestBuilder.GetCopyAppSettingsStep(buildConfigResponse.Id,
                    request.WorkingDirectory,
                    AppSettings.GetString("ApplicationSettingsBaseFolder"),
                    request.OwnerName,
                    request.Name));
        }

        private void CreateWebDeployPackStep(CreateSpaBuildProject request, CreateBuildConfigResponse buildConfigResponse)
        {
            TeamCityClient.CreateBuildStep(
                TeamCityRequestBuilder.GetWebDeployPackStep(buildConfigResponse.Id, request.WorkingDirectory));
        }

        private void CreateWebDeployPushStep(CreateSpaBuildProject request, CreateBuildConfigResponse buildConfigResponse)
        {
            TeamCityClient.CreateBuildStep(
                TeamCityRequestBuilder.GetWebDeployPush(buildConfigResponse.Id, request.WorkingDirectory));
        }

        private CreateVcsRootResponse CreateVcsRoot(CreateSpaBuildProject request,
            CreateProjectResponse createProjResponse,
            string gitHubToken)
        {
            var createVcs = TeamCityRequestBuilder.GetCreateVcsRootRequest(
                request.ProjectName,
                createProjResponse.Id,
                request.RepositoryUrl,
                request.PrivateRepository, request.Branch);
            // Use OAuth access_token as username as per https://github.com/blog/1270-easier-builds-and-deployments-using-git-over-https-and-oauth#using-oauth-with-git
            // Password of 'x-oauth-basic' also must be used based on how TeamCity creates the git clone command
            if (request.PrivateRepository)
            {
                createVcs.Properties.Properties.Add(new CreateVcsRootProperty
                {
                    Name = "username",
                    Value = gitHubToken
                });
                createVcs.Properties.Properties.Add(new CreateVcsRootProperty
                {
                    Name = "secure:password",
                    Value = "x-oauth-basic"
                });
            }

            var vcsResponse = TeamCityClient.CreateVcsRoot(createVcs);
            return vcsResponse;
        }

        public UpdateAppSettingsConfigResponse Post(UpdateAppSettingsConfig request)
        {
            if (Request.Files == null || Request.Files.Length == 0)
            {
                throw new HttpError(HttpStatusCode.BadRequest, "MissingFile");
            }
            var uploadedFile = Request.Files[0];
            // Thanks IE...
            string fileName = uploadedFile.FileName.IndexOf("\\", StringComparison.Ordinal) > 0
                ? uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                : uploadedFile.FileName;
            var filePath = "{0}{1}\\{2}\\{3}".Fmt(
                AppSettings.GetString("ApplicationSettingsBaseFolder"),
                request.OwnerName,
                request.RepositoryName,
                fileName);
            Log.Info("Application settings creation.\n\n Path: {0}\nFile size:{1}".Fmt(filePath,
                uploadedFile.ContentLength));
            var dir = Path.GetDirectoryName(filePath);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            uploadedFile.SaveTo(filePath);

            return new UpdateAppSettingsConfigResponse();
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
}