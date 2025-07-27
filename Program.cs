using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MathBot_CC_SP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // <-- Aquí habilitamos el logging a consola
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();                            // Quitamos proveedores por defecto
                    logging.AddConsole();                                // Añadimos consola
                    logging.SetMinimumLevel(LogLevel.Debug);       // Nivel mínimo de información
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
