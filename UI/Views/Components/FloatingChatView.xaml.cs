using ViewModels;

namespace UI.Views.Components;

public partial class FloatingChatView : ContentView
{
	public FloatingChatView()
	{
		InitializeComponent();

    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler != null && Handler.MauiContext != null)
        {
            BindingContext = Handler.MauiContext.Services.GetService<ChatViewModel>();
        }
    }
}