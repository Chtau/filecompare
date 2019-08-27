using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Duplicates
{
    public interface IDuplicatesManager
    {
        event EventHandler RefreshData;
        void RaiseRefreshData();
    }
}
