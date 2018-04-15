using System.Collections.Generic;

namespace DatasetManagementTool.Models
{
    public class Manifest
    {
        public string RootDirRelativeToManifest;

        public List<DataBatch> DataBatches;
    }
}
