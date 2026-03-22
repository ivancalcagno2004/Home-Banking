using ViewModels;

namespace UI.Views.Pages;

public partial class TransferPage : ContentPage
{
	public TransferPage(TransferViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}