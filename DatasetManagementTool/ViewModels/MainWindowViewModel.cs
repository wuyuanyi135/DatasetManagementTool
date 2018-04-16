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
        #region Privates

        private readonly IManifestFileService _manifestFileService;

        private string _title = "Dataset Management";
        private ObservableCollection<DataBatch> _dataBatchList;
        private bool _isManifestLoaded;
        private DataEntry _selectedDataEntry;
        private DataBatch _selectedBatch;

        #endregion

        #region Properties

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


        public bool IsManifestLoaded
        {
            get { return _isManifestLoaded; }
            set { SetProperty(ref _isManifestLoaded, value); }
        }

        public DataBatch SelectedBatch
        {
            get { return _selectedBatch; }
            set { SetProperty(ref _selectedBatch, value); }
        }

        public DataEntry SelectedDataEntry
        {
            get { return _selectedDataEntry; }
            set { SetProperty(ref _selectedDataEntry, value); }
        }

        #endregion


        #region Commands

        #region CanExecutes

        public bool CanExecuteWhenManifestNotNull()
        {
            return IsManifestLoaded;
        }

        #endregion

        #region Menu

        public DelegateCommand LoadCommand => new DelegateCommand(ExecuteLoadCommand);

        void ExecuteLoadCommand()
        {
            _manifestFileService.LoadDialog();
        }

        public DelegateCommand SaveCommand =>
            new DelegateCommand(ExecuteSaveCommand, CanExecuteWhenManifestNotNull)
                .ObservesProperty(() => IsManifestLoaded);

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

        public DelegateCommand SaveAsCommand =>
            new DelegateCommand(ExecuteSaveAsCommand, CanExecuteWhenManifestNotNull)
                .ObservesProperty(() => IsManifestLoaded);

        private void ExecuteSaveAsCommand()
        {
            _manifestFileService.SaveDialog();
        }

        public DelegateCommand NewCommand => new DelegateCommand(ExecuteNewCommand);

        private void ExecuteNewCommand()
        {
            _manifestFileService.New();
        }

        public DelegateCommand SaveAndExportCommand =>
            new DelegateCommand(ExecuteSaveAndExportCommand, CanExecuteWhenManifestNotNull).ObservesProperty(() =>
                IsManifestLoaded);

        private void ExecuteSaveAndExportCommand()
        {
            ExecuteSaveCommand();
            _manifestFileService.ExportDialog();
        }

        #endregion

        #region Batch Manager

        public DelegateCommand CreateBatchCommand =>
            new DelegateCommand(ExecuteCreateBatchCommand, CanExecuteWhenManifestNotNull)
                .ObservesProperty(() => IsManifestLoaded);

        private void ExecuteCreateBatchCommand()
        {
            _manifestFileService.InsertBatch(new DataBatch());
        }

        public DelegateCommand DeleteBatchCommand =>
            new DelegateCommand(ExecuteDeleteBatchCommand,
                    () => CanExecuteWhenManifestNotNull() && SelectedBatch != null)
                .ObservesProperty(() => IsManifestLoaded)
                .ObservesProperty(() => SelectedBatch);

        private void ExecuteDeleteBatchCommand()
        {
            _manifestFileService.RemoveBatch(SelectedBatch);
        }

        #endregion

        #region Image Manager

        public DelegateCommand DatasetAddImagesCommand =>
            new DelegateCommand(ExecuteDatasetAddImagesCommand, () => SelectedBatch != null).ObservesProperty(() =>
                SelectedBatch);

        private void ExecuteDatasetAddImagesCommand()
        {
            Application.Current.Dispatcher.Invoke(() =>
                _manifestFileService.AddImagesDialog(SelectedBatch));
        }

        public DelegateCommand DatasetRemoveImageCommand => 
            new DelegateCommand(ExecuteDatasetRemoveImageCommand, () => SelectedDataEntry != null).ObservesProperty(() => SelectedDataEntry);

        private void ExecuteDatasetRemoveImageCommand()
        {
            SelectedBatch.RemoveData(SelectedDataEntry);
        }

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            _manifestFileService = new ManifestFileService();
        }

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
            IsManifestLoaded = _manifestFileService.Manifest != null;
        }
    }
}