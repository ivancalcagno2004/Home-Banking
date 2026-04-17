using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Markdig;
using Microsoft.Extensions.Logging;
using Models.DTO;
using Services.Implementations;
using Services.Interfaces;
using System.Collections.ObjectModel;

namespace ViewModels
{
    public partial class ChatViewModel : BaseViewModel
    {
        private readonly IGroqChatService _chatService;
        private readonly UserSession _userSession;

        public string FullName => _userSession.CurrentUser?.FullName ?? "Usuario";
        public string Initials => !string.IsNullOrEmpty(FullName) ? FullName[0].ToString() : "U";

        [ObservableProperty]
        private bool _isChatVisible;

        [ObservableProperty]
        private string? _mensajeEscrito;

        public ObservableCollection<MessageChatDTO> ListaMensajes { get; } = [];

        public ChatViewModel(IGroqChatService chatService, UserSession userSession, ILogger<ChatViewModel> logger)
        {
            _chatService = chatService;
            _userSession = userSession;
            _logger = logger;
            _isChatVisible = false;

            ListaMensajes.Add(new MessageChatDTO
            {
                EsMio = false,
                Texto = "¡Hola! Soy tu asistente virtual de TandilBank. ¿En qué puedo ayudarte hoy?"
            });
        }

        [RelayCommand]
        private void ActivarChat()
        {
            IsChatVisible = !IsChatVisible;
        }

        [RelayCommand]
        private async Task EnviarMensaje()
        {
            if (string.IsNullOrWhiteSpace(MensajeEscrito)) return;

            var mensajeUsuario = MensajeEscrito;
            MensajeEscrito = string.Empty;

            ListaMensajes.Add(new MessageChatDTO
            {
                EsMio = true,
                Texto = mensajeUsuario,
                Initials = Initials
            });

            try
            {
                string? respuestaGroq = await _chatService.SendMessageAsync(mensajeUsuario);
                string respuestaFormateada = Markdown.ToHtml(respuestaGroq ?? string.Empty);

                ListaMensajes.Add(new MessageChatDTO
                {
                    EsMio = false,
                    Texto = respuestaFormateada
                });
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "Error al enviar el mensaje al servicio de chat.");
            }
        }
    }
}