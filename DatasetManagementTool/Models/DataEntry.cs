using System;

namespace DatasetManagementTool.Models
{
    public class DataEntry
    {
        public string File { get; set; }
        public string Dir { get; set; }
        public string Hash { get; set; }
        public DateTime AddTime { get; set; }
    }
}
