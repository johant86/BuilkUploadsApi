using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace builk_uploads_api.Settings
{
    public class AppSettings
    {
        public SharepointSettings sharepointSettings { set; get; }
        public string[] AllowedFileFormats { set; get; }
    }

    public class SharepointSettings
    {
        public string NetworkLogin { set; get; }
        public string Password { set; get; }
        public string Domain { set; get; }
    }
}
