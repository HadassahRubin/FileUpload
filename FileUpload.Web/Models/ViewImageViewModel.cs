namespace FileUpload.Web.Models
{
    public class ViewImageViewModel
    {
        public Data.Image Image { get; set; }
        public bool IsAuthorized { get; set; }
        public string ErrorMessage { get; set; }
        public int ViewCount { get; set; }
    }
}
