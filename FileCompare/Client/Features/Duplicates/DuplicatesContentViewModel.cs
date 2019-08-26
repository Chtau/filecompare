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

        public DuplicatesContentViewModel()
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (IDuplicatesRepository)Bootstrap.Instance.Services.GetService(typeof(IDuplicatesRepository));
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
    }
}
