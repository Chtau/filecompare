using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Internal
{
    public class Logger : ILogger
    {
        public Logger()
        {
            Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Information()
                   .WriteTo.File(System.IO.Path.Combine(StaticFolders.GetUserFolder(), GlobalValues.LogFile),
                       rollingInterval: RollingInterval.Day,
                       rollOnFileSizeLimit: true)
                   .CreateLogger();
        }

        public void Diagnostic(string message)
        {
            Log.Debug(message);
        }

        public void Error(Exception ex)
        {
            Log.Error(ex.ToString());
        }

        public void Error(string message)
        {
            Log.Error(message);
        }

        public void Error(Exception ex, string message)
        {
            Log.Error(ex, message);
        }

        public void Info(string message)
        {
            Log.Information(message);
        }

        public void Warn(string message)
        {
            Log.Warning(message);
        }
    }
}
