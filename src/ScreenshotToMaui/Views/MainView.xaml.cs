#nullable disable
using ScreenshotToMaui.ViewModels;
using System.ComponentModel;

namespace ScreenshotToMaui.Views
{
    public partial class MainView : ContentPage
    {
        readonly MainViewModel _viewModel;

        public MainView(MainViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(_viewModel is not null)
            {
                UpdateLayout();

                _viewModel.PropertyChanged += OnViewModelPropertyChanged; 
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing(); 
            
            if (_viewModel is not null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }

        void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(_viewModel.HasCode)))
            {
                UpdateLayout();
            }
        }

        void UpdateLayout()
        {
            if(_viewModel.HasCode)
            {
                Grid.SetColumn(GenerateBorder, 0);
                Grid.SetColumnSpan(GenerateBorder, 1); 
                GenerateBorder.HorizontalOptions = LayoutOptions.Center;
                GenerateBorder.Margin = new Thickness(12);
            }
            else
            {
                Grid.SetColumn(GenerateBorder, 0);
                Grid.SetColumnSpan(GenerateBorder, 3);
                GenerateBorder.HorizontalOptions = LayoutOptions.Center;
            }
        }
    }
}