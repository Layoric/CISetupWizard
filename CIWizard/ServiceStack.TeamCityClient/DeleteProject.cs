using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStack.TeamCityClient
{
    [Route("/projects/{Locator}", Verbs = "DELETE")]
    public class DeleteProject : IReturn<DeleteProjectResponse>
    {
        public string Locator { get; set; }
    }

    public class DeleteProjectResponse
    {
        
    }
}
