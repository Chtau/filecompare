using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Features.Jobs.Models;

namespace Client.Features.JobService
{
    public class JobService : IJobService
    {
        private readonly Internal.ILogger _logger;
        private readonly Jobs.IJobRepository _repository;

        public JobService()
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (Jobs.IJobRepository)Bootstrap.Instance.Services.GetService(typeof(Jobs.IJobRepository));
        }

        public void Run()
        {
            OnTaskRun();
        }

        private bool stopTask;

        public void Stop()
        {
            stopTask = true;
        }

        private void OnTaskRun()
        {
            try
            {
                Task.Run(async () =>
                {
                    try
                    {
                        do
                        {
                            if (!stopTask)
                                await Task.Delay(TimeSpan.FromMinutes(1));
                            if (!stopTask)
                            {
                                var jobs = await _repository.GetJobs();
                                foreach (var job in jobs)
                                {
                                    await OnJobConfigSetState(job);
                                }
                            }

                        } while (!stopTask);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                });
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private async Task OnJobConfigSetState(Job job)
        {
            try
            {
                var config = await _repository.GetJobConfiguration(job.Id);
                if (job.JobState == Jobs.JobState.Idle)
                {
                    // when idle we check config if we should start the job
                    if (OnShouldStartIdleJob(config))
                    {
                        job.JobState = Jobs.JobState.Starting;
                        await _repository.Update(job);
                    }
                } else if (job.JobState == Jobs.JobState.Stopping)
                {
                    // when stopping we stop the task for the job
                    OnStopJob(job, config);
                } else if (job.JobState == Jobs.JobState.Running)
                {
                    // when running we check against start time and max runtime in minutes
                    if (OnShouldStopRunningJob(job, config))
                    {
                        job.JobState = Jobs.JobState.Stopping;
                        await _repository.Update(job);
                    }
                } else if (job.JobState == Jobs.JobState.Starting)
                {
                    // when starting we should start the task for this job
                    OnStartJob(job, config);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private bool OnShouldStartIdleJob(JobConfiguration config)
        {
            if (config != null)
            {
                bool todaySelected = false;
                var dayOfWeek = DateTime.Now.DayOfWeek;
                if (dayOfWeek == DayOfWeek.Monday && ((config.Days & Jobs.Day.Monday) == Jobs.Day.Monday))
                    todaySelected = true;
                if (dayOfWeek == DayOfWeek.Tuesday && ((config.Days & Jobs.Day.Tuesday) == Jobs.Day.Tuesday))
                    todaySelected = true;
                if (dayOfWeek == DayOfWeek.Wednesday && ((config.Days & Jobs.Day.Wednesday) == Jobs.Day.Wednesday))
                    todaySelected = true;
                if (dayOfWeek == DayOfWeek.Thursday && ((config.Days & Jobs.Day.Thursday) == Jobs.Day.Thursday))
                    todaySelected = true;
                if (dayOfWeek == DayOfWeek.Friday && ((config.Days & Jobs.Day.Friday) == Jobs.Day.Friday))
                    todaySelected = true;
                if (dayOfWeek == DayOfWeek.Saturday && ((config.Days & Jobs.Day.Saturday) == Jobs.Day.Saturday))
                    todaySelected = true;
                if (dayOfWeek == DayOfWeek.Sunday && ((config.Days & Jobs.Day.Sunday) == Jobs.Day.Sunday))
                    todaySelected = true;
                if (todaySelected)
                {
                    if (config.Hours == DateTime.Now.Hour)
                    {
                        var currentMinute = DateTime.Now.Minute;
                        if (currentMinute >= config.Minutes && (currentMinute + 5 <= config.Minutes))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool OnShouldStopRunningJob(Job job, JobConfiguration config)
        {
            if (config != null)
            {
                if (config.MaxRuntimeMinutes != 0)
                {
                    if (job.LastExecuted.HasValue)
                    {
                        if (job.LastExecuted.Value.AddMinutes(config.MaxRuntimeMinutes) > DateTime.Now)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void OnStartJob(Job job, JobConfiguration config)
        {

        }

        private void OnStopJob(Job job, JobConfiguration config)
        {

        }

        public void StartJob(Job job)
        {
            var config = _repository.GetJobConfiguration(job.Id).GetAwaiter().GetResult();
            OnStartJob(job, config);
        }

        public void StopJob(Job job)
        {
            var config = _repository.GetJobConfiguration(job.Id).GetAwaiter().GetResult();
            OnStopJob(job, config);
        }
    }
}
