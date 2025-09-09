using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BDA.FileStorage
{
    /// <summary>
    /// File storage mechanism.
    /// </summary>
    public interface IFileStore
    {
        void Initialize();
        Stream GetStream(string fileId);
        void Save(string fileId, Stream stream, bool directlyCloseStream = true);
        void Overwrite(string fileId, Stream stream, bool directlyCloseStream = true);
        void Delete(string fileId);
    }
}
