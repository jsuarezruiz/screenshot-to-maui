using Azure;
using Azure.Storage.Blobs;
using System.Diagnostics;

namespace ScreenshotToMaui.Services
{
    public class AzureStorageService
    {
        public async Task<string> UploadFile(Stream stream, string fileName, string azureStorageConnectionString, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(azureStorageConnectionString);

            try
            {
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = blobContainerClient.GetBlobClient(fileName);
                if (stream != null)
                {
                    var result = await blobClient.UploadAsync(stream, true);
                    var status = result.GetRawResponse().Status;
                    stream.Close();

                    return blobClient.Uri.AbsoluteUri;
                }
                else
                {
                    throw new Exception($"The Stream is empty");
                }
            }
            catch (RequestFailedException ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task DeleteBlob(string blobName, string azureStorageConnectionString, string containerName)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = blobContainerClient.GetBlobClient(blobName);
                var response = await blobClient.DeleteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
