using DatasetManagementTool.Views;
using System.Windows;
using DatasetManagementTool.Services;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Unity;

namespace DatasetManagementTool
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            var moduleCatalog = (ModuleCatalog) ModuleCatalog;
            //moduleCatalog.AddModule(typeof(YOUR_MODULE));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IManifestFileService, ManifestFileService>(new ContainerControlledLifetimeManager());
        }
    }
}
