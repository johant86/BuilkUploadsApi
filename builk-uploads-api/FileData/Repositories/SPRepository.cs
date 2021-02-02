using builk_uploads_api.DataContext;
using builk_uploads_api.Settings;
using builk_uploads_api.Shared.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace builk_uploads_api.FileData.Repositories
{
    public class SPRepository: SPBaseRepository
    {
        private readonly ClientContext _Context;
        private readonly DataConfigContext _DbContext;
        public SPRepository(IOptions<AppSettings> sharePointSettings, DataConfigContext dbContext) : base(sharePointSettings)
        {
            this._DbContext = dbContext;
            //this._Context = SPBaseRepository.GetSPContext(this._sharePointSettings.JobOpeningsUrl, this._sharePointSettings.networkLogin, this._sharePointSettings.password, this._sharePointSettings.domain);

        }

        public void test()
        {

        }
    }
}
