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
    public static class TeamCityBuildSteps
    {
        public static CreateBuildStep GetNpmInstallStepRequest(string buildConfigId,string workingDirectory)
        {
            var npmStepRequest = new CreateBuildStep
            {
                BuildTypeLocator = "id:" + buildConfigId,
                Name = "NPM Install",
                TypeId = BuidStepTypes.Npm,
                StepProperies = new CreateBuildStepProperies
                {
                    Properties = new List<CreateBuildStepProperty>
                    {
                        new CreateBuildStepProperty
                        {
                            Name = "npm_commands",
                            Value = "install"
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "teamcity.build.workingDir",
                            Value = workingDirectory
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "teamcity.step.mode",
                            Value = "default"
                        }
                    }
                }
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
                StepProperies = new CreateBuildStepProperies
                {
                    Properties = new List<CreateBuildStepProperty>
                    {
                        new CreateBuildStepProperty
                        {
                            Name = "script.content",
                            Value = "bower install"
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "teamcity.build.workingDir",
                            Value = workingDirectory
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "teamcity.step.mode",
                            Value = "default"
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "use.custom.script",
                            Value = "true"
                        },
                    }
                }
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
                StepProperies = new CreateBuildStepProperies
                {
                    Properties = new List<CreateBuildStepProperty>
                    {
                        new CreateBuildStepProperty
                        {
                            Name = "nuget.path",
                            Value = "?NuGet.CommandLine.DEFAULT.nupkg"
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "nuget.updatePackages.mode",
                            Value = "sln"
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "nuget.use.restore",
                            Value = "restore"
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "nugetCustomPath",
                            Value = "?NuGet.CommandLine.DEFAULT.nupkg"
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "nugetPathSelector",
                            Value = "?NuGet.CommandLine.DEFAULT.nupkg"
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "sln.path",
                            Value = nugetSlnPath
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "teamcity.step.mode",
                            Value = "default"
                        }
                    }
                }
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
                StepProperies = new CreateBuildStepProperies
                {
                    Properties = new List<CreateBuildStepProperty>
                    {
                        new CreateBuildStepProperty
                        {
                            Name = "jonnyzzz.grunt.file",
                            Value = workingDirectory + "/gruntfile.js"
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "jonnyzzz.grunt.mode",
                            Value = "npm"
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "jonnyzzz.grunt.tasks",
                            Value = "build"
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "teamcity.build.workingDir",
                            Value = workingDirectory
                        },
                        new CreateBuildStepProperty
                        {
                            Name = "teamcity.step.mode",
                            Value = "default"
                        }
                    }
                }
            };
            return bowerInstallBuildStep;
        }
    }
}
