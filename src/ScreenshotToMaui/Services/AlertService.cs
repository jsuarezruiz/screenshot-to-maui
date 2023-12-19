#nullable disable
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace ScreenshotToMaui.Services
{
    public class AlertService
    {
        public Task ShowAlertAsync(string title, string message, string Ok = "OK")
        {
            return Application.Current.MainPage.DisplayAlert(title, message, Ok);
        }

        public Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Accept", string cancel = "Cancel")
        {
            return Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }

        public Task ShowSnackbarAsync(string message, string Ok = "OK")
        {
            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.Black,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.White,
                CornerRadius = new CornerRadius(10)
            };

            TimeSpan duration = TimeSpan.FromSeconds(3);

            var snackbar = Snackbar.Make(message, null, Ok, duration, snackbarOptions);

            return snackbar.Show();
        }
    }
}
