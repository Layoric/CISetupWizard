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
    [Route("/projects/{ProjectLocator}/buildTypes", "POST")]
    public class CreateBuildConfig : IReturn<CreateBuildConfigResponse>
    {
        public string ProjectLocator { get; set; }

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
    public class CreateBuildConfigResponse
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "projectName")]
        public string ProjectName { get; set; }
        [DataMember(Name = "projectId")]
        public string ProjectId { get; set; }
        [DataMember(Name = "href")]
        public string Href { get; set; }
        [DataMember(Name = "webUrl")]
        public string WebUrl { get; set; }
        [DataMember(Name = "project")]
        public Project Project { get; set; }
        [DataMember(Name = "vcs-root-entries")]
        public VcsRootsResponse VcsRootEntries { get; set; }
        [DataMember(Name = "settings")]
        public SettingsResponse SettingsResponse { get; set; }
        [DataMember(Name = "parameters")]
        public ParametersResponse Parameters { get; set; }
        [DataMember(Name = "steps")]
        public StepsResponse StepsResponse { get; set; }
        [DataMember(Name = "features")]
        public FeaturesResponse FeaturesResponse { get; set; }
        [DataMember(Name = "triggers")]
        public TriggersResponse TriggersResponse { get; set; }
        [DataMember(Name = "snapshot-dependencies")]
        public SnapshotDependencies SnapshotDependencies { get; set; }
        [DataMember(Name = "artifact-dependencies")]
        public ArtifactDependenciesResponse ArtifactDependenciesResponse { get; set; }
        [DataMember(Name = "agent-requirements")]
        public AgentRequirementsResponse AgentRequirementsResponse { get; set; }
        [DataMember(Name = "builds")]
        public Builds Builds { get; set; }
    }
}
