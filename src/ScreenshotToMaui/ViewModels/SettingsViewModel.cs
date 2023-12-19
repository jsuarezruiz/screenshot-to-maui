#nullable disable
using ScreenshotToMaui.Extensions;
using System.Windows.Input;

namespace ScreenshotToMaui.ViewModels
{
    public class SettingsViewModel : BindableObject
    {
        string _apiKey;
        string _visionEndpoint;
        string _dalle3Endpoint;

        public SettingsViewModel()
        {
            LoadAsync().FireAndForget();
        }

        public string ApiKey
        {
            get { return _apiKey; }
            set
            {
                _apiKey = value;
                OnPropertyChanged();
            }
        }

        public string VisionEndpoint
        {
            get { return _visionEndpoint; }
            set
            {
                _visionEndpoint = value;
                OnPropertyChanged();
            }
        }

        public string Dalle3Endpoint
        {
            get { return _dalle3Endpoint; }
            set
            {
                _dalle3Endpoint = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand => new Command(async () => await SaveAsync());

        async Task LoadAsync()
        {
            ApiKey = await SecureStorage.Default.GetAsync(Constants.Key);
            VisionEndpoint = await SecureStorage.Default.GetAsync(Constants.VisionKey);
            Dalle3Endpoint = await SecureStorage.Default.GetAsync(Constants.Dalle3Key);
        }

        async Task SaveAsync()
        {
            if (!string.IsNullOrEmpty(ApiKey))
                await SecureStorage.Default.SetAsync(Constants.Key, ApiKey);
            if (!string.IsNullOrEmpty(VisionEndpoint))
                await SecureStorage.Default.SetAsync(Constants.VisionKey, VisionEndpoint);
            if (!string.IsNullOrEmpty(Dalle3Endpoint))
                await SecureStorage.Default.SetAsync(Constants.Dalle3Key, Dalle3Endpoint);
        }
    }
}
