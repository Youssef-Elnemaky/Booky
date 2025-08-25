using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booky.Services
{
    public interface IFileService
    {
        string Upload(IFormFile file, string folder);
        void Delete(string path);
        string Replace(string oldPath,IFormFile file, string folder);
    }
}
