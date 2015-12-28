using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace CIWizard.ServiceModel
{
    [Route("/teamcity/url")]
    public class GetTeamCityUrl
    {

    }

    public class GetTeamCityUrlResponse
    {
        public string Url { get; set; }
    }
}
