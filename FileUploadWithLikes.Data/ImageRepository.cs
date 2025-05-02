using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadWithLikes.Data
{
    public class ImageRepository
    {
        private readonly string _connectionString;

        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Images> GetAll()
        {
            using var ctx = new ImageDataContext(_connectionString);
            return ctx.Images .OrderByDescending(i => i.DateTime).ToList();
        }

        public Images GetById(int id)
        {
            using var ctx = new ImageDataContext(_connectionString);
            return ctx.Images.FirstOrDefault(p => p.Id == id);
        }

        public void Add(Images image)
        {
            using var ctx = new ImageDataContext(_connectionString);
            ctx.Images.Add(image);
            ctx.SaveChanges();
        }

       public void IncrementLikes(int id)
        {
            using var ctx = new ImageDataContext(_connectionString);
            var image = ctx.Images.FirstOrDefault(i => i.Id == id);
            if (image != null)
            {
                image.Likes++;
                ctx.SaveChanges();
            }
        }
        public int GetLikes(int id)
        {
            using var ctx = new ImageDataContext(_connectionString);
            var image = ctx.Images.FirstOrDefault(i => i.Id == id);
            return image?.Likes ?? 0;
        }

    }
}
