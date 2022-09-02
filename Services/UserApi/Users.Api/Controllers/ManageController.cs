using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Users.Api.Data.Entities;
using Users.Api.Infrastructure.Extensions;
using Users.Api.Infrastructure.Helpers;
using Users.Api.Models;

namespace Users.Api.Controllers
{
    public class ManageController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly Mapper _mapper;

        static string filePath = "";

        public ManageController(
            UserManager<User> userManager,
            Mapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        [Route("Manage/ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordDto request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Your password has been changed.");
            }

            return BadRequest(result.GetError());
        }

        [HttpPost]
        [Authorize]
        [Route("Manage")]
        public async Task<IActionResult> Post([FromBody]PostDto request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            string uphoto;

            if (filePath != null)
            {
                uphoto = filePath;
            }
            else
            {
                uphoto = request.Photo;
            }

            user.Name = request.Name;
            user.Surname = request.Surname;
            user.Photo = uphoto;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                Ok("Your profile has been updated");
            }

            return BadRequest("Your profile has been updated");
        }

        [HttpGet]
        [Authorize]
        [Route("Manage")]
        public async Task<IActionResult> Get()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Ok(_mapper.MapToUserDto(user));
        }

        //Upload method

        [HttpPost("Manage/ImageUpload", Name = "upload")]
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

        /// Method to check if it is the right file
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