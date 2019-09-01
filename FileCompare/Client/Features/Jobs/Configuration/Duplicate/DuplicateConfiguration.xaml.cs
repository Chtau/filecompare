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
using System.Windows.Shapes;

namespace Client.Features.Jobs.Configuration.Duplicate
{
    /// <summary>
    /// Interaction logic for DuplicateConfiguration.xaml
    /// </summary>
    public partial class DuplicateConfiguration : MetroWindow
    {
        private readonly DuplicateConfigurationViewModel _viewModel;
        private readonly Internal.ILogger _logger;

        public DuplicateConfiguration(Guid jobId)
        {
            _viewModel = new DuplicateConfigurationViewModel(jobId);
            DataContext = _viewModel;

            InitializeComponent();

            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            //_viewModel.RefreshCommand.Execute(null);
        }
    }
}
