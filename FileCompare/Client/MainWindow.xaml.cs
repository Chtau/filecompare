using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewModel _viewModel;
        private readonly IMainManager _mainManager;

        public MainWindow()
        {
            _mainManager = (IMainManager)Bootstrap.Instance.Services.GetService(typeof(IMainManager));

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            InitializeComponent();

            MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;
            _mainManager.TabGridItemsChanged += _mainManager_TabGridItemsChanged;
        }

        private void _mainManager_TabGridItemsChanged(object sender, EventArgs e)
        {
            switch (_viewModel.ActiveTab)
            {
                case MainWindowViewModel.Tabs.Folders:
                    _viewModel.ActiveTabRows = _mainManager.GetTabGridItemCount(MainWindowViewModel.Tabs.Folders);
                    break;
                case MainWindowViewModel.Tabs.Duplicates:
                    _viewModel.ActiveTabRows = _mainManager.GetTabGridItemCount(MainWindowViewModel.Tabs.Duplicates);
                    break;
                case MainWindowViewModel.Tabs.Jobs:
                    _viewModel.ActiveTabRows = _mainManager.GetTabGridItemCount(MainWindowViewModel.Tabs.Jobs);
                    break;
                case MainWindowViewModel.Tabs.Cache:
                    _viewModel.ActiveTabRows = _mainManager.GetTabGridItemCount(MainWindowViewModel.Tabs.Cache);
                    break;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _mainManager.ClosingApplication();
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (_viewModel != null)
            {
                _viewModel.Dispose();
                DataContext = null;
            }
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is TabItem tab)
            {
                if (tab.Name == "TabFolders")
                {
                    _viewModel.ActiveTab = MainWindowViewModel.Tabs.Folders;
                    _viewModel.ActiveTabRows = _mainManager.GetTabGridItemCount(MainWindowViewModel.Tabs.Folders);
                }
                else if (tab.Name == "TabDuplicates")
                {
                    _viewModel.ActiveTab = MainWindowViewModel.Tabs.Duplicates;
                    _viewModel.ActiveTabRows = _mainManager.GetTabGridItemCount(MainWindowViewModel.Tabs.Duplicates);
                }
                else if (tab.Name == "TabJobs")
                {
                    _viewModel.ActiveTab = MainWindowViewModel.Tabs.Jobs;
                    _viewModel.ActiveTabRows = _mainManager.GetTabGridItemCount(MainWindowViewModel.Tabs.Jobs);
                }
                else if (tab.Name == "TabCache")
                {
                    _viewModel.ActiveTab = MainWindowViewModel.Tabs.Cache;
                    _viewModel.ActiveTabRows = _mainManager.GetTabGridItemCount(MainWindowViewModel.Tabs.Cache);
                }
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            this.SettingsFlyout.IsOpen = !this.SettingsFlyout.IsOpen;
        }
    }
}
