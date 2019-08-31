using Client.Internal;
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

namespace Client.Features.Duplicates.DuplicateAction
{
    /// <summary>
    /// Interaction logic for DuplicateActionDialog.xaml
    /// </summary>
    public partial class DuplicateActionDialog : MetroWindow
    {
        private readonly DuplicateActionDialogViewModel _viewModel;
        private readonly Internal.ILogger _logger;

        public DuplicateActionDialog(Guid duplicateValueId)
        {
            _viewModel = new DuplicateActionDialogViewModel(duplicateValueId);
            DataContext = _viewModel;

            InitializeComponent();

            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _viewModel.RefreshCommand.Execute(null);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void OpenDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && button.DataContext != null)
            {
                if (button.DataContext is ViewModels.DuplicatesResultPath dupResultPath)
                {
                    try
                    {
                        StaticFolders.OpenDirectory(dupResultPath.Directory, dupResultPath.FileName, dupResultPath.Extension);
                    } catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
            }
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectAllCommand.Execute(null);
        }

        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DeleteSelectCommand.Execute(null);
        }
    }
}
