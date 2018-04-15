using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DatasetManagementTool.Models
{
    public class DataBatch
    {
        public string Dir { get; set; }
        public string Name { get; set; }
        public ObservableCollection<DataEntry> Datasets = new ObservableCollection<DataEntry>();

        public void InsertData(DataEntry d)
        {
            Datasets.Add(d);
        }

        public void RemoveData(DataEntry d)
        {
            Datasets.Remove(d);
        }
    }
}
