using Client.Internal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ISettings _settings;
        private Globalize.ILocalize _localize;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            if (Bootstrap.Instance.InitApplication())
            {
                _settings = (ISettings)Bootstrap.Instance.Services.GetService(typeof(ISettings));
                _localize = (Globalize.ILocalize)Bootstrap.Instance.Services.GetService(typeof(Globalize.ILocalize));
                _settings.Load();
                _localize.SetLanguage();
                MainWindow frmMain = new MainWindow();
                frmMain.Show();
                this.ShutdownMode = ShutdownMode.OnLastWindowClose;
            }
            else
            {

            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _localize.GetMissingLocalization();
            base.OnExit(e);
        }
    }
}
