using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Models.Dto
{
    public class AddToCartDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public string ItemName { get; set; }
        [Required]
        [Range(1, (int)int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        //[IntegerValidator(MinValue = 1, MaxValue = int.MaxValue,
    //ExcludeRange = true)]

        public int Quantity { get; set; }
        [Required]
        public double UnitPrice { get; set; }
    }
}
