﻿using System;
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

namespace Client.Features.Jobs
{
    /// <summary>
    /// Interaction logic for JobsContent.xaml
    /// </summary>
    public partial class JobsContent : UserControl
    {
        private readonly JobsContentViewModel _viewModel;
        private readonly Internal.ILogger _logger;

        public JobsContent()
        {
            _viewModel = new JobsContentViewModel();
            DataContext = _viewModel;

            InitializeComponent();

            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _viewModel.RefreshCommand.Execute(null);
        }

        private void ConfigItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && button.DataContext != null)
            {
                if (button.DataContext is Models.Job job)
                {
                    var result = new Configuration.JobConfiguration(job.Id).ShowDialog();
                    if (result == true)
                    {

                    }
                    _viewModel.RefreshCommand.Execute(null);
                }
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && button.DataContext != null)
            {
                if (button.DataContext is Models.Job job)
                {
                    Task.Run(async () => await _viewModel.DeleteJob(job)).Wait();
                    _viewModel.RefreshCommand.Execute(null);
                }
            }
        }

        private void AddJob_Click(object sender, RoutedEventArgs e)
        {
            var result = new Configuration.JobConfiguration(Guid.Empty).ShowDialog();
            if (result == true)
            {

            }
            _viewModel.RefreshCommand.Execute(null);
        }
    }
}
