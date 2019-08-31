using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client.Internal
{
    public class JobStateIdleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (int.TryParse(value.ToString(), out int eValue))
                {
                    return ((Features.Jobs.JobState)eValue) == Features.Jobs.JobState.Idle;
                }
                else if (Enum.TryParse(value.ToString(), out Features.Jobs.JobState jobState))
                {
                    return jobState == Features.Jobs.JobState.Idle;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
