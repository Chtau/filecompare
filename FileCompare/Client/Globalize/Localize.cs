using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Client.Globalize
{
    public class Localize : ILocalize
    {
        public enum Language
        {
            English,
            German
        }

        private List<string> MissingLocalization = new List<string>();
        private ResourceManager _resourceManager;
        private readonly Internal.ILogger _logger;
        private readonly Internal.ISettings _settings;

        public Localize()
        {
            _resourceManager = new ResourceManager(typeof(Resource));
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _settings = (Internal.ISettings)Bootstrap.Instance.Services.GetService(typeof(Internal.ISettings));
        }

        /// <summary>
        /// get Translated Text if it exists
        /// </summary>
        /// <param name="keyValue">Text key to search in the Translation for</param>
        /// <returns></returns>
        public string GetText(string keyValue)
        {
            if (!string.IsNullOrWhiteSpace(keyValue))
            {
                try
                {
                    string tmpValue = _resourceManager.GetString(keyValue);
                    if (string.IsNullOrWhiteSpace(tmpValue))
                    {
#if DEBUG
                        if (!MissingLocalization.Contains(keyValue))
                            MissingLocalization.Add(keyValue);
                        tmpValue = "[" + keyValue + "]";
                        keyValue = tmpValue;
#endif
                    }
                    else
                        keyValue = tmpValue;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Could not load GetText value from resourceManager");
                }
            }
            return keyValue;
        }

        public void GetMissingLocalization()
        {
            string infos = "";
            if (MissingLocalization != null && MissingLocalization.Count > 0)
            {
                infos += MissingLocalization.Count + " missing Localizations" + Environment.NewLine;
                foreach (string item in MissingLocalization)
                {
                    infos += item + Environment.NewLine;
                }
                infos += "end missing Localizations";
            }
            else
            {
                infos += "No missing Localization";
            }
            System.Diagnostics.Debug.Print(infos);
        }

        public bool SetLanguage()
        {
            try
            {
                string languageCode = "en";
                switch (_settings.Culture)
                {
                    case Language.German:
                        languageCode = "de";
                        break;
                    default:
                        languageCode = "en";
                        break;
                }
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(languageCode);
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture(languageCode);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not set Localization to Thread");
                return false;
            }
        }
    }
}
