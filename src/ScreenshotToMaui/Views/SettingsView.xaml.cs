using CommunityToolkit.Maui.Views;
using ScreenshotToMaui.ViewModels;

namespace ScreenshotToMaui.Views;

public partial class SettingsView : Popup
{
	public SettingsView(SettingsViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}

	void OnCloseClicked(object sender, EventArgs args)
	{
		Close();
	}
}