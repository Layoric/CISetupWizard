using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.TeamCityClient.Types;

namespace ServiceStack.TeamCityClient
{
    [Route("/builds")]
    public class GetBuilds : IReturn<GetBuildsResponse>
    {
    }

    [DataContract]
    public class GetBuildsResponse
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
        [DataMember(Name = "href")]
        public string Href { get; set; }
        [DataMember(Name = "build")]
        public List<Build> Builds { get; set; }
    }

    [Route("/builds/{BuildLocator}")]
    public class GetBuild : IReturn<GetBuildResponse>
    {
        public string BuildLocator { get; set; }
    }

    [DataContract]
    public class GetBuildResponse
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "buildTypeId")]
        public string BuildTypeId { get; set; }
        [DataMember(Name = "number")]
        public string Number { get; set; }
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "state")]
        public string State { get; set; }
        [DataMember(Name = "href")]
        public string Href { get; set; }
        [DataMember(Name = "webUrl")]
        public string WebUrl { get; set; }
        [DataMember(Name = "statusText")]
        public string StatusText { get; set; }
        [DataMember(Name = "buildType")]
        public BuildType BuildType { get; set; }
        [DataMember(Name = "queuedDate")]
        public string QueuedDate { get; set; }
        [DataMember(Name = "startDate")]
        public string StartDate { get; set; }
        [DataMember(Name = "finishDate")]
        public string FinishDate { get; set; }
        [DataMember(Name = "triggered")]
        public Triggered Triggered { get; set; }
        [DataMember(Name = "lastChanges")]
        public LastChanges LastChanges { get; set; }
        [DataMember(Name = "changes")]
        public Changes Changes { get; set; }
        [DataMember(Name = "revisions")]
        public RevisionsResponse Revisions { get; set; }
        [DataMember(Name = "agent")]
        public Agent Agent { get; set; }
        [DataMember(Name = "artifacts")]
        public Artifacts Artifacts { get; set; }
        [DataMember(Name = "relatedIssues")]
        public RelatedIssues RelatedIssues { get; set; }
        [DataMember(Name = "properties")]
        public Properties Properties { get; set; }
        [DataMember(Name = "statistics")]
        public Statistics Statistics { get; set; }
    }
}
