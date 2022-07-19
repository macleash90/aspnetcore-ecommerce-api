using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hubtel.eCommerce.Cart.Api.Data;
using Hubtel.eCommerce.Cart.Api.Models;

using Cartt = Hubtel.eCommerce.Cart.Api.Models.Cart;
using Hubtel.eCommerce.Cart.Api.Models.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Carts
        //3. Provide an endpoint list all cart items (with filters => phoneNumbers, time, quantity, item - GET
        [HttpGet]
        public async Task<ActionResult<List<Cartt>>> GetCart( [FromQuery] string PhoneNumber, string ItemName, int? ItemId,
            int? Quantity, DateTime? FromDate, DateTime? ToDate)
        {
            //return await _context.Carts.ToListAsync();
            var query = _context.Carts.Where(x=> 1 == 1);

            if (!string.IsNullOrEmpty(PhoneNumber))
            {
                query = query.Where(x => x.User != null && x.User.PhoneNumber.Equals(PhoneNumber));
                //query = query.Include(x=>x.User).Where(x => x.User.PhoneNumber.Equals(PhoneNumber));
            }

            if (!string.IsNullOrEmpty(ItemName))
            {
                query = query.Where(x => x.Item.Name.Equals(ItemName));
            }

            if(ItemId != null )
            {
                query = query.Where(x => x.Item.Id.Equals(ItemId));
            }

            if (Quantity != null)
            {
                query = query.Where(x => x.Quantity.Equals(Quantity));
            }

            if (FromDate != null && FromDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt >= FromDate)
                    ;
            }

            if (ToDate != null && ToDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt <= ToDate)
                    ;
            }

            var carts = await query
                .Include(x => x.Item)
            //.Include(x => x.User)
            .Select(c => new
            {
                Id = c.Id,
                UserId = c.UserId,
                ItemId = c.ItemId,
                ItemName = c.ItemName,
                Quantity = c.Quantity,
                UnitPrice = c.UnitPrice,
                SubtotalPrice = c.SubtotalPrice,
                CreatedAt = c.CreatedAt,
                //User = c.User,
                Item = c.Item
            })
            
            .ToListAsync();

            return Ok(carts);
        }

        // GET: api/Carts/5
        [HttpGet("GetCartByUserId/{UserId}")]
        public async Task<ActionResult<List<Cartt>>> GetCartByUserId([FromRoute] string UserId)
        {
            var query = _context.Carts
                .Where(x => x.UserId.Equals(UserId));

            var carts =  await  query
                //.Include(x => x.User)
                .Include(x => x.Item)
                .Select(c => new Cartt
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    ItemId = c.ItemId,
                    ItemName = c.ItemName,
                    Quantity = c.Quantity,
                    UnitPrice = c.UnitPrice,
                    SubtotalPrice = c.SubtotalPrice,
                    CreatedAt = c.CreatedAt,
                    //User = c.User,
                    Item = c.Item
                })
                .ToListAsync();

            return Ok(carts);
        }

        // GET: api/Carts/5
        //4. Provide endpoint to get single item - GET
        [HttpGet("GetCartByItem/{ItemId}/{UserId}")]
        public async Task<ActionResult<Cartt>> GetCartByItem([FromRoute] int ItemId, string UserId)
        {
            return await _context.Carts
                .Where(x=> x.ItemId.Equals(ItemId))
                .Where(x => x.UserId.Equals(UserId))
                //.Include(x=>x.User)
                .Include(x => x.Item)
                .Select(c => new Cartt
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    ItemId = c.ItemId,
                    ItemName = c.ItemName,
                    Quantity = c.Quantity,
                    UnitPrice = c.UnitPrice,
                    SubtotalPrice = c.SubtotalPrice,
                    CreatedAt = c.CreatedAt,
                    //User = c.User,
                    Item = c.Item
                })
                .FirstOrDefaultAsync();

            //return Ok(cart);
        }

        // POST: api/Carts
        //1. Provide an endpoint to Add items to cart, with specified quantity
        [HttpPost]
        public async Task<ActionResult<Cartt>> PostCart(AddToCartDto cart)
        {
            var cartItem = await _context.Carts
                .Where(x=> x.ItemId.Equals(cart.ItemId))
                .Where(x=> x.UserId.Equals(cart.UserId))
                .FirstOrDefaultAsync();

            if (cartItem == null)
            {
                //check if user exists
                bool user_exists = await _context.Users.Where(x => x.Id.Equals(cart.UserId)).AnyAsync();
                if (!user_exists)
                {
                    return NotFound(new ErrorResponseDto { code = -1, message = "User does not exist" });
                }

                //check if item exists
                bool item_exists = await _context.Items.Where(x => x.Id.Equals(cart.ItemId)).AnyAsync();
                if (!item_exists)
                {
                    return NotFound(new ErrorResponseDto { code = -1, message = "Product/Item does not exist" });
                }


                // Create a new cart item if no cart item exists.                 
                cartItem = new Cartt
                {
                    ItemId = cart.ItemId,
                    Quantity = cart.Quantity,
                    UnitPrice = cart.UnitPrice,
                    SubtotalPrice = (cart.Quantity * cart.UnitPrice ),
                    ItemName = cart.ItemName,
                    UserId = cart.UserId,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now
                };

                _context.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart,                  
                // then add one to the quantity.                 
                cartItem.Quantity+= cart.Quantity;
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCart", new { id = cartItem.Id }, cartItem);
        }

        // DELETE: api/Carts/5
        // 2. Provide an endpoint to remove an item from cart - DELETE verb
        [HttpDelete("{ItemId}/{UserId}")]
        public async Task<ActionResult<Cartt>> DeleteCart( [FromRoute] int ItemId, string UserId)
        {
            var cart = await _context.Carts
                .Where(x=> (x.ItemId.Equals(ItemId) && x.UserId.Equals(UserId) ))
                .FirstOrDefaultAsync();
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return Ok(new { cart = cart });
        }

    }
}
