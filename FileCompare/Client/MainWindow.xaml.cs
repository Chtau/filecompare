using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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

        public MainWindow()
        {
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            InitializeComponent();

            MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;
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
                }
                else if (tab.Name == "TabDuplicates")
                {
                    _viewModel.ActiveTab = MainWindowViewModel.Tabs.Duplicates;
                }
                else if (tab.Name == "TabJobs")
                {
                    _viewModel.ActiveTab = MainWindowViewModel.Tabs.Jobs;
                }
            }
        }
    }
}
