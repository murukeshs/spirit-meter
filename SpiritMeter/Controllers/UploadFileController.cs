using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpiritMeter.Data;
using SpiritMeter.Models;
using static SpiritMeter.Data.Common;

namespace SpiritMeter.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        #region uploadFile
        /// <summary>
        /// To uploadFile image for mobile
        /// </summary>
        [HttpPost, Route("uploadFile")]
        [AllowAnonymous]
        public async Task<IActionResult> uploadFile(IFormFile formFile)
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
                string file = Common.CreateMediaItem(myFileContent, myFileName);
                Global global = new Global();
                global.fileurl = file;
                return StatusCode((int)HttpStatusCode.OK, global);
            }

            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("uploadFile", e.Message.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message.ToString() });
            }
        }

        #endregion

        #region UploadFileBase64
        /// <summary>
        /// To uploadFile image for web
        /// </summary>
        [HttpPost, Route("UploadFileBase64")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadFileBase64([FromBody] UploadModel uploadModel)
        {
            try
            {
                var imageDataByteArray = Convert.FromBase64String(uploadModel.file);

                string myFileName = uploadModel.fileName;
                byte[] myFileContent = imageDataByteArray;

                string file = Common.CreateMediaItem(myFileContent, myFileName);
                Global global = new Global();
                global.fileurl = file;
                return StatusCode((int)HttpStatusCode.OK, global);
            }
            catch (Exception e)
            {
                string SaveErrorLog = Data.Common.SaveErrorLog("UploadFileBase64", e.Message.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ErrorMessage = e.Message.ToString() });
            }
        }
        #endregion
    }
}