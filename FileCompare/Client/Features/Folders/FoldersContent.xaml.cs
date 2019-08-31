using Client.Internal;
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

namespace Client.Features.Folders
{
    /// <summary>
    /// Interaction logic for FoldersContent.xaml
    /// </summary>
    public partial class FoldersContent : UserControl
    {
        private readonly FoldersContentViewModel _viewModel;
        private readonly Internal.ILogger _logger;

        public FoldersContent()
        {
            _viewModel = new FoldersContentViewModel();
            DataContext = _viewModel;

            InitializeComponent();

            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _viewModel.RefreshCommand.Execute(null);
        }

        private void UserFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtFolder.Text = dialog.SelectedPath;
                }
            }
        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var folder = txtFolder.Text;
                Task.Run(async () =>
                {
                    if (await _viewModel.InsertCollectPath(folder))
                    {
                        _viewModel.RefreshCommand.Execute(null);
                    }
                    else
                    {

                    }
                });
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && button.DataContext != null)
            {
                if (button.DataContext is Models.CollectPath collectPath)
                {
                    Task.Run(async () => await _viewModel.DeleteCollectPath(collectPath)).Wait();
                    _viewModel.RefreshCommand.Execute(null);
                }
            }
        }

        private void OpenDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && button.DataContext != null)
            {
                if (button.DataContext is Models.CollectPath collectPath)
                {
                    try
                    {
                        StaticFolders.OpenDirectory(collectPath.Path);
                    } catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
            }
        }
    }
}
