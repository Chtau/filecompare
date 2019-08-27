using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class MainManager : IMainManager
    {

        private Dictionary<MainWindowViewModel.Tabs, int> tabGridItems;

        public event EventHandler TabGridItemsChanged;

        public void SetTabGridItem(int items, MainWindowViewModel.Tabs tab)
        {
            if (tabGridItems == null)
                tabGridItems = new Dictionary<MainWindowViewModel.Tabs, int>();
            if (!tabGridItems.ContainsKey(tab))
                tabGridItems.Add(tab, items);
            else
                tabGridItems[tab] = items;
            TabGridItemsChanged?.Invoke(this, new EventArgs());
        }

        public int GetTabGridItemCount(MainWindowViewModel.Tabs tab)
        {
            if (tabGridItems == null)
                tabGridItems = new Dictionary<MainWindowViewModel.Tabs, int>();
            if (tabGridItems.ContainsKey(tab))
            {
                return tabGridItems[tab];
            }
            return 0;
        }
    }
}
