using Microsoft.Extensions.Logging;
using ViewModels;

namespace UI.Views;

public partial class SignInPage : ContentPage
{
    private readonly SignInViewModel _viewModel;
    private readonly ILogger<SignInPage> _logger;
    public SignInPage(SignInViewModel vm, ILogger<SignInPage> logger)
	{
		InitializeComponent();
        _viewModel = vm;
        _logger = logger;
		BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await _viewModel.CheckSavedUserAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "SignInPage: error al cargar credenciales guardadas");    
        }
    }
}