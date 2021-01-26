using Microsoft.AspNetCore.Http;

namespace builk_uploads_api.FileData.Domain
{
    public class UploadRequest
    {
        public string alias { set; get; }
        public IFormFile file { set; get; }
    }
}
