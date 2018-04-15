using System.ComponentModel;
using DatasetManagementTool.Models;

namespace DatasetManagementTool.Services
{
    public interface IManifestFileService : INotifyPropertyChanged
    {
        void Load(string path);
        void LoadDialog();

        void New();

        void Save(string path = null);
        void SaveDialog(string filename = null);

        bool Dirty { get; }
        Manifest Manifest { get; }
    }
}
