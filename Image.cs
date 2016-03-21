using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBox
{
    public enum SyncStatus { OnlyLocal, OnlyServer, LocalAndServer };

    public class Image
    {
        public string Filename { get; set; }
        public SyncStatus Status { get; set; }

        public long Hash { get; set; }

        public long Size { get; set; }

        public override string ToString()
        {
            string status = Status.ToString();
            return Filename + " (" + status + ")";
        }
    }
}
