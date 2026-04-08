using Microsoft.Extensions.Logging;
using ViewModels;

namespace UI.Views.Pages;

/// <summary>
/// Página de carga inicial. Asocia el <see cref="LoadingViewModel"/> como BindingContext
/// (la lógica de arranque se ejecuta desde el ViewModel).
/// </summary>
public partial class LoadingPage : ContentPage
{
	private readonly ILogger<LoadingPage> _logger;
	public LoadingPage(LoadingViewModel vm, ILogger<LoadingPage> logger)
	{
		InitializeComponent();
		BindingContext = vm;
		_logger = logger;
        _logger.LogInformation("========== INICIANDO TANDIL BANK ==========");
    }
}