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

        public List<KeyValuePair<int, string>> CultureEnum { get; set; }
        public int CultureEnumSelected { get; set; }

        public SettingsViewModel()
        {
            _settings = (ISettings)Bootstrap.Instance.Services.GetService(typeof(ISettings));

            CultureEnumSelected = (int)_settings.Culture;
            CultureEnum = ComboBoxBindingModelBuilder.FromEnum(typeof(Globalize.Localize.Language), false);
        }
    }
}
