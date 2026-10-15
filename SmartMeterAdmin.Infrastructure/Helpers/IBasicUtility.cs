

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SmartMeterAdmin.Infrastructure.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Infrastructure.Helpers
{
    public interface IBasicUtility
    {
        string Base64ToImage(string imagedata, string foldername);
        string CreateJsonFile(JObject obj);
        Task<string> GetCensusCode(string villagename);
        Task<string> UploadFile(IFormFile formFile, string FolderName, string[] allowExt);
        string GenrateCommaStringIds<T>(IEnumerable<T> ids);

    }
    public class BasicUtility : IBasicUtility
    {
        private readonly IWebHostEnvironment _env;
        private readonly SmartMeterContext _context;
        public BasicUtility(IWebHostEnvironment env, SmartMeterContext context)
        {
            _env = env;
            _context = context;
        }
        public string Base64ToImage(string imagedata, string foldername)
        {
            string extension;
            string cleandata;
            if (imagedata.Contains("data:image/png;base64"))
            {
                cleandata = imagedata.Replace("data:image/png;base64,", "");
                extension = ".png";
            }
            else if (imagedata.Contains("data:image/jpeg;base64"))
            {
                cleandata = imagedata.Replace("data:image/jpeg;base64,", "");
                extension = ".jpeg";
            }
            else if (imagedata.Contains("data:image/jpg;base64"))
            {
                cleandata = imagedata.Replace("data:image/jpg;base64,", "");
                extension = ".jpg";
            }
            else
            {

                cleandata = imagedata.Replace("data:image/jpeg;base64,", "");
                extension = ".jpeg";

            }

            var bytes = Convert.FromBase64String(cleandata);// a.base64image 
            string filedir = Path.Combine(_env.WebRootPath, foldername);
            if (!Directory.Exists(filedir))
            { //check if the folder exists;
                Directory.CreateDirectory(filedir);
            }
            //string fff = DateTime.Now.ToString();
            string fff = Guid.NewGuid().ToString();
            string getDat = fff.Replace("/", "");
            string getDat1 = getDat.Replace(":", "");
            string getDat2 = getDat1.Replace(" ", "");
            string filePath = Path.Combine(filedir, getDat2 + extension);
            string Url = filePath.Replace(filePath, "/" + foldername + "/" + getDat2 + extension);
            if (bytes.Length > 0)
            {
                using var stream = new FileStream(filePath, FileMode.Create);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
            return Url;
        }

        //using this method we can save a file in our system
        public async Task<string> UploadFile(IFormFile formFile, string FolderName, string[] allowExt)
        {
            _ = Path.GetFileNameWithoutExtension(formFile.FileName);
            string fileExt = Path.GetExtension(formFile.FileName);
            if (allowExt.Contains(fileExt.ToLower()))
            {
                //var newFilename = DateTime.Now.ToString("yyyyMMddHHmmssffff") + "." + filetype;
                string filedir = Path.Combine(_env.WebRootPath, FolderName);
                if (!Directory.Exists(filedir))
                { //check if the folder exists;
                    Directory.CreateDirectory(filedir);
                }
                var filePath = Path.Combine(filedir) + $@"\{formFile.FileName}";
                string Url = filePath.Replace(filePath, "/" + FolderName + "/" + formFile.FileName);
                if (formFile.Length > 0)
                {
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await formFile.CopyToAsync(stream);
                }
                return Url;
            }
            return string.Empty;
        }

        public string CreateJsonFile(JObject obj)
        {
            string filedir = Path.Combine(_env.WebRootPath, "CustomerDataJson");
            if (!Directory.Exists(filedir))
            { //check if the folder exists;
                Directory.CreateDirectory(filedir);
            }
            string fff = Guid.NewGuid().ToString() + ".json";
            string filePath = Path.Combine(filedir, fff);
            System.IO.File.WriteAllText(filePath, obj.ToString());
            return filePath;
        }

        public async Task<string> GetCensusCode(string villagename)
        {
            return await _context.VillageDatas.Where(a => a.Name == villagename).Select(a => a.CensusCode).FirstOrDefaultAsync();
        }

        public string GenrateCommaStringIds<T>(IEnumerable<T> ids)
        {
            return string.Join(",", ids);
        }
    }
}
