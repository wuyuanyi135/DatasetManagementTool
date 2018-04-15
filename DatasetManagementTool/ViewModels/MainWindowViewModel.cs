using System.Reactive;
using System.Reactive.Linq;
using DatasetManagementTool.Extensions;
using DatasetManagementTool.Services;
using Prism.Mvvm;

namespace DatasetManagementTool.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IManifestFileService _manifestFileService;

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public MainWindowViewModel(IManifestFileService manifestFileService)
        {
            _manifestFileService = manifestFileService;

            _manifestFileService
                .OnPropertyChanges(service => service.Dirty)
                .Subscribe(new AnonymousObserver<bool>(b => Title = $"Dataset Management {(b ? "*" : "" )} "));
        }
    }
}
