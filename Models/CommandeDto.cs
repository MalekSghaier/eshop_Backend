using System;

namespace eShopBackend.Models
{
    public class CommandeDto
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryAddress { get; set; } // Nouvelle propriété
    }
}
