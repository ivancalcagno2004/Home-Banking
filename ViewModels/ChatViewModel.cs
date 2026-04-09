using Markdig;
using Models.DTO;
using Services;
using Services.Implementations;
using Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private readonly IGroqChatService _chatService;
        private readonly UserSession _userSession;
        public string FullName => _userSession.CurrentUser?.FullName ?? "Usuario";
        public string Initials => !string.IsNullOrEmpty(FullName) ? FullName[0].ToString() : "U";

        private bool _isChatVisible;
        public bool IsChatVisible
        {
            get => _isChatVisible;
            set { _isChatVisible = value; OnPropertyChanged(); }
        }

        private string? _mensajeEscrito;
        public string? MensajeEscrito
        {
            get => _mensajeEscrito;
            set { _mensajeEscrito = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MessageChatDTO> ListaMensajes { get; set; } = new ObservableCollection<MessageChatDTO>();

        public ICommand ActivarChatCommand { get; }
        public ICommand EnviarMensajeCommand { get; }

        public ChatViewModel(IGroqChatService chatService, UserSession userSession)
        {
            _chatService = chatService;
            _userSession = userSession;
            IsChatVisible = false;
            ActivarChatCommand = new RelayCommand(param => IsChatVisible = !IsChatVisible);
            EnviarMensajeCommand = new RelayCommand(SendMessage);

            ListaMensajes.Add(new MessageChatDTO
            {
                EsMio = false,
                Texto = "¡Hola! Soy tu asistente virtual de TandilBank. ¿En qué puedo ayudarte hoy?"
            });
        }

        private async void SendMessage(object o)
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

            string? respuestaGroq = await _chatService.SendMessageAsync(mensajeUsuario);
            string respuestaFormateada = Markdown.ToHtml(respuestaGroq ?? string.Empty);

            ListaMensajes.Add(new MessageChatDTO
            {
                EsMio = false,
                Texto = respuestaFormateada
            });
        }
    }
}