using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIWizard.ServiceModel;
using ServiceStack;

namespace CIWizard.ServiceInterface
{
    [Authenticate]
    public class SessionInfoServices : Service
    {
        public object Any(SessionInfo request)
        {
            var result = SessionAs<AuthUserSession>().ConvertTo<UserSessionInfo>();
            result.ProviderOAuthAccess = null;
            return result;
        }
    }

    public class UserSessionInfo : AuthUserSession { }
}
