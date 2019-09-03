using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Features.Cache
{
    public class CacheContentViewModel : BaseViewModel
    {
        private readonly Internal.ILogger _logger;
        private readonly JobService.IJobServiceRepository _repository;
        private readonly IMainManager _mainManager;

        public CacheContentViewModel()
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (JobService.IJobServiceRepository)Bootstrap.Instance.Services.GetService(typeof(JobService.IJobServiceRepository));
            _mainManager = (IMainManager)Bootstrap.Instance.Services.GetService(typeof(IMainManager));
        }

        private ObservableCollection<JobService.Models.PathCompareValue> resultsItems;
        public ObservableCollection<JobService.Models.PathCompareValue> ResultsItems
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
                ResultsItems = new ObservableCollection<JobService.Models.PathCompareValue>(await _repository.GetAll());
                _mainManager.SetTabGridItem(ResultsItems.Count, MainWindowViewModel.Tabs.Cache);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnRefresh failed to load data");
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
                await _repository.ClearCachePathCompareValues();
                await OnRefresh();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnClear failed");
            }
        }

        public void DeleteItem(JobService.Models.PathCompareValue pathCompareValue)
        {
            try
            {
                _repository.ClearCachePathCompareValues(pathCompareValue).GetAwaiter().GetResult();
                OnRefresh().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DeleteItem failed for PathCompareValue");
            }
        }
    }
}
