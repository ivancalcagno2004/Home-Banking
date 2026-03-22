using ViewModels;

namespace UI.Views.Pages;

public partial class TransactionsPage : ContentPage
{
	public TransactionsPage(TransactionsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}