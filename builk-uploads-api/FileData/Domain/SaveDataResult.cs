using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace builk_uploads_api.FileData.Domain
{
    public class SaveDataResult
    {
        public bool success { set; get; }
        public string message { set; get; }
        public List<ErrorDetails> errorDetails { set; get; }
    }

    public class ErrorDetails
    {
        public int errorCode { set; get; }
        public string error { set; get; }
        public string severity { set; get; }
        public int columnNumber { set; get; }
        public string errorDate { set; get; }
    }
}
