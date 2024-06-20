using eShopBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace eShopBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly string connectionString;
        private readonly ILogger<ProductsController> logger;

        public ProductsController(IConfiguration configuration, ILogger<ProductsController> logger)
        {
            connectionString = configuration.GetConnectionString("SqlServerDb");
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult GetProducts(int pageNumber = 0, int pageSize = 3)
        {
            try
            {
                List<Product> products = new List<Product>();
                int offset = pageNumber * pageSize;

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Produit ORDER BY id OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@offset", offset);
                        command.Parameters.AddWithValue("@pageSize", pageSize);
                        command.CommandTimeout = 300;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var product = new Product
                                {
                                    id = (int)reader["id"],
                                    name = reader["name"].ToString(),
                                    description = reader["description"].ToString(),
                                    price = (decimal)reader["price"],
                                    stock = (int)reader["stock"],
                                    imageUrl = reader["imageUrl"].ToString()
                                };
                                products.Add(product);
                            }
                        }
                    }
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception while fetching products: {ex.Message}");
                ModelState.AddModelError("Product", $"Sorry, but we have an exception: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Produit WHERE id = @id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var product = new Product
                                {
                                    id = (int)reader["id"],
                                    name = reader["name"].ToString(),
                                    description = reader["description"].ToString(),
                                    price = (decimal)reader["price"],
                                    stock = (int)reader["stock"],
                                    imageUrl = reader["imageUrl"].ToString()
                                };
                                return Ok(product);
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception while fetching product by ID: {ex.Message}");
                ModelState.AddModelError("Product", $"Sorry, but we have an exception: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductDto productDto)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Produit (name, description, price, stock, imageUrl) VALUES (@name, @description, @price, @stock, @imageUrl)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", productDto.name);
                        command.Parameters.AddWithValue("@description", productDto.description);
                        command.Parameters.AddWithValue("@price", productDto.price);
                        command.Parameters.AddWithValue("@stock", productDto.stock);
                        command.Parameters.AddWithValue("@imageUrl", productDto.imageUrl);

                        command.ExecuteNonQuery();
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception while creating product: {ex.Message}");
                ModelState.AddModelError("Product", $"Sorry, but we have an exception: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, ProductDto productDto)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Produit SET name = @name, description = @description, price = @price, stock = @stock, imageUrl = @imageUrl WHERE id = @id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", productDto.name);
                        command.Parameters.AddWithValue("@description", productDto.description);
                        command.Parameters.AddWithValue("@price", productDto.price);
                        command.Parameters.AddWithValue("@stock", productDto.stock);
                        command.Parameters.AddWithValue("@imageUrl", productDto.imageUrl);
                        command.Parameters.AddWithValue("@id", id);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            return NotFound();
                        }
                    }
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception while updating product: {ex.Message}");
                ModelState.AddModelError("Product", $"Sorry, but we have an exception: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM Produit WHERE id = @id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            return NotFound();
                        }
                    }
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception while deleting product: {ex.Message}");
                ModelState.AddModelError("Product", $"Sorry, but we have an exception: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return Ok("Connection successful");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Connection failed: {ex.Message}");
                return BadRequest($"Connection failed: {ex.Message}");
            }
        }

        [HttpPost("generate-products")]
        public IActionResult GenerateProducts()
        {
            try
            {
                var random = new Random();
                var products = new List<Product>();

                for (int i = 0; i < 2000000; i++)
                {
                    var product = new Product
                    {
                        name = "Produit " + random.Next(1, 1000000),
                        description = "Description " + random.Next(1, 1000000),
                        price = (decimal)(random.Next(1, 10000) + random.NextDouble()),
                        stock = random.Next(1, 1000),
                        imageUrl = "https://picsum.photos/200?random=" + random.Next(1, 1000000)
                    };
                    products.Add(product);
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var product in products)
                    {
                        string sql = "INSERT INTO Produit (name, description, price, stock, imageUrl) VALUES (@name, @description, @price, @stock, @imageUrl)";

                        using (var command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@name", product.name);
                            command.Parameters.AddWithValue("@description", product.description);
                            command.Parameters.AddWithValue("@price", product.price);
                            command.Parameters.AddWithValue("@stock", product.stock);
                            command.Parameters.AddWithValue("@imageUrl", product.imageUrl);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("2 millions de produits ont été générés et insérés avec succès.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception lors de la génération des produits : {ex.Message}");
                return BadRequest($"Exception lors de la génération des produits : {ex.Message}");
            }
        }
    }
}