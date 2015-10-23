using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ServiceStack.TeamCityClient.Types;

namespace ServiceStack.TeamCityClient
{
    [Route("/vcs-roots", Verbs = "POST")]
    [XmlSerializerFormat]
    [XmlType(TypeName = "vcs-root", Namespace = "")]
    public class CreateVcsRoot : IReturn<CreateVcsRootResponse>
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "vcsName")]
        public string VcsName { get; set; }

        [XmlElement(ElementName = "project")]
        public CreateVcsRootProject Project { get; set; }

        [XmlElement(ElementName = "properties")]
        public CreateVcsRootProperties Properties { get; set; }
    }

    [XmlSerializerFormat]
    [XmlType(TypeName = "properties", Namespace = "")]
    public class CreateVcsRootProperties
    {
        [XmlAttribute(AttributeName = "count")]
        public int Count { get; set; }

        [XmlElement(ElementName = "property")]
        public List<CreateVcsRootProperty> Properties { get; set; }
    }

    [XmlSerializerFormat]
    [XmlType(TypeName = "project", Namespace = "")]
    public class CreateVcsRootProject
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlSerializerFormat]
    [XmlType(TypeName = "property", Namespace = "")]
    public class CreateVcsRootProperty
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [DataContract]
    public class CreateVcsRootResponse
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "vcsName")]
        public string VcsName { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "lastChecked")]
        public string LastChecked { get; set; }

        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "project")]
        public Project Project { get; set; }

        [DataMember(Name = "properties")]
        public PropertiesResponse Properties { get; set; }

        [DataMember(Name = "vcsRootInstances")]
        public VcsRootInstance VcsRootInstances { get; set; }
    }

    public static class VcsRootTypes
    {
        public const string Git = "jetbrains.git";
    }
}
