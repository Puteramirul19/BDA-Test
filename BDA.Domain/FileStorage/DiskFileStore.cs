using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDA.FileStorage
{
    /// <summary>
    /// An implementation of the IFileStore that stores files in the disk file system.
    /// </summary>
    public class DiskFileStore : IFileStore
    {
        public string StorageDir { get; set; }

        public DiskFileStore(string storageDir)
        {
            StorageDir = storageDir;
        }

        public void Initialize()
        {
            var di = new DirectoryInfo(this.StorageDir);
            if (!di.Exists)
                new Exception($"Error initializing DiskFileStore: Directory {this.StorageDir} does not exists.");
        }

        public Stream GetStream(string fileId)
        {
            // Check for LFI (although we should already prevent it using file permission)
            if (fileId.Contains(".."))
                throw new System.Security.SecurityException("Suspected Local File Inclusion attack. No \"..\" is allowed in fileId.");

            var fi = new FileInfo(Path.Combine(StorageDir, fileId));
            return fi.OpenRead();
        }

        public void Save(string fileId, Stream stream, bool directlyCloseStream = true)
        {
            // Read stream
            byte[] bytes = FileHelper.ReadBytes(stream);
            if (directlyCloseStream)
                stream.Close();

            if (!Directory.Exists(StorageDir))
                Directory.CreateDirectory(StorageDir);

            // Write to file system. If already exists, an IOException will be thrown.
            var s = new FileStream(Path.Combine(StorageDir, fileId).ToString(), FileMode.CreateNew, FileAccess.Write);
            s.Write(bytes, 0, bytes.Length);
            s.Close();
        }

        public void Overwrite(string fileId, Stream stream, bool directlyCloseStream = true)
        {
            // Read stream
            byte[] bytes = FileHelper.ReadBytes(stream);
            if (directlyCloseStream)
                stream.Close();

            if (!Directory.Exists(StorageDir))
                Directory.CreateDirectory(StorageDir);

            // Write to file system. If already exists, overwrites it.
            var s = new FileStream(Path.Combine(StorageDir, fileId).ToString(), FileMode.Create);
            s.Write(bytes, 0, bytes.Length);
            s.Close();
        }

        public void Delete(string fileId)
        {
            var fi = new FileInfo(Path.Combine(StorageDir, fileId));
            fi.Delete();
        }
    }
}
