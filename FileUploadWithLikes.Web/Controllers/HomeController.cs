using FileUploadWithLikes.Data;
using FileUploadWithLikes.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace FileUploadWithLikes.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString;
        private IWebHostEnvironment _webHostEnvironment;

        public HomeController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var repo = new ImageRepository(_connectionString);
            var vm = new IndexViewModel
            {
                Images = repo.GetAll()
            };
            return View(vm);
        }
        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(IFormFile image, string title)
        {
            var fileName = $"{Guid.NewGuid()}-{image.FileName}";
            var fullFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", fileName);
            using FileStream fs = new FileStream(fullFilePath, FileMode.Create);
            image.CopyTo(fs);
            var repo = new ImageRepository(_connectionString);
            var imageToAdd = new Images
            {
                Title = title,
                FileName = fileName,
                DateTime = DateTime.Now,
                Likes = 0
            };

            repo.Add(imageToAdd);

            return RedirectToAction("Index");
        }
        public IActionResult ViewImage(int id)
        {
            var repo = new ImageRepository(_connectionString);
            var likedImages = HttpContext.Session.Get<List<int>>("likedImages") ?? new List<int>();
            bool hasLiked = likedImages.Contains(id);

            var vm = new ImageViewModel
            {
                Image = repo.GetById(id),
                HasLiked = hasLiked
            };
            return View(vm);
        }
        public IActionResult Like(int id)
        {
            var repo = new ImageRepository(_connectionString);
            var liked = HttpContext.Session.Get<List<int>>("likedImages") ?? new List<int>();
            if (!liked.Contains(id))
            {
                repo.IncrementLikes(id);
                liked.Add(id);
                HttpContext.Session.Set("likedImages", liked);
            }
            return RedirectToAction("Index");
        }
        public IActionResult GetLikes(int id)
        {
            var repo = new ImageRepository(_connectionString);
            int likes = repo.GetLikes(id);
            return Json(likes);
        }
    }

      
    }

public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T Get<T>(this ISession session, string key)
    {
        string value = session.GetString(key);

        return value == null ? default(T) :
            JsonSerializer.Deserialize<T>(value);
    }
}
