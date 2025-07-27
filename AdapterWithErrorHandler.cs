// Gusing Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MathBot_CC_SP.Bots
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(IConfiguration config, ILogger<BotFrameworkHttpAdapter> logger)
            : base(config, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // 1) Log completo en consola
                logger.LogError(exception, "[OnTurnError] Excepción no manejada");

                // 2) Enviar detalles al usuario
                await turnContext.SendActivityAsync(
                    $"❌ Exception: {exception.Message}");
                await turnContext.SendActivityAsync(
                    $"```\n{exception.StackTrace}\n```");

                // 3) Traza para el Emulator
                await turnContext.TraceActivityAsync(
                    "OnTurnError Trace",
                    exception.ToString(),
                    "https://www.botframework.com/schemas/error",
                    "TurnError");
            };
        }
    }
}