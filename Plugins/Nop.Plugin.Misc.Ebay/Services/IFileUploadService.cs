using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Misc.Ebay.Services
{
    public interface IFileUploadService
    {
        /// <summary>
        /// Saves the file to the directory and returns the full path including file name.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        string Upload(IFormFile file);
        string GetDeliveryLabelUrl(string strFileName, byte[] contents);
    }
}
