using System;
using System.Collections.Generic;           // <-- para List<T>
using System.Linq;                          // <-- para Min()
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;              // <-- para AuthenticationHeaderValue
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MathBot_CC_SP.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly OpenAIChatService _chatService;
        private readonly ILogger<EchoBot> _logger;

        public EchoBot(OpenAIChatService chatService, ILogger<EchoBot> logger)
        {
            _chatService = chatService;
            _logger = logger;
            _logger.LogInformation("EchoBot instanciado y listo.");
        }

        protected override async Task OnMessageActivityAsync(
            ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var userInput = turnContext.Activity.Text?.Trim() ?? string.Empty;
            _logger.LogInformation("Usuario dijo: {Text}", userInput);

            try
            {
                var respuesta = await _chatService.SendMessageAsync(userInput);
                await turnContext.SendActivityAsync(
                    MessageFactory.Text(respuesta),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en OpenAIChatService");
                var detalle = $"❌ Exception: {ex.Message}\n```{ex.StackTrace}```";
                await turnContext.SendActivityAsync(
                    MessageFactory.Text(detalle),
                    cancellationToken);
            }
        }
    }

    public class OpenAIChatService
    {
        private readonly HttpClient _client;
        private readonly string _deploymentName;
        private readonly string _apiVersion;

        public OpenAIChatService(HttpClient client, IConfiguration config, ILogger<OpenAIChatService> logger)
        {
            _client = client;
            _deploymentName = config["OpenAI:DeploymentName"]!;
            _apiVersion = config["OpenAI:ApiVersion"]!;
        }

        public async Task<string> SendMessageAsync(string prompt)
        {
            var route = $"openai/deployments/{_deploymentName}/chat/completions?api-version={_apiVersion}";
            var payload = new { messages = new[] { new { role = "user", content = prompt } } };
            using var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _client.PostAsync(route, content);
            response.EnsureSuccessStatusCode();

            var doc = await response.Content.ReadFromJsonAsync<JsonDocument>();
            return doc!.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString() ?? string.Empty;
        }
    }
}