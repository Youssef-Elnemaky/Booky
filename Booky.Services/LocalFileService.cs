using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Booky.Services
{
    public class LocalFileService : IFileService
    {
        private readonly string rootPath;

        public LocalFileService(string rootPath)
        {
            this.rootPath = rootPath;
        }

        public string Upload(IFormFile file, string folder)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var directoryPath = Path.Combine(rootPath, folder);
            var fullPath = Path.Combine(directoryPath, fileName);

            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return $"{folder}/{fileName}";
        }

        public void Delete(string path)
        {
            if(string.IsNullOrWhiteSpace(path)) return;

            var fullPath = Path.Combine(rootPath, path.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
        }

        public string Replace(string oldPath, IFormFile file, string folder)
        {
            Delete(oldPath);
            return Upload(file, folder);
        }
    }
}
