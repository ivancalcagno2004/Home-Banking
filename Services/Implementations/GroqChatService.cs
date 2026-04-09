using DataAccess.Context;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Services.Implementations
{
    public class GroqChatService : IGroqChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = DatabaseSecrets.APIKeyGroq;

        public GroqChatService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string?> SendMessageAsync(string message)
        {
            try
            {
                var requestBody = new
                {
                    model = "llama-3.1-8b-instant",
                    messages = new[]
                    {
                        new { role = "system", content = DatabaseSecrets.GroqContext },
                        new { role = "user", content = message }
                    },
                    temperature = 0.5
                };

                var response = await _httpClient.PostAsJsonAsync("https://api.groq.com/openai/v1/chat/completions", requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    string errorReal = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"\n=== Groq dice que le mandamos algo mal ===");
                    Debug.WriteLine(errorReal);
                    Debug.WriteLine($"===========================================\n");

                    return "Uy, la IA no entendió el formato. Revisá la consola de salida.";
                }

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(responseString);

                string? respuestaBot = jsonDocument.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return respuestaBot;
            }
            catch (Exception ex) {
                Debug.WriteLine($"[GroqChatService] Error al enviar mensaje: {ex.Message}");
                return "Perdón, estoy teniendo problemas para conectarme al banco en este momento.";
            }
        }
    }
}
