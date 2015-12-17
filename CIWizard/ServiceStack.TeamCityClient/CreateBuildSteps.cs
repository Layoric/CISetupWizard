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
    [XmlType(TypeName = "step", Namespace = "")]
    [Route("/buildTypes/{BuildTypeLocator}/steps", Verbs = "POST")]
    public class CreateBuildStep : IReturn<CreateBuildStepResponse>
    {
        [XmlIgnore]
        public string BuildTypeLocator { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string TypeId { get; set; }

        [XmlElement(ElementName = "properties")]
        public CreateTeamCityProperties StepProperties { get; set; }
    }

    public class CreateBuildStepResponse
    {

    }

    [XmlSerializerFormat]
    [XmlType(TypeName = "properties", Namespace = "")]
    public class CreateTeamCityProperties
    {
        [XmlElement(ElementName = "property")]
        public List<CreateTeamCityProperty> Properties { get; set; } 
    }

    [XmlSerializerFormat]
    [XmlType(TypeName = "property", Namespace = "")]
    public class CreateTeamCityProperty
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    public class BuidStepTypes
    {
        public const string Npm = "jonnyzzz.npm";
        public const string CommandLine = "simpleRunner";
        public const string Grunt = "jonnyzzz.grunt";
        public const string NuGetInstaller = "jb.nuget.installer";
    }
}
