using CommunityToolkit.Maui.Views;
using ScreenshotToMaui.ViewModels;

namespace ScreenshotToMaui.Views;

public partial class AboutView : Popup
{
	public AboutView(AboutViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}