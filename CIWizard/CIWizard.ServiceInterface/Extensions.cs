using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace CIWizard.ServiceInterface
{
    public static class Extensions
    {
        public static string GetGitHubAccessToken(this AuthUserSession authUserSession)
        {
            var githubAuthProvider = authUserSession.ProviderOAuthAccess.First(x => x.Provider.EqualsIgnoreCase("GitHub"));
            return githubAuthProvider.Items["access_token"];
        }
    }
}
