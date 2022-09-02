using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Users.Api.Data.Entities;
using Users.Api.Infrastructure.Extensions;
using Users.Api.Infrastructure.Helpers;
using Users.Api.Models;
using Users.Api.Services;

namespace Users.Api.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        static string filePath = "";

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Account/Login")]
        public async Task<IActionResult> Login([FromBody]LoginDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (result.Succeeded)
                {
                    var date = DateTime.UtcNow;
                    var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, date.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
                        };
                    var securityKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration["JwtConfiguration:SecurityKey"]));
                    var securityToken = new JwtSecurityToken(
                        issuer: _configuration["JwtConfiguration:Issuer"],
                        audience: _configuration["JwtConfiguration:Audience"],
                        claims: claims,
                        notBefore: date,
                        expires: date.AddMinutes(60),
                        signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                    );

                    var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
                    return Ok(token);
                }
            }

            return BadRequest("Please try another email address or password.");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Account/ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest("Please try another email address.");
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Code, request.Password);
            if (result.Succeeded)
            {
                return Ok("Your password has been reset.");
            }

            return BadRequest(result.GetError());
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Account/ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest("Please try another email address.");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = UrlBuilder.ResetPasswordCallbackLink(code);
            await _emailSender.SendEmailAsync(request.Email, "Reset Password",
                $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

            return Ok("Please check your email to reset your password.");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Account/ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]int userId, [FromQuery]string code)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for email confirm.");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Ok("Thank you for confirmed your email.");
            }
            
            return BadRequest(result.GetError());
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Account/Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            string uphoto;

            if (filePath !=null)
            {
                uphoto = filePath;
            }
            else
            {
                uphoto = request.Photo;
            }
            var user = new User
            {
                Name = request.Name,
                Surname = request.Surname,
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber=request.PhoneNumber,
                Photo= uphoto
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = UrlBuilder.EmailConfirmationLink(user.Id,HttpUtility.UrlEncode(code));
                await _emailSender.SendEmailAsync(request.Email, "Confirm your email",
                    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");

                return Created($"User/{user.Id}", null);
            }

            return BadRequest(result.GetError());
        }

        //Upload method

        [HttpPost("Account/ImageUpload", Name = "Account/ImageUpload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadFile(
         IFormFile file,
         CancellationToken cancellationToken)
        {
            if (CheckFile(file))
            {
                await WriteFile(file);
            }
            else
            {
                return BadRequest(new { message = "Invalid file extension" });
            }

            return Ok();
        }

        /// 

        /// Method to check if it is the right filehttp
        /// 

        /// 
        /// 
        private bool CheckFile(IFormFile file)
        {
            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            return (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".PNG"); // Change the extension based on your need
        }

        private async Task<bool> WriteFile(IFormFile file)
        {
            string fileName;
            bool isSaveSuccess = false;
            try
            {

                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Files");

                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt + @"/Photos");
                }

                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files/Photos",
                   fileName);


                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                isSaveSuccess = true;

               

            }
            catch (Exception e)
            {
                e.Message.ToString();
            }

            return isSaveSuccess;
        }
    }
}