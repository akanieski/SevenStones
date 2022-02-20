using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SevenStones.Models.GitLeaks
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class DetectResult
    {
        [JsonPropertyName("Description")]
        public string Description { get; set; }

        [JsonPropertyName("StartLine")]
        public int StartLine { get; set; }

        [JsonPropertyName("EndLine")]
        public int EndLine { get; set; }

        [JsonPropertyName("StartColumn")]
        public int StartColumn { get; set; }

        [JsonPropertyName("EndColumn")]
        public int EndColumn { get; set; }

        [JsonPropertyName("Match")]
        public string Match { get; set; }

        [JsonPropertyName("Secret")]
        public string Secret { get; set; }

        [JsonPropertyName("File")]
        public string File { get; set; }

        [JsonPropertyName("Commit")]
        public string Commit { get; set; }

        [JsonPropertyName("Entropy")]
        public float Entropy { get; set; }

        [JsonPropertyName("Author")]
        public string Author { get; set; }

        [JsonPropertyName("Email")]
        public string Email { get; set; }

        [JsonPropertyName("Date")]
        public string Date { get; set; }

        [JsonPropertyName("Message")]
        public string Message { get; set; }

        [JsonPropertyName("Tags")]
        public List<object> Tags { get; set; }

        [JsonPropertyName("RuleID")]
        public string RuleID { get; set; }
    }


}
