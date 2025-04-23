using FileUpload.Data;
using FileUpload.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace FileUpload.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=MyFirstDatabase;Integrated Security=True;Trust Server Certificate=true;";

        private IWebHostEnvironment _webHostEnvironment;
        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(IFormFile image, string password)
        {
            var fileName = $"{Guid.NewGuid()}-{image.FileName}";
            var fullFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", fileName);
            using FileStream fs = new FileStream(fullFilePath, FileMode.Create);
            image.CopyTo(fs);
            var repo = new ImageManager(_connectionString);
            var savedImage = repo.Add(new Data.Image
            {
                ImagePath = fileName,
                Password = password
            });

            var vm = new ImageViewModel
            {
                Image = savedImage
            };

            return View(vm);
        }
        public IActionResult ViewImage(int id)
        {
            var repo = new ImageManager(_connectionString);
            var image = repo.GetById(id); 

            var key = $"ImageAuth-{id}";
            bool authorized = HttpContext.Session.GetString(key) == "true";

            var vm = new ViewImageViewModel
            {
                Image = image,
                IsAuthorized = authorized,
                ViewCount = authorized ? repo.IncrementViewCount(id) : 0
            };

            return View(vm);
        }
        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {
            var repo = new ImageManager(_connectionString);
            var image = repo.GetById(id);

            if (image == null)
            {
                return RedirectToAction("Index"); 
            }

            if (image.Password == password)
            {
                HttpContext.Session.SetString($"ImageAuth-{id}", "true");
                return RedirectToAction("ViewImage", new { id });
            }

         
            var vm = new ViewImageViewModel
            {
                Image = image,
                IsAuthorized = false,
                ErrorMessage = "Please try again"
            };

            return View(vm);
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
}
