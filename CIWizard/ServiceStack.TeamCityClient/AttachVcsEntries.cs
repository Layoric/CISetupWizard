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
    [XmlType(TypeName = "vcs-root-entries", Namespace = "")]
    [Route("/buildTypes/{BuildTypeLocator}/vcs-root-entries", Verbs = "PUT")]
    public class AttachVcsEntries : IReturn<AttachVcsEntryResponse>
    {
        [XmlIgnore]
        public string BuildTypeLocator { get; set; }

        [XmlElement(ElementName = "vcs-root-entry")]
        public List<AttachVcsRootEntry> VcsRootEntries { get; set; } 
    }

    [XmlSerializerFormat]
    [XmlType(TypeName = "vcs-root-entry", Namespace = "")]
    public class AttachVcsRootEntry
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "vcs-root")]
        public AttachVcsRoot VcsRoot { get; set; }
    }

    [XmlSerializerFormat]
    [XmlType(TypeName = "vcs-root", Namespace = "")]
    public class AttachVcsRoot
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    public class AttachVcsEntryResponse
    {
    }
}
