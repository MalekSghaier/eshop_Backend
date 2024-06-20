using System.ComponentModel.DataAnnotations;

namespace eShopBackend.Models
{
    // read the data from the database
    public class Product
    {
        public int id { get; set; }

        public string name { get; set; } = "";

        public string description { get; set; } = "";

        public decimal price { get; set; }

        public int stock { get; set; }

        public string imageUrl { get; set; } = ""; // Nouvelle propriété pour l'URL de l'image
    }
}
