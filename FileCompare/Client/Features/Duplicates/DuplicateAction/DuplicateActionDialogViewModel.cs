using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Features.Duplicates.DuplicateAction
{
    public class DuplicateActionDialogViewModel : BaseViewModel
    {
        private readonly Internal.ILogger _logger;
        private readonly IDuplicatesRepository _repository;

        private readonly Guid _duplicateValueId;

        public DuplicateActionDialogViewModel(Guid duplicateValueId)
        {
            _duplicateValueId = duplicateValueId;
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (IDuplicatesRepository)Bootstrap.Instance.Services.GetService(typeof(IDuplicatesRepository));
        }

        private ObservableCollection<ViewModels.DuplicatesResultPath> resultsItems;
        public ObservableCollection<ViewModels.DuplicatesResultPath> ResultsItems
        {
            get { return resultsItems; }
            set
            {
                resultsItems = value;
                RaisePropertyChanged("ResultsItems");
            }
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(
                        p => true,
                        async p => await OnRefresh());
                }
                return _refreshCommand;
            }
        }

        private async Task OnRefresh()
        {
            try
            {
                ResultsItems = new ObservableCollection<ViewModels.DuplicatesResultPath>(await _repository.DuplicatesPaths(_duplicateValueId));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnRefresh failed to load data");
            }
        }

        private ICommand _selectAllCommand;
        public ICommand SelectAllCommand
        {
            get
            {
                if (_selectAllCommand == null)
                {
                    _selectAllCommand = new RelayCommand(
                        p => true,
                        async p => await OnSelectAll());
                }
                return _selectAllCommand;
            }
        }

        private async Task OnSelectAll()
        {
            try
            {
                if (ResultsItems != null && ResultsItems.Count > 0)
                {
                    var list = new List<ViewModels.DuplicatesResultPath>();
                    foreach (var item in ResultsItems)
                    {
                        item.Checked = true;
                        list.Add(item);
                    }
                    ResultsItems = new ObservableCollection<ViewModels.DuplicatesResultPath>(list);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnSelectAll failed to set data");
            }
        }
    }
}
