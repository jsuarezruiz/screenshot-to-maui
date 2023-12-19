#nullable disable
using Newtonsoft.Json;

namespace ScreenshotToMaui.Models
{
    public class Choice
    {
        [JsonProperty("finish_details")]
        public FinishDetails FinishDetails { get; set; }
        [JsonProperty("index")]
        public int Index { get; set; }
        [JsonProperty("message")]
        public Message Message { get; set; }
        [JsonProperty("content_filter_results")]
        public ContentFilterResults ContentFilterResults { get; set; }
    }

    public class ContentFilterResults
    {
        [JsonProperty("hate")]
        public Hate Hate { get; set; }
        [JsonProperty("self_harm")]
        public SelfHarm SelfHarm { get; set; }
        [JsonProperty("sexual")] 
        public Sexual Sexual { get; set; }
        [JsonProperty("violence")] 
        public Violence Violence { get; set; }
    }

    public class FinishDetails
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("stop")]
        public string Stop { get; set; }
    }

    public class Hate
    {
        [JsonProperty("filtered")]
        public bool Filtered { get; set; }
        [JsonProperty("severity")]
        public string Severity { get; set; }
    }

    public class Message
    {
        [JsonProperty("role")]
        public string Role { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public class PromptFilterResult
    {
        [JsonProperty("prompt_index")]
        public int PromptIndex { get; set; }
        [JsonProperty("content_filter_results")]
        public ContentFilterResults ContentFilterResults { get; set; }
    }

    public class Root
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string @object { get; set; }
        [JsonProperty("created")]
        public int Created { get; set; }
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("prompt_filter_results")]
        public List<PromptFilterResult> PromptFilterResults { get; set; }
        [JsonProperty("choices")]
        public List<Choice> Choices { get; set; }
        [JsonProperty("usage")]
        public Usage Usage { get; set; }
    }

    public class SelfHarm
    {
        [JsonProperty("filtered")]
        public bool Filtered { get; set; }
        [JsonProperty("severity")]
        public string Severity { get; set; }
    }

    public class Sexual
    {
        [JsonProperty("filtered")]
        public bool Filtered { get; set; }
        [JsonProperty("severity")]
        public string Severity { get; set; }
    }

    public class Usage
    {
        [JsonProperty("prompt_tokens")]
        public int PromptTokens { get; set; }
        [JsonProperty("completion_tokens")]
        public int CompletionTokens { get; set; }
        [JsonProperty("sevetotal_tokensrity")]
        public int TotalTokens { get; set; }
    }

    public class Violence
    {
        [JsonProperty("filtered")]
        public bool Filtered { get; set; }
        [JsonProperty("severity")]
        public string Severity { get; set; }
    }
}
