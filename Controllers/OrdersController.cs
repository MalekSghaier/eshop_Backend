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
    public class CommandesController : ControllerBase
    {
        private readonly string connectionString;
        private readonly ILogger<CommandesController> logger;

        public CommandesController(IConfiguration configuration, ILogger<CommandesController> logger)
        {
            connectionString = configuration.GetConnectionString("SqlServerDb");
            this.logger = logger;
        }

        [HttpPost]
        public IActionResult CreateCommande(CommandeDto commandeDto)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO [Commandes] (OrderDate, TotalPrice, DeliveryAddress) OUTPUT INSERTED.ID VALUES (@OrderDate, @TotalPrice, @DeliveryAddress)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@OrderDate", commandeDto.OrderDate);
                        command.Parameters.AddWithValue("@TotalPrice", commandeDto.TotalPrice);
                        command.Parameters.AddWithValue("@DeliveryAddress", commandeDto.DeliveryAddress);

                        int commandeId = (int)command.ExecuteScalar();
                        return Ok(new { Id = commandeId });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception while creating commande: {ex.Message}");
                ModelState.AddModelError("Commande", $"Sorry, but we have an exception: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetCommandeById(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM [Commandes] WHERE Id = @Id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var commande = new Commande
                                {
                                    Id = (int)reader["Id"],
                                    OrderDate = (DateTime)reader["OrderDate"],
                                    TotalPrice = (decimal)reader["TotalPrice"],
                                    DeliveryAddress = reader["DeliveryAddress"].ToString()
                                };
                                return Ok(commande);
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
                logger.LogError($"Exception while fetching commande by ID: {ex.Message}");
                ModelState.AddModelError("Commande", $"Sorry, but we have an exception: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        public IActionResult GetCommandes()
        {
            try
            {
                List<Commande> commandes = new List<Commande>();
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM [Commandes] ORDER BY Id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var commande = new Commande
                                {
                                    Id = (int)reader["Id"],
                                    OrderDate = (DateTime)reader["OrderDate"],
                                    TotalPrice = (decimal)reader["TotalPrice"],
                                    DeliveryAddress = reader["DeliveryAddress"].ToString()
                                };
                                commandes.Add(commande);
                            }
                        }
                    }
                }
                return Ok(commandes);
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception while fetching commandes: {ex.Message}");
                ModelState.AddModelError("Commande", $"Sorry, but we have an exception: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCommande(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM [Commandes] WHERE Id = @Id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
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
                logger.LogError($"Exception while deleting commande: {ex.Message}");
                ModelState.AddModelError("Commande", $"Sorry, but we have an exception: {ex.Message}");
                return BadRequest(ModelState);
            }
        }
    }
}
