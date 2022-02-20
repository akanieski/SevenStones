using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SevenStones.Models.Microsoft
{
    public class Message
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("html")]
        public string Html { get; set; }

        [JsonPropertyName("markdown")]
        public string Markdown { get; set; }
    }

    public class DetailedMessage
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("html")]
        public string Html { get; set; }

        [JsonPropertyName("markdown")]
        public string Markdown { get; set; }
    }

    public class Author
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
    }

    public class Committer
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
    }

    public class Commit
    {
        [JsonPropertyName("commitId")]
        public string CommitId { get; set; }

        [JsonPropertyName("author")]
        public Author Author { get; set; }

        [JsonPropertyName("committer")]
        public Committer Committer { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class RefUpdate
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("oldObjectId")]
        public string OldObjectId { get; set; }

        [JsonPropertyName("newObjectId")]
        public string NewObjectId { get; set; }
    }

    public class Project
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("visibility")]
        public string Visibility { get; set; }

        [JsonPropertyName("lastUpdateTime")]
        public DateTime LastUpdateTime { get; set; }
    }

    public class Repository
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("project")]
        public Project Project { get; set; }

        [JsonPropertyName("defaultBranch")]
        public string DefaultBranch { get; set; }

        [JsonPropertyName("remoteUrl")]
        public string RemoteUrl { get; set; }
    }

    public class PushedBy
    {
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("uniqueName")]
        public string UniqueName { get; set; }
    }

    public class Resource
    {
        [JsonPropertyName("commits")]
        public List<Commit> Commits { get; set; }

        [JsonPropertyName("refUpdates")]
        public List<RefUpdate> RefUpdates { get; set; }

        [JsonPropertyName("repository")]
        public Repository Repository { get; set; }

        [JsonPropertyName("pushedBy")]
        public PushedBy PushedBy { get; set; }

        [JsonPropertyName("pushId")]
        public int PushId { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class Collection
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class Account
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class ResourceContainers
    {
        [JsonPropertyName("collection")]
        public Collection Collection { get; set; }

        [JsonPropertyName("account")]
        public Account Account { get; set; }

        [JsonPropertyName("project")]
        public Project Project { get; set; }
    }

    public class AzureDevOpsEvent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("eventType")]
        public string EventType { get; set; }

        [JsonPropertyName("publisherId")]
        public string PublisherId { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("detailedMessage")]
        public DetailedMessage DetailedMessage { get; set; }

        [JsonPropertyName("resource")]
        public Resource Resource { get; set; }

        [JsonPropertyName("resourceVersion")]
        public string ResourceVersion { get; set; }

        [JsonPropertyName("resourceContainers")]
        public ResourceContainers ResourceContainers { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }
    }


}
