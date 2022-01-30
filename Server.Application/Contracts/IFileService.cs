using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Server.Application.Contracts
{
    public interface IFileService
    {
        Task<string> SaveImage(IFormFile imageFile, string namePreset);
    }
}