using ViewModels;

namespace UI.Views;

public partial class SignInPage : ContentPage
{
	public SignInPage(SignInViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}