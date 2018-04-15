using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DatasetManagementTool.Models
{
    public class Manifest
    {
        public string RootDirRelativeToManifest;

        public ObservableCollection<DataBatch> DataBatches = new ObservableCollection<DataBatch>();
    }
}
