using builk_uploads_api.FileData.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace builk_uploads_api.DataContext.Entites
{
    public class DataUploaded
    {
        [Key]
        public int id { set; get; }
    }

    public class UploadResult
    {
        public int RowsInserted { set; get; }
        public List<ErrorDetails> errorDetails { set; get; }
    }
}
