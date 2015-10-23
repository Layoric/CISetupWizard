using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIWizard.ServiceModel
{
    public class CreateSpaBuildProject
    {
        public string Name { get; set; }
        public string RepositoryUrl { get; set; }
        public string SlnPath { get; set; }
    }

    public class CreateSpaBuildProjectResponse
    {
        
    }
}
