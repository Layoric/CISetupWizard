using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServiceStack.TeamCityClient
{
    [XmlSerializerFormat]
    [XmlType(TypeName = "newBuildTypeDescription", Namespace = "")]
    [Route("/buildTypes/{Locator}/settings/{SettingId}", "PUT")]
    public class UpdateBuildConfigSetting : IReturn<UpdateBuildConfigSettingResponse>
    {
        public string Locator { get; set; }

        public string SettingId { get; set; }

        public string Value { get; set; }
    }

    public class UpdateBuildConfigSettingResponse
    {
        
    }

}
