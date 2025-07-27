using System;
using MathBot_CC_SP.Bots;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MathBot_CC_SP
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            // 1) Controllers + JSON
            services.AddControllers().AddNewtonsoftJson();

            // 2) Credenciales del bot
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // 3) Typed client para OpenAIChatService
            var openAIConfig = Configuration.GetSection("OpenAI");
            services.AddHttpClient<OpenAIChatService>(client =>
            {
                client.BaseAddress = new Uri(openAIConfig["Endpoint"]);
                client.DefaultRequestHeaders.Add("api-key", openAIConfig["ApiKey"]);
            });

            // 4) Inyección del Bot
            services.AddSingleton<IBot, EchoBot>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDefaultFiles()
               .UseStaticFiles()
               .UseRouting()
               .UseAuthorization()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               });
        }
    }
}
