using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Serialization;
using ServiceStack.TeamCityClient.Types;

namespace ServiceStack.TeamCityClient
{
    /// <summary>
    /// XmlSerializer is used due to TC only accepting XML for some of the commands.
    /// Can still respond with JSON so TcXmlServiceClient is used to manage different serializations.
    ///  </summary>
    [XmlSerializerFormat]
    [XmlType(TypeName = "newBuildTypeDescription", Namespace = "")]
    [Route("/projects/{Locator}/buildTypes", "POST")]
    public class CreateBuildConfig : IReturn<CreateBuildConfigResponse>
    {
        public string Locator { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "sourceBuildTypeLocator")]
        public string SourceBuildConfigLocator { get; set; }

        [XmlAttribute(AttributeName = "copyAllAssociatedSettings")]
        public bool CopyAllSettings { get; set; }

        [XmlAttribute(AttributeName = "shareVCSRoots")]
        public bool ShareVcsRoot { get; set; }
    }

    [DataContract]
    public class CreateBuildConfigResponse : BuildTypeResponse
    {
    }
}
