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
    [XmlType(TypeName = "build", Namespace = "")]
    [Route("/buildQueue", Verbs = "POST")]
    public class TriggerBuild : IReturn<TriggerBuildResponse>
    {
        [XmlElement(ElementName = "buildType")]
        public TriggerBuildType TriggerBuildType { get; set; }
    }

    [XmlSerializerFormat]
    public class TriggerBuildType
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    public class TriggerBuildResponse
    {
        
    }
}
