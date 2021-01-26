using Microsoft.EntityFrameworkCore;


namespace builk_uploads_api.DataContext.Context
{
    public class DocumentContext:DbContext
    {

        private readonly string _connectionString;

        public DocumentContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }


    }

}
