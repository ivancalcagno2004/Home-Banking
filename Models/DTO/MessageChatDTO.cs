using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DTO
{
    public class MessageChatDTO
    {
        public required string Texto { get; set; }
        public required bool EsMio { get; set; }
        public bool? EsBot => !EsMio;
        public string? Initials { get; set; }
    }
}
