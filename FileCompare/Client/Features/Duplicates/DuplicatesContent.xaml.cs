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
                if (button.DataContext is ViewModels.DuplicatesResult dupResult)
                {
                    var result = new DuplicateAction.DuplicateActionDialog(dupResult.DuplicateId).ShowDialog();
                    if (result == true)
                    {
                        // we should check if this id has only 1 remaining duplicate path
                        // if so delete this result
                    }
                    _viewModel.RefreshCommand.Execute(null);
                }
            }
        }
    }
}
