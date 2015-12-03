﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIWizard.ServiceModel;
using CIWizard.ServiceModel.Types;
using ServiceStack;
using ServiceStack.Text;

namespace CIWizard.ServiceInterface
{
    public static class Extensions
    {
        public static string GetGitHubAccessToken(this AuthUserSession authUserSession)
        {
            var githubAuthProvider =
                authUserSession.ProviderOAuthAccess.First(x => x.Provider.EqualsIgnoreCase("GitHub"));
            return githubAuthProvider.Items["access_token"];
        }

        public static bool IsReactDesktopAppRepository(this List<Tree> trees)
        {
            return trees.Any(x => x.Path.EndsWith(".AppMac")) && trees.Any(x => x.Path.EndsWith("wwwroot_build/package-deploy-winforms.bat"));
        }

        public static bool IsReactSpaRepsitory(this List<Tree> trees)
        {
            return trees.Any(x => x.Path.EndsWith("js/app.jsx")) && trees.Any(x => x.Path.EndsWith("wwwroot_build/00-install-dependencies.bat"));
        }

        public static bool IsAngularSpaRepository(this List<Tree> trees)
        {
            return trees.Any(x => x.Path.EndsWith("js/app.js")) && trees.Any(x => x.Path.EndsWith("wwwroot_build/00-install-dependencies.bat"));
        }

        public static string GetProjectWorkingDirectory(this List<Tree> trees)
        {
            string projectName = trees.GetProjectName();
            var projTree = trees.FirstOrDefault(x => x.Path.EndsWith(projectName + ".csproj"));
            if (projTree == null)
                return null;

            return projTree.Path.Substring(0, projTree.Path.LastIndexOf("/{0}".Fmt(projectName), StringComparison.Ordinal));
        }

        public static string GetProjectName(this List<Tree> trees)
        {
            string slnPath = trees.GetSolutionTree().Path;
            if (slnPath == null)
                return null;
            int indexOfFwdSlash = slnPath.IndexOf("/", StringComparison.Ordinal);
            string projName = slnPath.Substring(indexOfFwdSlash == -1 ? 0 : indexOfFwdSlash,
            slnPath.LastIndexOf(".", StringComparison.Ordinal));
            return projName;
        }

        public static Tree GetSolutionTree(this List<Tree> trees)
        {
            return trees.FirstOrDefault(x => x.Type == "blob" && x.Path.EndsWith(".sln"));
        }

        public static ServiceStackTemplateType ResolveTemplateType(this List<Tree> files)
        {
            //Try React Desktop App
            if (files.IsReactDesktopAppRepository())
                return ServiceStackTemplateType.ReactDesktop;

            if (files.IsReactSpaRepsitory())
                return ServiceStackTemplateType.ReactSpa;

            if (files.IsAngularSpaRepository())
                return ServiceStackTemplateType.AngularSpa;

            return ServiceStackTemplateType.Unknown;
        }
    }

    public static class GitHubHelper
    {
        public const string GitHubApiGetReposUrl = "https://api.github.com/user/repos?access_token={0}&per_page={1}&visibility=all";
        public const string GitHubApiGetRepoUrl = "https://api.github.com/repos/{1}/{2}?access_token={0}";
        public const string GitHubApiGetTreeUrl = "https://api.github.com/repos/{1}/{2}/git/trees/{3}?recursive=1&access_token={0}";
        public const string GitHubGitUrl = "https://github.com/{0}/{1}.git";

        public static GitHubRepository GetGitHubRepository(string owner, string repoName, string accessToken)
        {
            var response =
                GitHubApiGetRepoUrl.Fmt(accessToken, owner, repoName).GetJsonFromUrl(req =>
                {
                    req.UserAgent = ".NET";
                });
            GitHubRepository repo;
            using (JsConfig
                .With(propertyConvention: PropertyConvention.Lenient,
                    emitLowercaseUnderscoreNames: true,
                    emitCamelCaseNames: false))
            {
                repo = response.FromJson<GitHubRepository>();
            }
            return repo;
        }

        public static List<GitHubRepository> GetGitHubRepositories(string accessToken, int perPage = 200)
        {
            var response = GitHubApiGetReposUrl.Fmt(accessToken, perPage.ToString()).GetJsonFromUrl(req =>
            {
                req.UserAgent = ".NET";
            });

            List<GitHubRepository> repos;

            using (JsConfig
                .With(propertyConvention: PropertyConvention.Lenient,
                    emitLowercaseUnderscoreNames: true,
                    emitCamelCaseNames: false))
            {
                repos = response.FromJson<List<GitHubRepository>>();
            }
            return repos;
        }

        public static GetGitHubTreeResponse GetGithubFiles(string owner, string repoName, string accessToken, string branch = "master")
        {
            var response = GitHubApiGetTreeUrl.Fmt(accessToken, owner, repoName, branch).GetJsonFromUrl(req =>
            {
                req.UserAgent = ".NET";
            });
            GetGitHubTreeResponse githubResponse;
            using (JsConfig
                .With(propertyConvention: PropertyConvention.Lenient,
                    emitLowercaseUnderscoreNames: true,
                    emitCamelCaseNames: false))
            {
                githubResponse = response.FromJson<GetGitHubTreeResponse>();
            }
            return githubResponse;
        }
    }
}