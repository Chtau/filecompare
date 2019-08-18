using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Globalize
{
    public interface ILocalize
    {
        string GetText(string keyValue);
        void GetMissingLocalization();
        bool SetLanguage();
    }
}
