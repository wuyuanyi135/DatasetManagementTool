using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Timers;
using System.Windows;
using DatasetManagementTool.Extensions;
using DatasetManagementTool.Models;
using DatasetManagementTool.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace DatasetManagementTool.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IManifestFileService _manifestFileService;

        private string _title;
        private ObservableCollection<DataBatch> _dataBatchList;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ObservableCollection<DataBatch> DataBatchList
        {
            get => _dataBatchList;
            private set => SetProperty(ref _dataBatchList, value);
        }

        #region Commands
        public DelegateCommand LoadCommand => new DelegateCommand(ExecuteLoadCommand);

        void ExecuteLoadCommand()
        {
            _manifestFileService.LoadDialog();
        }

        public DelegateCommand SaveCommand => new DelegateCommand(ExecuteSaveCommand);

        private void ExecuteSaveCommand()
        {
            try
            {
                _manifestFileService.Save();
            }
            catch (Exception)
            {
                _manifestFileService.SaveDialog();
            }
        }

        public DelegateCommand SaveAsCommand => new DelegateCommand(ExecuteSaveAsCommand);

        private void ExecuteSaveAsCommand()
        {
            _manifestFileService.SaveDialog();
        }

        public DelegateCommand NewCommand => new DelegateCommand(ExecuteNewCommand);

        private void ExecuteNewCommand()
        {
            _manifestFileService.New();
        }

        public DelegateCommand CreateBatchCommand => new DelegateCommand(ExecuteCreateBatchCommand);

        private void ExecuteCreateBatchCommand()
        {
            _manifestFileService.InsertBatch(new DataBatch(){});
        }

        #endregion
        public MainWindowViewModel(IManifestFileService manifestFileService)
        {
            _manifestFileService = manifestFileService;

            _manifestFileService
                .OnPropertyChanges(service => service.Dirty)
                .Subscribe(new AnonymousObserver<bool>(b => Title = $"Dataset Management {(b ? "*" : "")} "));

            _manifestFileService.OnManifestLoaded += _manifestFileService_OnManifestLoaded;
            
        }

        private void _manifestFileService_OnManifestLoaded(object sender, EventArgs e)
        {
            DataBatchList = _manifestFileService.Manifest.DataBatches;
        }
    }
}