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
    [XmlType(TypeName = "trigger", Namespace = "")]
    [Route("/buildTypes/{BuildTypeLocator}/triggers", Verbs = "POST")]
    public class CreateTrigger : IReturn<CreateTriggerResponse>
    {
        [XmlIgnore]
        public string BuildTypeLocator { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string TypeId { get; set; }

        [XmlElement(ElementName = "properties")]
        public CreateTeamCityProperies TriggerProperties { get; set; }
    }

    public class CreateTriggerResponse
    {
        
    }
}
