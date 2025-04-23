using FileUpload.Data;
using static System.Net.Mime.MediaTypeNames;
using Image = FileUpload.Data.Image;

namespace FileUpload.Web.Models
{
    public class ImageViewModel
    {
        public Image Image { get; set; }
    }
}
