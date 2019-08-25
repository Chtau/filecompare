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

namespace Client.Features.Duplicates
{
    /// <summary>
    /// Interaction logic for DuplicatesContent.xaml
    /// </summary>
    public partial class DuplicatesContent : UserControl
    {
        private readonly DuplicatesContentViewModel _viewModel;
        private readonly Internal.ILogger _logger;

        public DuplicatesContent()
        {
            _viewModel = new DuplicatesContentViewModel();
            DataContext = _viewModel;

            InitializeComponent();

            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _viewModel.RefreshCommand.Execute(null);
        }

        private void DuplicateActionItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && button.DataContext != null)
            {
                if (button.DataContext is ViewModels.DuplicatesResult result)
                {
                    /*var result = new Configuration.JobConfiguration(job.Id).ShowDialog();
                    if (result == true)
                    {

                    }*/
                    _viewModel.RefreshCommand.Execute(null);
                }
            }
        }
    }
}
