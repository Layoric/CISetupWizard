using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStack.TeamCityClient
{
    public class TeamCityClient
    {
        private JsonServiceClient _jsonServiceClient;

        public TeamCityClient(string baseUrl, string userName, string password)
        {
            _jsonServiceClient = new JsonServiceClient(baseUrl)
            {
                UserName = userName,
                Password = password,
                StoreCookies = true
            };
        }

        public void CreateProject(CreateProject project)
        {
            
        }
    }

    public class CreateProject
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string ParentLocator { get; set; }
    }
}
