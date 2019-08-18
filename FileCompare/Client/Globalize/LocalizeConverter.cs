using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client.Globalize
{
    public class LocalizeConverter : IValueConverter
    {
        private readonly ILocalize _localize;

        public LocalizeConverter()
        {
            _localize = (ILocalize)Bootstrap.Instance.Services.GetService(typeof(ILocalize));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (_localize != null)
                return _localize.GetText(value?.ToString());
            else
                return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
