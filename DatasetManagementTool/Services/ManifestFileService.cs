using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DatasetManagementTool.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace DatasetManagementTool.Services
{
    public class ManifestFileService : BindableBase, IManifestFileService
    {
        public ManifestFileService()
        {

        }
        private Manifest _manifest;
        private string _manifestPath;
        private bool _dirty;

        public void Load(string path)
        {
            // deserialize JSON directly from a file
            using (var file = File.OpenText(path))
            {
                var serializer = new JsonSerializer();
                _manifest = (Manifest) serializer.Deserialize(file, typeof(Manifest));
                _manifestPath = path;
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
            _manifest = new Manifest();
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
                serializer.Serialize(file, _manifest);
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

        public Manifest Manifest { get; private set; }
    }
}
