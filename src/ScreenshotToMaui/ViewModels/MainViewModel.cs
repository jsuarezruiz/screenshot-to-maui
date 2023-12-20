#nullable disable
using CommunityToolkit.Maui.Core;
using ScreenshotToMaui.Extensions;
using ScreenshotToMaui.Helpers;
using ScreenshotToMaui.Services;
using System.Diagnostics;
using System.Windows.Input;

namespace ScreenshotToMaui.ViewModels
{
    public class MainViewModel : BindableObject
    {
        readonly AlertService _alertService;
        readonly OpenAIVisionService _openAIVisionService;
        readonly OpenAIDalle3Service _openAIDalle3Service;
        readonly IPopupService _popupService;

        ImageSource _screenshot;
        bool _hasScreenshot;
        bool _isGenerating;
        bool _hasCode;
        string _state;
        string _liveXaml;
        View _preview;

        Stopwatch _stopwatch;

        public MainViewModel(AlertService alertService, OpenAIVisionService openAIVisionService, OpenAIDalle3Service openAIDalle3Service, IPopupService popupService)
        {
            _alertService = alertService;
            _openAIVisionService = openAIVisionService;
            _openAIDalle3Service = openAIDalle3Service;
            _popupService = popupService;

            HasScreenshot = false;
            IsGenerating = false;
            HasCode = false;

            _stopwatch = new Stopwatch();
        }

        public ImageSource Screenshot
        {
            get { return _screenshot; }
            set
            {
                _screenshot = value;
                OnPropertyChanged();
            }
        }

        public bool HasScreenshot
        {
            get { return _hasScreenshot; }
            set
            {
                _hasScreenshot = value;
                OnPropertyChanged();
            }
        }

        public bool IsGenerating
        {
            get { return _isGenerating; }
            set
            {
                _isGenerating = value;
                OnPropertyChanged();
            }
        }

        public bool HasCode
        {
            get { return _hasCode; }
            set
            {
                _hasCode = value;
                OnPropertyChanged();
            }
        }

        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        public string LiveXaml
        {
            get { return _liveXaml; }
            set
            {
                _liveXaml = value;
                OnPropertyChanged();
                PreviewXamlAsync(_liveXaml).FireAndForget();
            }
        }

        public View Preview
        {
            get { return _preview; }
            set
            {
                _preview = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadImageCommand => new Command(async () => await LoadImageAsync());
        public ICommand ReloadCommand => new Command(async () => await ReloadAsync());
        public ICommand CopyCommand => new Command(async () => await CopyToClipboardAsync());
        public ICommand ResetCommand => new Command(Reset);
        public ICommand SettingsCommand => new Command(OpenSettings);
        public ICommand FaqCommand => new Command(OpenFaq);
        public ICommand AboutCommand => new Command(OpenAbout);

        async Task LoadImageAsync()
        {
            var result = await MediaPicker.PickPhotoAsync();
            {
                if (result == null)
                {
                    HasScreenshot = false;
                    return;
                }

                State = "Opening the screenshot";
                ImageSource imageSource = ImageSource.FromStream(() => result.OpenReadAsync().Result);
                Screenshot = imageSource;
                HasScreenshot = true;

                await GenerateCodeAsync(result.FullPath);
            }
        }

        async Task GenerateCodeAsync(string screenshotFullPath)
        {
            try
            {
                _stopwatch.Start();

                NetworkAccess accessType = Connectivity.Current.NetworkAccess;

                if (accessType == NetworkAccess.Internet)
                {
                    var apiKey = await SecureStorage.Default.GetAsync(Constants.Key);
                    var visionEndpoint = await SecureStorage.Default.GetAsync(Constants.VisionKey);
                    var dalle3Endpoint = await SecureStorage.Default.GetAsync(Constants.Dalle3Key);

                    if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(visionEndpoint))
                    {
                        await _alertService.ShowAlertAsync("No credentials", "An Azure OpenAI Api Key and endpoint are required. Check the FAQ for more information.");
                        return;
                    }

                    IsGenerating = true;
                    State = "Analyzing the screenshot...";
                    var xaml = await _openAIVisionService.GetCodeFromScreenshotAsync(apiKey, visionEndpoint, screenshotFullPath);
                    string updatedXaml;

                    if (!string.IsNullOrEmpty(dalle3Endpoint))
                    {
                        var images = ParseHelper.GetImages(xaml);
                        updatedXaml = await UpdateCodeImagesAsync(xaml, images);
                    }
                    else
                        updatedXaml = xaml;

                    var sanitizedXaml = ParseHelper.Sanitize(updatedXaml);
                    LiveXaml = sanitizedXaml;

                    HasCode = true;
                }
                else
                {
                    await _alertService.ShowAlertAsync("No internet connection", "An internet connection is necessary to generate the code.");
                }
            }
            catch (Exception ex)
            {
                await _alertService.ShowAlertAsync("An error has occurred generating the code", ex.Message);
                Reset();
            }
            finally
            {
                _stopwatch.Stop();
                TimeSpan stopwatchElapsed = _stopwatch.Elapsed;
                Debug.WriteLine($"Code generated in {stopwatchElapsed.TotalSeconds} seconds");
                IsGenerating = false;
            }
        }

        async Task<string> UpdateCodeImagesAsync(string xaml, List<string> images)
        {
            var apiKey = await SecureStorage.Default.GetAsync(Constants.Key);
            var dalle3Endpoint = await SecureStorage.Default.GetAsync(Constants.Dalle3Key);

            if (!string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(dalle3Endpoint))
            {
                Dictionary<string, string> mappedImages = new Dictionary<string, string>();
                int counter = 1;
                foreach (var image in images)
                {
                    var description = ParseHelper.GetImageDescription(image);
                    State = $"Creating image {counter} of {images.Count}";
                    var generatedImage = await _openAIDalle3Service.GetImageFromDescriptionAsync(apiKey, dalle3Endpoint, description);
                    mappedImages.Add(image, generatedImage);
                    counter++;
                }

                var result = ParseHelper.UpdateImages(xaml, mappedImages);

                return result;
            }

            return await Task.FromResult(string.Empty);
        }

        async Task PreviewXamlAsync(string xaml)
        {
            var contentPage = new ContentPage();

            try
            {
                if (string.IsNullOrEmpty(xaml))
                    return;

                string contentPageXaml = $"<?xml version='1.0' encoding='utf-8' ?><ContentPage xmlns='http://schemas.microsoft.com/dotnet/2021/maui' xmlns:x='http://schemas.microsoft.com/winfx/2009/xaml' x:Class ='ScreenshotToMaui.XamlPage'>{xaml}</ContentPage>";

                await Live.UpdatePageFromXamlAsync(contentPage, contentPageXaml);
            }
            catch (Exception exception)
            {
                // XAML Error 
                Debug.WriteLine(exception.Message);
                var xamlException = Live.GetXamlException(exception);
                await Live.UpdatePageFromXamlAsync(contentPage, xamlException);
            }

            Preview = contentPage.Content;
        }

        async Task ReloadAsync()
        {
            await PreviewXamlAsync(LiveXaml);
        }

        async Task CopyToClipboardAsync()
        {
            await Clipboard.Default.SetTextAsync(LiveXaml);
            await _alertService.ShowSnackbarAsync("Code copied correctly");
        }

        void Reset()
        {
            HasScreenshot = false;
            IsGenerating = false;
            HasCode = false;
            LiveXaml = string.Empty;
        }

        void OpenSettings()
        {
            _popupService.ShowPopup<SettingsViewModel>();
        }

        void OpenFaq()
        {
            _popupService.ShowPopup<FaqViewModel>();
        }

        void OpenAbout()
        {
            _popupService.ShowPopup<AboutViewModel>();
        }
    }
}