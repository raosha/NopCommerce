using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Media;

namespace Nop.Plugin.Misc.Ebay.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment env;
        private readonly IWebHelper _webHelper;
        private readonly INopFileProvider _fileProvider;
        public FileUploadService(IWebHostEnvironment env, IWebHelper webHelper, INopFileProvider fileProvider)
        {
            this.env = env;
            _webHelper = webHelper;
            _fileProvider = fileProvider;
        }
        public string Upload(IFormFile file)
        {
            if (!Directory.Exists(NopMediaDefaults.DeliveryLabelsPath))
                Directory.CreateDirectory(NopMediaDefaults.DeliveryLabelsPath);

            var uploadPath = _fileProvider.GetAbsolutePath(NopMediaDefaults.DeliveryLabelsPath);
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var strem = File.Create(filePath))
                file.CopyTo(strem);

            return filePath;
        }

        public string GetDeliveryLabelUrl(string strFileName, byte[] contents)
        {
            if (!Directory.Exists(NopMediaDefaults.DeliveryLabelsPath))
                Directory.CreateDirectory(NopMediaDefaults.DeliveryLabelsPath);

            var uploadPath = _fileProvider.GetAbsolutePath(NopMediaDefaults.DeliveryLabelsPath);
            var fileName = Path.GetFileName(strFileName);
            var filePath = Path.Combine(uploadPath, fileName);

            if (!File.Exists(filePath))
                _fileProvider.WriteAllBytes(filePath, contents);

            var imagesPathUrl = _webHelper.GetStoreLocation();
            //imagesPathUrl += NopMediaDefaults.DeliveryLabelsPath;

            Uri baseUri = new Uri(imagesPathUrl);
            var fullUrlToImage = new Uri(baseUri, $"/images/deliverylabels/{fileName}");

            return fullUrlToImage.AbsoluteUri;
        }
    }
}
