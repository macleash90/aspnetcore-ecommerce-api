using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Models.Dto
{
    public class ErrorResponseDto
    {
        public ErrorResponseDto()
        {
            this.code = -1;
        }
        public string message;
        public int code;
    }
}
