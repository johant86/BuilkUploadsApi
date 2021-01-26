using builk_uploads_api.DataContext.Context;
using builk_uploads_api.DataContext.Entites;
using Microsoft.EntityFrameworkCore;

namespace builk_uploads_api.FileData.Repositories
{
    public class SQLRepository
    {
        public void processData(string[,] data, SourceConfiguration configuration)
        {
            string s = "Server=(LocalDB)\\LocalDB;Database=Test_DB;User Id=sa;Password=123456;";
            var optionsBuilder = new DbContextOptionsBuilder<DataUploadContext>();
            optionsBuilder.UseSqlServer(s);
            var _dbSource = new DataUploadContext(optionsBuilder.Options);
        }
    }
}
