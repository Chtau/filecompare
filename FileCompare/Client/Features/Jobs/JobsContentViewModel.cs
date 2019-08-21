using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Features.Jobs
{
    public class JobsContentViewModel : BaseViewModel
    {
        private readonly Internal.ILogger _logger;
        private readonly IJobRepository _repository;

        public JobsContentViewModel()
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (IJobRepository)Bootstrap.Instance.Services.GetService(typeof(IJobRepository));
        }

        private ObservableCollection<Models.Job> jobItems;
        public ObservableCollection<Models.Job> JobItems
        {
            get { return jobItems; }
            set
            {
                jobItems = value;
                RaisePropertyChanged("JobItems");
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
                JobItems = new ObservableCollection<Models.Job>(await _repository.GetJobs());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnRefresh failed to load data");
            }
        }

        public async Task<bool> DeleteJob(Models.Job job)
        {
            try
            {
                if (job != null)
                    return await _repository.Delete(job);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public async Task<bool> StartJob(Models.Job job)
        {
            try
            {
                if (job != null && job.JobState == JobState.Idle)
                {
                    job.JobState = JobState.Starting;
                    return await _repository.Update(job);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public async Task<bool> StopJob(Models.Job job)
        {
            try
            {
                if (job != null && job.JobState == JobState.Running)
                {
                    job.JobState = JobState.Stopping;
                    return await _repository.Update(job);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }
    }
}
