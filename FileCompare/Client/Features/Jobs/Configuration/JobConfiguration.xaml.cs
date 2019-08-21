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

namespace Client.Features.Jobs.Configuration
{
    /// <summary>
    /// Interaction logic for JobConfiguration.xaml
    /// </summary>
    public partial class JobConfiguration : MetroWindow
    {
        private readonly JobConfigurationViewModel _viewModel;
        private readonly Internal.ILogger _logger;

        public JobConfiguration(Guid jobId)
        {
            _viewModel = new JobConfigurationViewModel(jobId);
            DataContext = _viewModel;

            InitializeComponent();

            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _viewModel.RefreshCommand.Execute(null);
        }

        private void DeletePathItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && button.DataContext != null)
            {
                if (button.DataContext is ViewModels.JobPathView path)
                {
                    Task.Run(async () => await _viewModel.DeleteCollectPath(path)).Wait();
                }
            }
        }

        private void AddPath_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddPathCommand.Execute(null);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveCommand.Execute(null);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
