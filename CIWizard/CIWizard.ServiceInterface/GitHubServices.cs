using System.Collections.Generic;
using System.Linq;
using CIWizard.ServiceModel;
using CIWizard.ServiceModel.Types;
using ServiceStack;
using ServiceStack.Text;

namespace CIWizard.ServiceInterface
{
    [Authenticate]
    public class GitHubServices : Service
    {
        public GetGitHubRepositoriesResponse Get(GetGitHubRepositories request)
        {
            var gitHubToken = SessionAs<AuthUserSession>().GetGitHubAccessToken();
            return new GetGitHubRepositoriesResponse
            {
                Repos = GitHubHelper.GetGitHubRepositories(gitHubToken)
            };
        }

        public GetGitHubRepositoryResponse Get(GetGitHubRepository request)
        {
            var session = SessionAs<AuthUserSession>();
            var gitHubToken = session.GetGitHubAccessToken();
            return new GetGitHubRepositoryResponse
            {
                Repo = GitHubHelper.GetGitHubRepository(request.OwnerName,request.RepositoryName,gitHubToken)
            };
        }

        public object Get(GetSolutionDetails request)
        {
            var session = SessionAs<AuthUserSession>();
            var gitHubToken = session.GetGitHubAccessToken();
            string branch = request.Branch ?? "master";
            var githubFiles = GitHubHelper.GetGithubFiles(request.OwnerName, request.RepositoryName, gitHubToken,
                request.Branch ?? "master");
            var gitHubRepoDetails = GitHubHelper.GetGitHubRepository(request.OwnerName, request.RepositoryName,
                gitHubToken);
            var solutionPathTree = githubFiles.Tree.FirstOrDefault(x => x.Type == "blob" && x.Path.EndsWith(".sln"));
            if (solutionPathTree == null)
            {
                throw HttpError.NotFound("No valid solution found");
            }
            string solutionPath = solutionPathTree.Path;
            return new GetSolutionFilePathResponse
            {
                SolutionPath = solutionPath,
                Branch = branch,
                RepositoryUrl = GitHubHelper.GitHubGitUrl.Fmt(request.OwnerName,request.RepositoryName),
                TemplateType = githubFiles.Tree.ResolveTemplateType(),
                ProjectWorkingDirectory = githubFiles.Tree.GetProjectWorkingDirectory(),
                ProjectName = githubFiles.Tree.GetProjectName(),
                PrivateRepository = gitHubRepoDetails.Private
            };
        }
    }
}