using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signinMgr)
        {
            _userManager = userManager;
            _signInManager = signinMgr;

        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new AppUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FullName = model.FullName,
                        PhoneNumber = model.PhoneNumber,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                    };

                    AppUser checkEmail = await _userManager.FindByEmailAsync(user.Email);

                    if (checkEmail != null)
                    {
                        return BadRequest(new ErrorResponseDto { code = -1, message= "Email already registered" });
                    }

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        return CreatedAtAction("Created",new { message = "User Account Successfully Registered" });
                    }

                    return BadRequest(new { message = "Could not create account" });

                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = "Something went wrong" });
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto user)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    AppUser appUser = await _userManager.FindByEmailAsync(user.Email);

                    if (appUser != null)
                    {
                            var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, false);

                            if (result.Succeeded)
                            {

                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.JWT_Key));
                            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                            var token = new JwtSecurityToken(
                                issuer: Startup.JWT_Issuer,
                                audience: Startup.JWT_Audience ,
                                expires: DateTime.Now.AddHours(24),
                                signingCredentials: creds);

                            var auth = new JwtSecurityTokenHandler().WriteToken(token);
                            await _userManager.SetAuthenticationTokenAsync(appUser, "Server", appUser.UserName, auth);
                            var tok = await _userManager.CreateSecurityTokenAsync(appUser);

                            //await _signInManager.CanSignInAsync(user);

                            return Ok(new { Access_Token = auth, Expires_In_Hours = 12, Date = DateTime.Now, UserId = appUser.Id });

                        }
                            else
                            {
                                return BadRequest(new { message = "Incorrect username/password combination" });
                            }
                        
                    }
                    else
                    {

                        return BadRequest(new { message = "Incorrect username/password combination" });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Something went wrong" });
                }

            }

            else
            {
                return BadRequest(ModelState);
            }

        }
    }
}
