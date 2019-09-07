using Client.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Settings
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly ISettings _settings;
        private readonly Globalize.ILocalize _localize;

        public List<KeyValuePair<int, string>> CultureEnum { get; set; }
        public int CultureEnumSelected { get; set; }

        public SettingsViewModel()
        {
            _settings = (ISettings)Bootstrap.Instance.Services.GetService(typeof(ISettings));
            _localize = (Globalize.ILocalize)Bootstrap.Instance.Services.GetService(typeof(Globalize.ILocalize));
            CultureEnumSelected = (int)_settings.Culture;
            CultureEnum = ComboBoxBindingModelBuilder.FromEnum(typeof(Globalize.Localize.Language), false);
        }

        public void Save()
        {
            _settings.Culture = (Globalize.Localize.Language)CultureEnumSelected;
            _settings.Save();
            _localize.SetLanguage();
        }

        public void OpenDataFolder()
        {
            StaticFolders.OpenDirectory(StaticFolders.GetUserFolder());
        }
    }
}
