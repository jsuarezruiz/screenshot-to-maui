#nullable disable
using Newtonsoft.Json;
using ScreenshotToMaui.Models;
using System.Text;

namespace ScreenshotToMaui.Services
{
    public class OpenAIVisionService
    {
        const string INSTRUCTIONS = @"
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
        const string QUESTION = "Here are the latest designs. Implement a new .NET MAUI app based on these designs and instructions."; // Set your question here

        public async Task<string> GetCodeFromScreenshotAsync(string apiKey, string endPoint, string screenshotFullPath)
        {   
            // TODO: Use Azure.AI.OpenAI.

            var encodedImage = Convert.ToBase64String(File.ReadAllBytes(screenshotFullPath));

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
                var payload = new
                {
                    messages = new object[]
                    {
                        new 
                        {
                            role = "system",
                            content = new object[] {
                                new 
                                {
                                    type = "text",
                                    text = INSTRUCTIONS
                                }
                            }
                        },
                        new 
                        {
                            role = "user",
                            content = new object[] {
                                new 
                                {
                                    type = "image_url",
                                    image_url = new 
                                    {
                                        url = $"data:image/jpeg;base64,{encodedImage}"
                                    }
                                },
                                new 
                                {
                                    type = "text",
                                    text = QUESTION
                                }
                            }
                        }
                    },
                    temperature = 0.7,
                    top_p = 0.95,
                    max_tokens = 2000,
                    stream = false
                };

                var response = await httpClient.PostAsync(endPoint, new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<Root>(content);

                    return responseData.Choices[0].Message.Content;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
