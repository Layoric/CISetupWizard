using System.Collections.Generic;
using ServiceStack;
using ServiceStack.TeamCityClient;

namespace CIWizard.ServiceInterface
{
    public static class TeamCityRequestBuilder
    {
        private const string BuildConfigWorkingDirPropertyName = "teamcity.build.workingDir";
        private const string StepModePropertyName = "teamcity.step.mode";

        public static CreateBuildStep GetNpmInstallStepRequest(string buildConfigId,string workingDirectory)
        {
            var npmStepRequest = new CreateBuildStep
            {
                BuildTypeLocator = "id:" + buildConfigId,
                Name = "NPM Install",
                TypeId = BuidStepTypes.Npm,
                StepProperties = new CreateTeamCityProperties()
                    .AddTeamCityProperty("npm_commands", "install\ninstall grunt\ninstall grunt-cli")
                    .AddTeamCityProperty(BuildConfigWorkingDirPropertyName,workingDirectory)
                    .AddTeamCityProperty(StepModePropertyName, "default")
            };
            return npmStepRequest;
        }

        public static CreateVcsRoot GetCreateVcsRootRequest(string projectName, string projectId, string repositoryUrl, bool isPrivate, string branch = null)
        {
            var createVcs = new CreateVcsRoot
            {
                Name = "GitHub_{0}".Fmt(projectName),
                VcsName = VcsRootTypes.Git,
                Project = new CreateVcsRootProject { Id = projectId },
                Properties = new CreateVcsRootProperties
                {
                    Properties = new List<CreateVcsRootProperty>
                    {
                        new CreateVcsRootProperty
                        {
                            Name = "url",
                            Value = repositoryUrl
                        },
                        new CreateVcsRootProperty
                        {
                            Name = "authMethod",
                            Value = isPrivate ? "PASSWORD" : "ANONYMOUS"
                        },
                        new CreateVcsRootProperty
                        {
                            Name = "branch",
                            Value = "refs/heads/{0}".Fmt(branch.IsNullOrEmpty() ? branch : "master")
                        }
                    }
                }
            };

            return createVcs;
        }

        public static AttachVcsEntries GetAttachVcsEntriesRequest(string buildConfigId, string vcsId)
        {
            var attachRequest = new AttachVcsEntries
            {
                BuildTypeLocator = "id:" + buildConfigId,
                VcsRootEntries = new List<AttachVcsRootEntry>
                {
                    new AttachVcsRootEntry
                    {
                        Id = vcsId,
                        VcsRoot = new AttachVcsRoot
                        {
                            Id = vcsId
                        }
                    }
                }
            };
            return attachRequest;
        }

        public static CreateBuildStep GetBowerInstallBuildStep(string buildConfigId, string workingDirectory)
        {
            var bowerInstallBuildStep = new CreateBuildStep
            {
                BuildTypeLocator = "id:" + buildConfigId,
                Name = "Bower Install",
                TypeId = BuidStepTypes.CommandLine,
                StepProperties = new CreateTeamCityProperties()
                    .AddTeamCityProperty("script.content", "bower install")
                    .AddTeamCityProperty(BuildConfigWorkingDirPropertyName,workingDirectory)
                    .AddTeamCityProperty(StepModePropertyName, "default")
                    .AddTeamCityProperty("use.custom.script","true")
            };
            return bowerInstallBuildStep;
        }

        public static CreateBuildStep GetNuGetRestoreBuildStep(string buildConfigId, string nugetSlnPath)
        {
            var nuGetRestoreBuildStep = new CreateBuildStep
            {
                BuildTypeLocator = "id:" + buildConfigId,
                Name = "NuGet Restore",
                TypeId = BuidStepTypes.NuGetInstaller,
                StepProperties = new CreateTeamCityProperties()
                    .AddTeamCityProperty("nuget.path", "?NuGet.CommandLine.DEFAULT.nupkg")
                    .AddTeamCityProperty("nuget.updatePackages.mode", "sln")
                    .AddTeamCityProperty("nuget.use.restore", "restore")
                    .AddTeamCityProperty("nugetCustomPath", "?NuGet.CommandLine.DEFAULT.nupkg")
                    .AddTeamCityProperty("nugetPathSelector", "?NuGet.CommandLine.DEFAULT.nupkg")
                    .AddTeamCityProperty("sln.path",nugetSlnPath)
                    .AddTeamCityProperty(StepModePropertyName, "default")
            };
            return nuGetRestoreBuildStep;
        }

        public static CreateBuildStep GetGruntBuildStep(string buildConfigId, string workingDirectory)
        {
            var bowerInstallBuildStep = new CreateBuildStep
            {
                BuildTypeLocator = "id:" + buildConfigId,
                Name = "Grunt build",
                TypeId = BuidStepTypes.Grunt,
                StepProperties = new CreateTeamCityProperties()
                    .AddTeamCityProperty("jonnyzzz.grunt.file", workingDirectory + "/gruntfile.js")
                    .AddTeamCityProperty("jonnyzzz.grunt.mode", "npm")
                    .AddTeamCityProperty("jonnyzzz.grunt.tasks","build")
                    .AddTeamCityProperty(BuildConfigWorkingDirPropertyName, workingDirectory)
                    .AddTeamCityProperty(StepModePropertyName, "default")
            };
            return bowerInstallBuildStep;
        }

        public static CreateBuildStep GetCopyAppSettingsStep(string buildConfigId, string workingDirectory, string rootAppSettingsDir, string ownerName, string repoName)
        {
            var copyAppSettingsStep = new CreateBuildStep
            {
                BuildTypeLocator = "id:" + buildConfigId,
                Name = "Copy App Settings",
                TypeId = BuidStepTypes.CommandLine,
                StepProperties = new CreateTeamCityProperties()
                    .AddTeamCityProperty(
                        "script.content",
                        "xcopy {0}{1}\\{2}\\appsettings.txt %teamcity.build.workingDir%\\wwwroot /Y".Fmt(
                            rootAppSettingsDir,
                            ownerName, 
                            repoName)
                    )
                    .AddTeamCityProperty(BuildConfigWorkingDirPropertyName,workingDirectory)
                    .AddTeamCityProperty(StepModePropertyName, "default")
                    .AddTeamCityProperty("use.custom.script", "true")
            };
            return copyAppSettingsStep;
        }

        public static CreateBuildStep GetWebDeployPackStep(string buildConfigId, string workingDirectory)
        {
            var getWebDeployPackStep = new CreateBuildStep
            {
                BuildTypeLocator = "id:" + buildConfigId,
                Name = "Web Deploy - Pack",
                TypeId = BuidStepTypes.CommandLine,
                StepProperties = new CreateTeamCityProperties()
                    .AddTeamCityProperty("command.executable", "%env.MSDeployPath%\\msdeploy.exe")
                    .AddTeamCityProperty("command.parameters", "-verb:sync -source:iisApp=\"%teamcity.build.workingDir%\\wwwroot\" -dest:package=\"%teamcity.build.workingDir%\\webapp.zip\"")
                    .AddTeamCityProperty(BuildConfigWorkingDirPropertyName, workingDirectory)
                    .AddTeamCityProperty(StepModePropertyName, "default")
            };
            return getWebDeployPackStep;
        }

        public static CreateBuildStep GetWebDeployPush(string buildConfigId, string workingDirectory,string appName, string endpoint)
        {
            var getWebDeployPush = new CreateBuildStep
            {
                BuildTypeLocator = "id:" + buildConfigId,
                Name = "Web Deploy - Push",
                TypeId = BuidStepTypes.CommandLine,
                StepProperties = new CreateTeamCityProperties()
                    .AddTeamCityProperty("command.executable", "%env.MSDeployPath%\\msdeploy.exe")
                    .AddTeamCityProperty(
                    "command.parameters", 
                    "-verb:sync -source:package=\"%teamcity.build.workingDir%\\webapp.zip\" -dest:iisApp=\"{0}\",wmsvc={1},username=%ss.msdeploy.username%,password=%ss.msdeploy.password% -allowUntrusted=true"
                            .Fmt(appName,endpoint)
                    )
                    .AddTeamCityProperty(BuildConfigWorkingDirPropertyName, workingDirectory)
                    .AddTeamCityProperty(StepModePropertyName, "default")
            };
            return getWebDeployPush;
        }

        public static CreateTeamCityProperty CreateTeamCityProperty(string name, string val)
        {
            return new CreateTeamCityProperty
            {
                Name = name,
                Value = val
            };
        }

        public static CreateTeamCityProperties AddTeamCityProperty(this CreateTeamCityProperties properties, string name,
            string val)
        {
            if(properties.Properties == null) properties.Properties = new List<CreateTeamCityProperty>();
            properties.Properties.Add(CreateTeamCityProperty(name,val));
            return properties;
        }

        public static CreateTrigger GetCreateVcsTrigger(string buildConfigId)
        {
            var createTrigger = new CreateTrigger
            {
                BuildTypeLocator = "id:" + buildConfigId,
                TypeId = "vcsTrigger",
                TriggerProperties = new CreateTeamCityProperties
                {
                    Properties = new List<CreateTeamCityProperty>
                    {
                        new CreateTeamCityProperty
                        {
                            Name = "quietPeriodMode",
                            Value = "DO_NOT_USE"
                        }
                    }
                }
            };
            return createTrigger;
        }
    }
}
