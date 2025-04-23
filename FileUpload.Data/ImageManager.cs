using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FileUpload.Data
{
    public class ImageManager
    {
        private readonly string _connectionString;

        public ImageManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Image Add(Image image)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Images (Password, ImagePath) VALUES (" +
                "@password, @path); SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@password", image.Password);
            cmd.Parameters.AddWithValue("@path", image.ImagePath);
            connection.Open();
            int imageId = (int)(decimal)cmd.ExecuteScalar();
            cmd.CommandText = "SELECT * FROM Images WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", imageId);
            var reader = cmd.ExecuteReader();
            reader.Read();
            Image newImage = new()
            {
                Id = (int)reader["Id"],
                Password = (string)reader["password"],
                ImagePath = (string)reader["imagePath"],
                ViewCount = (int)reader["ViewCount"]

            };
            return newImage;
        }

        public List<Image> GetAll()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Images";
            connection.Open();
            var reader = cmd.ExecuteReader();
            List<Image> images = new();
            while (reader.Read())
            {
                images.Add(new Image
                {
                    Id = (int)reader["Id"],
                    Title = (string)reader["Title"],
                    ImagePath = (string)reader["ImagePath"]
                });
            }

            return images;
        }
        public Image GetById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Images WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new Image
            {
                Id = (int)reader["Id"],
                Password = (string)reader["Password"],
                ImagePath = (string)reader["ImagePath"]
            };
        }
        public int IncrementViewCount(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Images SET ViewCount = ISNULL(ViewCount, 0) + 1 WHERE Id = @id;
                        SELECT ViewCount FROM Images WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            return (int)cmd.ExecuteScalar();
        }

    }
}
