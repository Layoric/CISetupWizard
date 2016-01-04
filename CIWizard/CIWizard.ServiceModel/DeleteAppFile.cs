using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace CIWizard.ServiceModel
{
    [Route("/user/projects/{OwnerName}/{RepositoryName}/files", Verbs = "DELETE")]
    public class DeleteAppFile : IReturnVoid
    {
        public string OwnerName { get; set; }
        public string RepositoryName { get; set; }

        public string FileName { get; set; }
    }
}
