using System;
using System.IO;

namespace DatasetManagementTool.Models
{
    public class DataEntry
    {
        public string File { get; set; }
        public string Dir { get; set; }
        public string Hash { get; set; }
        public DateTime AddTime { get; set; }

        public string ImagePath => Path.Combine(Dir, File);
    }
}
