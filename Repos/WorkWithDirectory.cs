using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    internal class WorkWithDirectory
    {
        private DirectoryInfo directoryInfo;
        private FileInfo[] fileInfos;

        public DirectoryInfo DirectoryInfo { get => directoryInfo; set => directoryInfo = value; }
        public FileInfo[] FileInfos { get => fileInfos; set => fileInfos = value; }
    }
}
