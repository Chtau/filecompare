using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Globalize;
using Newtonsoft.Json;

namespace Client.Internal
{
    public class Settings : ISettings
    {
        private readonly Internal.ILogger _logger;

        public Settings()
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            Culture = Localize.Language.English;
            DBPath = System.IO.Path.Combine(StaticFolders.GetUserFolder(), GlobalValues.DBFileName);
        }

        public Localize.Language Culture { get; set; }
        public string DBPath { get; set; }

        public void Save()
        {
            try
            {
                var value = JsonConvert.SerializeObject(this);
                var path = System.IO.Path.Combine(StaticFolders.GetUserFolder(), GlobalValues.SettingsFileName);
                System.IO.File.WriteAllText(path, value);
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void Load()
        {
            try
            {
                var path = System.IO.Path.Combine(StaticFolders.GetUserFolder(), GlobalValues.SettingsFileName);
                if (System.IO.File.Exists(path))
                {
                    var settings = JsonConvert.DeserializeObject<Settings>(System.IO.File.ReadAllText(path));
                    Culture = settings.Culture;
                    DBPath = settings.DBPath;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
