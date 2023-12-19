#nullable disable
using Azure;
using Azure.AI.OpenAI;
using Newtonsoft.Json;
using ScreenshotToMaui.Models;
using System.Text;

namespace ScreenshotToMaui.Services
{
    public class OpenAIDalle3Service
    {
        public async Task<string> GetImageFromDescriptionAsync(string apiKey, string endPoint, string description)
        {
            // TODO: Use Azure.AI.OpenAI.
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

                var payload = new
                {
                    model = "dall-e-3", // the name of your DALL-E 3 deployment
                    prompt = description,
                    n = 1
                };
                  
                var response = await httpClient.PostAsync(endPoint, new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<Dalle3Root>(content);

                    return responseData.Data[0].Url;
                }
                else
                {
                    return string.Empty;
                }
            }

            /*
            try
            {
                OpenAIClient client = new(new Uri(endPoint), new AzureKeyCredential(apiKey));

                Response<ImageGenerations> imageGenerations = await client.GetImageGenerationsAsync(
                    new ImageGenerationOptions()
                    {
                        DeploymentName = "Dalle3",
                        Prompt = description,
                        Size = ImageSize.Size256x256,
                        ImageCount = 1,
                        Quality = ImageGenerationQuality.Standard
                    });

                return imageGenerations.Value.Data[0].Url.AbsoluteUri;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
            */
        }
    }
}
