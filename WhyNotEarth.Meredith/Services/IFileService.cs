﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.Services
{
    public interface IFileService
    {
        Task SaveAsync(string path, string contentType, Stream stream);

        Task<string> SaveAsync(string companySlug, List<string> path, string contentType, Stream stream);

        Task GetAsync(string path, Stream stream);

        string GetPrivateUrl(string path);
    }
}