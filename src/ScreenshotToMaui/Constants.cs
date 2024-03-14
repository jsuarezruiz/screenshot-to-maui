namespace ScreenshotToMaui
{
    public class Constants
    {
        public const string Key = "api_key";
        public const string VisionKey = "vision_endpoint";
        public const string Dalle3Key = "dalle_endpoint";

        public const string AzureOpenAIKey = "azure_openai_key";
        public const string AzureOpenAIEndpoint = "azure_open_ai_endpoint";
        public const string AzureGPT4VisionModelName = "gpt4_vision_model_name";
        public const string AzureDalle3ModelName = "dalle3_model_name";

        public const string AzureStorageAccountName = "azure_storage_account_name";
        public const string AzureStorageAccessKey = "azure_storage_accessKey";
        public const string AzureStorageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={AzureStorageAccountName};AccountKey={AzureStorageAccessKey};EndpointSuffix=core.windows.net";
        public const string AzureStorageContainerName = "azure_storage_containerName";
    }
}
