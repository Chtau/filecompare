using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IMainManager
    {
        event EventHandler TabGridItemsChanged;
        void SetTabGridItem(int items, MainWindowViewModel.Tabs tab);
        int GetTabGridItemCount(MainWindowViewModel.Tabs tab);
    }
}
