using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Internal
{
    public interface ISettings
    {
        Globalize.Localize.Language Culture { get; set; }
    }
}
