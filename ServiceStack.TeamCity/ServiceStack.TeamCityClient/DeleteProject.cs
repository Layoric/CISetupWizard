using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStack.TeamCityClient
{
    [Route("/projects/{ProjectLocator}", Verbs = "DELETE")]
    public class DeleteProject : IReturn<DeleteProjectResponse>
    {
        public string ProjectLocator { get; set; }
    }

    public class DeleteProjectResponse
    {
        
    }
}
