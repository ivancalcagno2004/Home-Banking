using ViewModels;

namespace UI.Views.Pages;

public partial class PaymentsPage : ContentPage
{
	public PaymentsPage(PaymentsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}