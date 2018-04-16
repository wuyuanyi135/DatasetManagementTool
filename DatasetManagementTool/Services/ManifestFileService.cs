using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using DatasetManagementTool.Models;
using Newtonsoft.Json;
using Prism.Mvvm;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace DatasetManagementTool.Services
{
    public class ManifestFileService : BindableBase, IManifestFileService
    {
        private Manifest _manifest;
        private string _manifestPath;
        private bool _dirty;

        public void Load(string path)
        {
            // deserialize JSON directly from a file
            using (var file = File.OpenText(path))
            {
                var serializer = new JsonSerializer();
                Manifest = (Manifest) serializer.Deserialize(file, typeof(Manifest));
                _manifestPath = path;
                OnManifestLoaded?.Invoke(this, EventArgs.Empty);
            }
        }

        public void LoadDialog()
        {
            var dlg = new OpenFileDialog()
            {
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = ".json",
                Filter = "Json File (.json) | *.json"
            };

            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                Load(dlg.FileName);
            }

            Dirty = false;
        }

        public void New()
        {
            Manifest = new Manifest();
            OnManifestLoaded?.Invoke(this, EventArgs.Empty);
            Dirty = true;
        }

        public void Save(string path = null)
        {
            if (path == null)
            {
                if (_manifestPath != null)
                {
                    path = _manifestPath;
                }
                else
                {
                    throw new InvalidOperationException("Path not provided");
                }
            }

            using (var file = File.CreateText(path))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, Manifest);
            }

            _manifestPath = path;
            Dirty = false;
        }

        public void SaveDialog(string filename = "Manifest.json")
        {
            var dlg = new SaveFileDialog()
            {
                FileName = filename,
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = ".json",
                Filter = "Json File (.json) | *.json"
            };
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                Save(dlg.FileName);
            }
        }

        public bool Dirty
        {
            get => _dirty;
            private set => SetProperty(ref _dirty, value);
        }

        public Manifest Manifest
        {
            get => _manifest;
            set => SetProperty(ref _manifest, value);
        }

        public void InsertBatch(DataBatch entry)
        {
            if (entry.Name == null)
            {
                entry.Name = AssignUniqueBatchName();
            }
            Manifest.DataBatches.Add(entry);
        }

        private string AssignUniqueBatchName()
        {
            int i = 0;
            while (true)
            {
                var name = $"Batch {i}";
                if (_manifest.DataBatches.All(batch => batch.Name != name))
                {
                    return name;
                }
                i++;
            }
        }

        public void RemoveBatch(DataBatch entry)
        {
            Manifest.DataBatches.Remove(entry);
        }

        public DataBatch GetBatch(int index)
        {
            return Manifest.DataBatches[index];
        }

        public void AddImagesDialog(DataBatch destBatch)
        {
            var dlg = new OpenFileDialog()
            {
                Multiselect = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png",
                DefaultExt = "*.jpg; *.jpeg; *.jpe; *.jfif; *.png"
            };
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                var hashCollection = ComputeHashList();
                foreach (var fileName in dlg.FileNames)
                {
                    var entry = new DataEntry();
                    var hash = ComputeHash(File.OpenRead(fileName));
                    if (hashCollection.Contains(hash))
                    {
                        MessageBox.Show($"{fileName} is already in the batches! skip.", "Duplication found.");
                        continue;
                    }
                    entry.Hash = hash;
                    entry.File = Path.GetFileName(fileName);
                    entry.Dir = Path.GetDirectoryName(fileName);
                    entry.AddTime = DateTime.Now;

                    destBatch.Datasets.Add(entry);
                }
            }
        }

        private string ComputeHash(Stream s)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(s);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        public void ExportDialog()
        {
            var dlg = new FolderBrowserDialog() {Description = "Select folder to export", ShowNewFolderButton = true};
            var result = dlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                Export(dlg.SelectedPath);
            }
        }

        public void Export(string path)
        {
            foreach (var batch in _manifest.DataBatches)
            {
                ExportBatch(path, batch);
            }
        }

        public void ExportBatch(string path, DataBatch batch)
        {
            var dirName = Path.Combine(path, batch.Name);
            if (! Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            foreach (var dataset in batch.Datasets)
            {
                var destImageFilePath = Path.Combine(dirName, dataset.File);
                if (File.Exists(destImageFilePath))
                {
                    var hash = ComputeHash(File.OpenRead(destImageFilePath));
                    if (hash == dataset.Hash)
                        continue;
                }

                File.Copy(dataset.ImagePath, destImageFilePath, true);
            }
        }

        private List<string> ComputeHashList()
        {
            List<string> hashList = new List<string>();
            foreach (var manifestDataBatch in _manifest.DataBatches)
            {
                foreach (var dataEntry in manifestDataBatch.Datasets)
                {
                    hashList.Add(dataEntry.Hash);
                }
            }

            return hashList;
        }
        public event EventHandler OnManifestLoaded;
    }
}