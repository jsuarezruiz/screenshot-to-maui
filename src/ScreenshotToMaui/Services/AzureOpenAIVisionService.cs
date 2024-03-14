using Azure;
using Azure.AI.OpenAI;
using System.Diagnostics;

namespace ScreenshotToMaui.Services
{
    public class AzureOpenAIVisionService
    {
        private const string INSTRUCTIONS = @"
        You are an expert developer specialized in implementing .NET MAUI apps using XAML.
        I will provide you with an image of a reference design and some instructions and it will be your job to implement the corresponding app using .NET MAUI and XAML.
        - Pay close attention to background color, text color, font size, font family, padding, margin, border, gradients, etc. in the design. Match the colors and sizes exactly.
        - If it contains text, use the exact text in the design.
        - Repeat elements as needed to match the screenshot. For example, if there are 10 items, the code should have 10 items. 
        - DO NOT LEAVE comments like 'Repeat for each item'.
        - For images, use placeholder images from https://placehold.co with 100x100 size and include a detailed description of the image in a `description` query parameter so that an image generation AI can generate the image later (e.g. https://placehold.co/100x100?description=An%20image%20of%20a%20cat). Add as many details as possible to the description.
        
        Try your best to figure out what the designer and product owner want and make it happen. If there are any questions or underspecified features, use what you know about applications, user experience, and app design patterns to 'fill in the blanks'. 
        If you're unsure of how the designs should work, take a guess—it's better for you to get it wrong than to leave things incomplete.

        Technical details:
        - Use .NET MAUI XAML.
        - Use only official .NET MAUI packages unless otherwise specified.
        - RETURN ONLY THE CODE FOR THE `MainPage.xaml` FILE.
        - Return only the ContentPage Content.
        - Set the main background color to the first used Layout.
        - Generate the CollectionView ItemsSource data in XAML using x:Array.
        - Don't include any explanations or comments.

        Remember: you love your designers and POs and want them to be happy. The more complete and impressive your app, the happier they will be. Let's think step by step. Good luck, you've got this!.";

        private const string QUESTION = "Here are the latest designs. Implement a new .NET MAUI app based on these designs and instructions."; // Set your question here

        private readonly string _gpt4VisionModelName;
        private readonly string _azureStorageConnectionString;
        private readonly string _azureStorageContainerName;

        private readonly OpenAIClient _openAIClient;
        private AzureStorageService _azureStorageService;

        /// <summary>
        /// AzureOpenAIVisionService Constructor
        /// </summary>
        /// <param name="openAIEndpoint">Azure OpenAI Endpoint</param>
        /// <param name="openAIKey">Azure OpenAI Key</param>
        /// <param name="gpt4VisionModelName">The deployed GPT4 with Vision Model Name</param>
        /// <exception cref="ArgumentNullException"></exception>
        public AzureOpenAIVisionService
            (
            string openAIEndpoint,
            string openAIKey,
            string gpt4VisionModelName,
            string azureStorageConnectionString,
            string azureStorageContainerName
            )
        {
            if (string.IsNullOrWhiteSpace(openAIEndpoint))
                throw new ArgumentNullException(nameof(openAIEndpoint));
            if (string.IsNullOrWhiteSpace(openAIKey))
                throw new ArgumentNullException(nameof(openAIKey));

            _gpt4VisionModelName = gpt4VisionModelName ?? throw new ArgumentNullException(nameof(gpt4VisionModelName));
            _azureStorageConnectionString = azureStorageConnectionString ?? throw new ArgumentNullException(nameof(azureStorageConnectionString));
            _azureStorageContainerName = azureStorageContainerName ?? throw new ArgumentNullException(nameof(azureStorageContainerName));

            _openAIClient = new(new Uri(openAIEndpoint), new AzureKeyCredential(openAIKey));
            _azureStorageService = new AzureStorageService();
        }

        /// <summary>
        /// Generates code from the provided Image Design Path
        /// </summary>
        /// <param name="imagePath">The full path to the Image Design</param>
        /// <returns>Returns .NET MAUI XAML Design</returns>
        public async Task<string> GenerateCodeFromScreenshot(string imagePath)
        {
            try
            {
                var systemMessage = new ChatRequestSystemMessage(INSTRUCTIONS);

                var imageStream = GetImageStream(imagePath);
                var fileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.jpg";
                var imageUrl = await _azureStorageService.UploadFile(imageStream, fileName, _azureStorageConnectionString, _azureStorageContainerName);

                var image = new ChatMessageImageContentItem(new Uri(imageUrl!));
                var imageMessage = new ChatRequestUserMessage(image);

                var userMessage = new ChatRequestUserMessage(QUESTION);

                var chatRequestMessages = new List<ChatRequestMessage>
                {
                    systemMessage,
                    imageMessage,
                    userMessage
                };

                ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions(_gpt4VisionModelName, chatRequestMessages);

                chatCompletionsOptions.Temperature = 0.7f;
                chatCompletionsOptions.MaxTokens = 2048;
                chatCompletionsOptions.FrequencyPenalty = 0;
                chatCompletionsOptions.ChoiceCount = 1;
                chatCompletionsOptions.PresencePenalty = 0;

                Response<ChatCompletions> completionsResponse = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);

                var response = completionsResponse.Value.Choices[0].Message.Content;

                // delete blob
                await _azureStorageService.DeleteBlob(fileName, _azureStorageConnectionString, _azureStorageContainerName);

                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        private Stream GetImageStream(string imagePath)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                using (FileStream file = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    file.CopyTo(ms);
                    ms.Position = 0;
                }
                return ms;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }
    }
}
