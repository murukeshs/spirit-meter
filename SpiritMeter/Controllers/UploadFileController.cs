using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpiritMeter.Data;
using SpiritMeter.Models;

namespace SpiritMeter.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        #region UploadFile
        [HttpPost, Route("UploadFile")]
        [AllowAnonymous]
        public async Task<string> UploadFile()
        {
            try
            {
                IFormFile myFile = Request.Form.Files.First();
                string myFileName = null;
                byte[] myFileContent = null;
                if (myFile != null)
                {
                    myFileName = myFile.FileName;


                    using (var memoryStream = new MemoryStream())
                    {
                        await myFile.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        myFileContent = new byte[memoryStream.Length];
                        await memoryStream.ReadAsync(myFileContent, 0, myFileContent.Length);
                    }
                }
                Global.fileurl = Common.CreateMediaItem(myFileContent, myFileName);
                return Global.fileurl;
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }

        #endregion
    }
}