using Azure;
using Azure.AI.OpenAI;
using System.Diagnostics;

namespace ScreenshotToMaui.Services
{
    public class AzureOpenAIDalle3Service
    {
        private readonly OpenAIClient _openAIClient;
        private readonly string _dalle3ModelName;

        /// <summary>
        /// AzureOpenAIDalle3Service Constructor
        /// </summary>
        /// <param name="openAIEndpoint">Azure OpenAI Endpoint</param>
        /// <param name="openAIKey">Azure OpenAI Key</param>
        /// <param name="dalle3ModelName">The deployed DALL.E 3 Model Name</param>
        /// <exception cref="ArgumentNullException"></exception>
        public AzureOpenAIDalle3Service(string openAIEndpoint, string openAIKey, string dalle3ModelName)
        {
            if (string.IsNullOrWhiteSpace(openAIEndpoint))
                throw new ArgumentNullException(nameof(openAIEndpoint));
            if (string.IsNullOrWhiteSpace(openAIKey))
                throw new ArgumentNullException(nameof(openAIKey));

            _dalle3ModelName = dalle3ModelName ?? throw new ArgumentNullException(nameof(dalle3ModelName));

            _openAIClient = new(new Uri(openAIEndpoint), new AzureKeyCredential(openAIKey));
        }

        /// <summary>
        /// Generates Image from description
        /// </summary>
        /// <param name="description">The description of the image</param>
        /// <param name="imageSize">The image sie e.g ImageSize.Size256x256</param>
        /// <returns>Returns the Url of the generated image</returns>
        public async Task<string> GenerateImageFromDescription(string description, ImageSize imageSize)
        {
            try
            {
                var imageGenerationOptions = new ImageGenerationOptions()
                {
                    DeploymentName = _dalle3ModelName,
                    Prompt = description,
                    // Size = ImageSize.Size256x256, // Throws a Size error that I couldn't fix
                    ImageCount = 1,
                    Style = ImageGenerationStyle.Natural,
                    Quality = ImageGenerationQuality.Standard
                };

                var imageGenerations = await _openAIClient.GetImageGenerationsAsync(imageGenerationOptions);

                var imageUri = imageGenerations.Value.Data[0].Url;

                return imageUri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }
    }
}
