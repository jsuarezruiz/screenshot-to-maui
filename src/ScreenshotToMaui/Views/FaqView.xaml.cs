using CommunityToolkit.Maui.Views;
using ScreenshotToMaui.ViewModels;

namespace ScreenshotToMaui.Views;

public partial class FaqView : Popup
{
	public FaqView(FaqViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}