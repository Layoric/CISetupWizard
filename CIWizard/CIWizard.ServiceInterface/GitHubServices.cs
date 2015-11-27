using System.Collections.Generic;
using ServiceStack;
using ServiceStack.Text;

namespace CIWizard.ServiceInterface
{
    [Authenticate]
    public class GitHubServices : Service
    {
        private const string GitHubApiGetReposUrl = "https://api.github.com/user/repos?access_token={0}&per_page={1}&visibility=all";
        private const string GitHubApiGetRepoUrl = "https://api.github.com/repos/{1}/{2}?access_token={0}";

        public GetGitHubRepositoriesResponse Get(GetGitHubRepositories request)
        {
            var gitHubToken = SessionAs<AuthUserSession>().GetGitHubAccessToken();
            var response = GitHubApiGetReposUrl.Fmt(gitHubToken, request.PerPage ?? "200").GetJsonFromUrl(req =>
            {
                req.UserAgent = ".NET";
            });

            var repos = new List<GitHubRepository>();

            using (JsConfig
                .With(propertyConvention: PropertyConvention.Lenient,
                    emitLowercaseUnderscoreNames: true,
                    emitCamelCaseNames: false))
            {
                repos = response.FromJson<List<GitHubRepository>>();
            }

            return new GetGitHubRepositoriesResponse
            {
                Repos = repos
            };
        }

        public GetGitHubRepositoryResponse Get(GetGitHubRepository request)
        {
            var session = SessionAs<AuthUserSession>();
            var gitHubToken = session.GetGitHubAccessToken();
            var response =
                GitHubApiGetRepoUrl.Fmt(gitHubToken, session.UserName, request.RepositoryName).GetJsonFromUrl(req =>
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
            return new GetGitHubRepositoryResponse
            {
                Repo = repo
            };
        }
    }

    [Route("/user/repos/{RepositoryName}")]
    public class GetGitHubRepository
    {
        public string RepositoryName { get; set; }
    }

    public class GetGitHubRepositoryResponse
    {
        public GitHubRepository Repo { get; set; }
    }

    public class GetGitHubRepositoriesResponse
    {
        public List<GitHubRepository> Repos { get; set; }
    }

    [Route("/user/repos")]
    public class GetGitHubRepositories
    {
        public string PerPage { get; set; }
    }

    public class GitHubOwner
    {
        public string Login { get; set; }
        public int Id { get; set; }
        public string AvatarUrl { get; set; }
        public string GravatarId { get; set; }
        public string Url { get; set; }
        public string HtmlUrl { get; set; }
        public string FollowersUrl { get; set; }
        public string FollowingUrl { get; set; }
        public string GistsUrl { get; set; }
        public string StarredUrl { get; set; }
        public string SubscriptionsUrl { get; set; }
        public string OrganizationsUrl { get; set; }
        public string ReposUrl { get; set; }
        public string EventsUrl { get; set; }
        public string ReceivedEventsUrl { get; set; }
        public string Type { get; set; }
        public bool SiteAdmin { get; set; }
    }

    public class GitHubRepository
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public GitHubOwner Owner { get; set; }
        public bool Private { get; set; }
        public string HtmlUrl { get; set; }
        public string Description { get; set; }
        public bool Fork { get; set; }
        public string Url { get; set; }
        public string ForksUrl { get; set; }
        public string KeysUrl { get; set; }
        public string CollaboratorsUrl { get; set; }
        public string TeamsUrl { get; set; }
        public string HooksUrl { get; set; }
        public string IssueEventsUrl { get; set; }
        public string EventsUrl { get; set; }
        public string AssigneesUrl { get; set; }
        public string BranchesUrl { get; set; }
        public string TagsUrl { get; set; }
        public string BlobsUrl { get; set; }
        public string GitTagsUrl { get; set; }
        public string GitRefsUrl { get; set; }
        public string TreesUrl { get; set; }
        public string StatusesUrl { get; set; }
        public string LanguagesUrl { get; set; }
        public string StargazersUrl { get; set; }
        public string ContributorsUrl { get; set; }
        public string SubscribersUrl { get; set; }
        public string SubscriptionUrl { get; set; }
        public string CommitsUrl { get; set; }
        public string GitCommitsUrl { get; set; }
        public string CommentsUrl { get; set; }
        public string IssueCommentUrl { get; set; }
        public string ContentsUrl { get; set; }
        public string CompareUrl { get; set; }
        public string MergesUrl { get; set; }
        public string ArchiveUrl { get; set; }
        public string DownloadsUrl { get; set; }
        public string IssuesUrl { get; set; }
        public string PullsUrl { get; set; }
        public string MilestonesUrl { get; set; }
        public string NotificationsUrl { get; set; }
        public string LabelsUrl { get; set; }
        public string ReleasesUrl { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string PushedAt { get; set; }
        public string GitUrl { get; set; }
        public string SshUrl { get; set; }
        public string CloneUrl { get; set; }
        public string SvnUrl { get; set; }
        public string Homepage { get; set; }
        public int Size { get; set; }
        public int StargazersCount { get; set; }
        public int WatchersCount { get; set; }
        public string Language { get; set; }
        public bool HasIssues { get; set; }
        public bool HasDownloads { get; set; }
        public bool HasWiki { get; set; }
        public bool HasPages { get; set; }
        public int ForksCount { get; set; }
        public int OpenIssuesCount { get; set; }
        public int Forks { get; set; }
        public int OpenIssues { get; set; }
        public int Watchers { get; set; }
        public string DefaultBranch { get; set; }
    }
}