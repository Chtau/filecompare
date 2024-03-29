﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client
{
    public class MainWindowViewModel : BaseViewModel, IDisposable
    {
        private readonly Internal.ILogger _logger;
        private readonly IMainManager _mainManager;

        public MainWindowViewModel()
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _mainManager = (IMainManager)Bootstrap.Instance.Services.GetService(typeof(IMainManager));
            _mainManager.StatusBarInfoTextChanged += _mainManager_StatusBarInfoTextChanged;

            Task.Run(OnRefresh).Wait();
        }

        private void _mainManager_StatusBarInfoTextChanged(object sender, string e)
        {
            StatusBarInfoText = e;
        }

        public enum Tabs
        {
            Folders,
            Duplicates,
            Jobs,
            Cache
        }

        private ObservableCollection<object> duplicates;

        public ObservableCollection<object> Duplicates
        {
            get { return duplicates; }
            set
            {
                duplicates = value;
                RaisePropertyChanged(nameof(Duplicates));
            }
        }

        private int activeTabRows = (int)Tabs.Duplicates;

        public int ActiveTabRows
        {
            get { return activeTabRows; }
            set
            {
                activeTabRows = value;
                RaisePropertyChanged(nameof(ActiveTabRows));
            }
        }

        private string statusBarInfoText = null;

        public string StatusBarInfoText
        {
            get { return statusBarInfoText; }
            set
            {
                statusBarInfoText = value;
                RaisePropertyChanged(nameof(StatusBarInfoText));
            }
        }

        private Tabs activeTab = Tabs.Folders;

        public Tabs ActiveTab
        {
            get { return activeTab; }
            set
            {
                activeTab = value;
                OnChangeTabRows();
                RaisePropertyChanged(nameof(ActiveTab));
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
                /*ClipDataTexts = new ObservableCollection<DAL.Models.ClipText>(await Internal.Global.Instance.DBContext.GetClipText());
                ClipDataImages = new ObservableCollection<DAL.Models.ClipImage>(await Internal.Global.Instance.DBContext.GetClipImage());
                ClipDataFiles = new ObservableCollection<DAL.Models.ClipFile>(await Internal.Global.Instance.DBContext.GetClipFile());
                Summary = new ObservableCollection<DAL.Models.Summary>(await Global.Instance.DBContext.GetSummary());*/
                OnChangeTabRows();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OnRefresh failed to load data");
            }
        }

        private void OnChangeTabRows()
        {
            switch (ActiveTab)
            {
                case Tabs.Folders:
                    /*if (Folders != null)
                        ActiveTabRows = Folders.Count;*/
                    break;
                case Tabs.Duplicates:
                    if (Duplicates != null)
                        ActiveTabRows = Duplicates.Count;
                    break;
                case Tabs.Jobs:
                    /*if (Folders != null)
                        ActiveTabRows = Folders.Count;*/
                    break;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    duplicates = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}