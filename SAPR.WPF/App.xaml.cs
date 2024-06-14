using Serilog;
using System.Configuration;
using System.Data;
using System.Windows;

namespace SAPR.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Log.Information($"Приложение запущено: {DateTime.Now}");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Log.Information($"Приложение завершено: {DateTime.Now}");
            Log.CloseAndFlush();
        }
    }

}
