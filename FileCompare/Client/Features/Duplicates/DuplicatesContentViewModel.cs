using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Features.Duplicates
{
    public class DuplicatesContentViewModel : BaseViewModel
    {
        private readonly Internal.ILogger _logger;
        private readonly IDuplicatesRepository _repository;
        private readonly IDuplicatesManager _duplicatesManager;
        private readonly IMainManager _mainManager;

        public DuplicatesContentViewModel()
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (IDuplicatesRepository)Bootstrap.Instance.Services.GetService(typeof(IDuplicatesRepository));
            _duplicatesManager = (IDuplicatesManager)Bootstrap.Instance.Services.GetService(typeof(IDuplicatesManager));
            _duplicatesManager.RefreshData += _duplicatesManager_RefreshData;
            _mainManager = (IMainManager)Bootstrap.Instance.Services.GetService(typeof(IMainManager));
        }

        private void _duplicatesManager_RefreshData(object sender, EventArgs e)
        {
            OnRefresh().GetAwaiter().GetResult();
        }

        private ObservableCollection<ViewModels.DuplicatesResult> resultsItems;
        public ObservableCollection<ViewModels.DuplicatesResult> ResultsItems
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
                DialogDuplicateId = Guid.Empty;
                ResultsItems = new ObservableCollection<ViewModels.DuplicatesResult>(await _repository.Duplicates());
                _mainManager.SetTabGridItem(ResultsItems.Count, MainWindowViewModel.Tabs.Duplicates);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnRefresh failed to load data");
            }
        }

        public Guid DialogDuplicateId { get; set; }
        private ICommand _checkDuplicateRemoveCommand;
        public ICommand CheckDuplicateRemoveCommand
        {
            get
            {
                if (_checkDuplicateRemoveCommand == null)
                {
                    _checkDuplicateRemoveCommand = new RelayCommand(
                        p => true,
                        async p => await OnCheckDuplicateRemove());
                }
                return _checkDuplicateRemoveCommand;
            }
        }

        private async Task OnCheckDuplicateRemove()
        {
            try
            {
                if (DialogDuplicateId != Guid.Empty)
                {
                    await _repository.CheckDuplicateRemove(DialogDuplicateId);
                }
                await OnRefresh();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnCheckDuplicateRemove failed");
            }
        }

        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand(
                        p => true,
                        async p => await OnClear());
                }
                return _clearCommand;
            }
        }

        private async Task OnClear()
        {
            try
            {
                await _repository.ClearDuplicates();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnClear failed to load data");
            }
        }
    }
}
