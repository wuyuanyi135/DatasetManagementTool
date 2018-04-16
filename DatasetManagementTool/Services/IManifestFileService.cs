using System;
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
        void SaveDialog(string filename = "Manifest.json");

        bool Dirty { get; }
        Manifest Manifest { get; }

        void InsertBatch(DataBatch entry);
        void RemoveBatch(DataBatch entry);
        DataBatch GetBatch(int index);

        event EventHandler OnManifestLoaded;
        void AddImagesDialog(DataBatch destBatch);
        void ExportDialog();
    }
}
