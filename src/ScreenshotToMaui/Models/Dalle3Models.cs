#nullable disable
using Newtonsoft.Json;

namespace ScreenshotToMaui.Models
{
    public class Datum
    {
        [JsonProperty("revised_prompt")]
        public string RevisedPrompt { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class Dalle3Root
    {
        [JsonProperty("created")]
        public int Created { get; set; }
        [JsonProperty("data")]
        public List<Datum> Data { get; set; }
    }
}
