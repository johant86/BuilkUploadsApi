using builk_uploads_api.FileData.Domain;
using builk_uploads_api.FileData.Repositories;
using builk_uploads_api.Middlewares;
using builk_uploads_api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;


namespace builk_uploads_api.FileData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class UploadController : ControllerBase
    {
        public DataRepository _UploadData { get; private set; }

        public UploadController(DataRepository uploadData)
        {
            this._UploadData = uploadData;
        }

        [HttpPost]
       // [Authorize]
        public IActionResult UploadData([FromForm] UploadRequest dataConfig)
        {
            try
            {
                if (dataConfig != null || dataConfig.file != null || dataConfig.alias != null)
                {
                    var result = this._UploadData.SaveData(dataConfig);
                    return Ok(result);
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
                new LogErrors().WriteLog("UploadDataController", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message, $"Request=> {JsonConvert.SerializeObject(dataConfig)}");
                return StatusCode(500);
            }
        }
    }
}
