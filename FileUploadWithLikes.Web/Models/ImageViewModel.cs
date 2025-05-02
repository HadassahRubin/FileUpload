using FileUploadWithLikes.Data;
using static System.Net.Mime.MediaTypeNames;

namespace FileUploadWithLikes.Web.Models
{
    public class ImageViewModel
    {
        public Images Image { get; set; }
        public bool HasLiked { get; set; }
    }
}
