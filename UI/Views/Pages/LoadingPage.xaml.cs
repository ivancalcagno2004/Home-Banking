using ViewModels;

namespace UI.Views.Pages;

public partial class LoadingPage : ContentPage
{
	public LoadingPage(LoadingViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}