using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIWizard.ServiceModel;
using ServiceStack;

namespace CIWizard.ServiceInterface
{
    public class TeamCityWizardService : Service
    {
        public CreateSpaBuildProjectResponse Post(CreateSpaBuildProject request)
        {
            var response = new CreateSpaBuildProjectResponse();
            return response;
        }
    }
}
