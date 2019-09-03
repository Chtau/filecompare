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

namespace Client.Features.Cache
{
    /// <summary>
    /// Interaction logic for CacheContent.xaml
    /// </summary>
    public partial class CacheContent : UserControl
    {
        private readonly CacheContentViewModel _viewModel;
        private readonly Internal.ILogger _logger;

        public CacheContent()
        {
            _viewModel = new CacheContentViewModel();
            DataContext = _viewModel;

            InitializeComponent();

            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _viewModel.RefreshCommand.Execute(null);
        }

        private void DeleteCacheImte_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && button.DataContext != null)
            {
                if (button.DataContext is JobService.Models.PathCompareValue value)
                {
                    _viewModel.DeleteItem(value);
                }
            }
        }
    }
}
