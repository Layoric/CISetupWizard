using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace CIWizard.ServiceModel
{
    [Route("/user/projects/{OwnerName}/{RepositoryName}", Verbs = "DELETE")]
    public class DeleteTeamCityProject
    {
        public string RepositoryName { get; set; }
        public string OwnerName { get; set; }

        public string ProjectId
        {
            get { return "SS_" + OwnerName + "_" + RepositoryName; }
        }
    }
}
