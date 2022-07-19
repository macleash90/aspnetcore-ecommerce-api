using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class Cart
    {
        public Cart()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }

        public int Quantity { get; set; }

        public double UnitPrice { get; set; }
        public double SubtotalPrice { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [NotMapped]
        public virtual AppUser User { get; set; }
        public virtual Item Item { get; set; }
        
    }
}
