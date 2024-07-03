using System;

namespace eShopBackend.Models
{
    public class Commande
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryAddress { get; set; } // Nouvelle propriété
    }
}
