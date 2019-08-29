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
        public event EventHandler<string> StatusBarInfoTextChanged;
        public event EventHandler ApplicationClosing;

        public void SetTabGridItem(int items, MainWindowViewModel.Tabs tab)
        {
            if (tabGridItems == null)
                tabGridItems = new Dictionary<MainWindowViewModel.Tabs, int>();
            if (!tabGridItems.ContainsKey(tab))
                tabGridItems.Add(tab, items);
            else
                tabGridItems[tab] = items;
            TabGridItemsChanged?.Invoke(this, EventArgs.Empty);
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

        public void SetStatusBarInfoText(string text)
        {
            StatusBarInfoTextChanged?.Invoke(this, text);
        }

        public void ClosingApplication()
        {
            ApplicationClosing?.Invoke(this, EventArgs.Empty);
        }
    }
}
