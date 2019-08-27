using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Duplicates
{
    public class DuplicatesManager : IDuplicatesManager
    {
        public event EventHandler RefreshData;

        public void RaiseRefreshData()
        {
            RefreshData?.Invoke(this, new EventArgs());
        }
    }
}
