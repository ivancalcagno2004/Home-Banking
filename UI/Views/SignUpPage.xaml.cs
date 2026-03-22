namespace UI.Views;
using ViewModels;

public partial class SignUpPage : ContentPage
{
	public SignUpPage(SignUpViewModel vm)
	{
        InitializeComponent();
        BindingContext = vm;
	}
}