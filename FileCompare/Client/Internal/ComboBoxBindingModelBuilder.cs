using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Internal
{
    public static class ComboBoxBindingModelBuilder
    {
        public static List<KeyValuePair<int, string>> FromEnum(Type @enum, bool translate = true)
        {
            var _localize = (Globalize.ILocalize)Bootstrap.Instance.Services.GetService(typeof(Globalize.ILocalize));
            List<KeyValuePair<int, string>> returnValue = new List<KeyValuePair<int, string>>();
            var values = Enum.GetValues(@enum);

            foreach (int eValue in values)
            {
                returnValue.Add(
                    new KeyValuePair<int, string>(
                        eValue,
                        translate ? _localize.GetText(Enum.GetName(@enum, eValue)) : Enum.GetName(@enum, eValue)
                    )
                );
            }
            return returnValue;
        }
    }
}
