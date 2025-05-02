using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadWithLikes.Data
{
    public class ImageDataContextFactory: IDesignTimeDbContextFactory<ImageDataContext>
    {
        public ImageDataContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
              .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(),
              $"..{Path.DirectorySeparatorChar}FileUploadWithLikes.Web"))
              .AddJsonFile("appsettings.json")
              .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true).Build();

            return new ImageDataContext(config.GetConnectionString("ConStr"));
        }
    }
}
