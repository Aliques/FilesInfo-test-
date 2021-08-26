using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesInfo.ReportLib.Models
{
    public class FilesStatisticModel
    {
        public string ExtentionName { get; set; }
        public string MimeType { get; set; }
        public double Size { get; set; }
        public int TotalCount { get; set; }
        public double TotalSize { get; set; }

    }
}
