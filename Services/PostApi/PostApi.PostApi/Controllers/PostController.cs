using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostApi.INFRASTRUCTURE.Extensions;
using PostApi.INFRASTRUCTURE.Requests;
using PostApi.DTO;
using PostApi.ORM.Data;
using System.Diagnostics;

namespace PostApi.Controllers
{
    [Route("api/[Controller]")]
    public class PostController: Controller
	{
        // dbcontext string
        private readonly MicroServiceDBContext _dbContext;

        // creating lock object
        static object _lockobject = new object();

        static string filePath="";
        static string fileThumbnail= "";

        public PostController(MicroServiceDBContext dbContext)
        {
            lock (_lockobject)
            {
                _dbContext = dbContext;
            }
        }

        //Get By Id Listing method

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _dbContext.Posts.Where(c => c.Id == id).FirstOrDefaultAsync();

            if (post != null)
            {
                return Ok(post.MapToPostDTO());
            }

            return NotFound();
        }

        //GetAll Listing method

        [HttpGet("Post")]
        public async Task<IActionResult> GetAll()
        {
            var post = await _dbContext.Posts.OrderBy(c => c.CreatedDate).ToListAsync();

            return Ok(post.Select(c => c.MapToPostDTO()));
        }

        //Post / Add  method


        [HttpPost("Post")]
        public async Task<IActionResult> Post(CreatePostRequest request)
        {
            var post = new Post
            {
                Source = filePath,
                //Source = request.Source,
                Thumbnail = fileThumbnail,
                //Thumbnail = request.Thumbnail,
                UserId = request.UserId,
                CategoryId = request.CategoryId,
                RatingCount = request.RatingCount,
                CreatedDate = DateTime.Now.Date
            };

            _dbContext.Posts.Add(post);

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post.MapToPostDTO());
        }

        //Update method

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, PostDTO model)
        {
            var post = await _dbContext.Posts.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (post != null)
            {

                post.Source = model.Source;
                post.Thumbnail = model.Thumbnail;
                post.UserId = model.UserId;
                post.CategoryId = model.CategoryId;
                post.RatingCount = model.RatingCount;
                post.CreatedDate = model.CreatedDate;

                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = post.Id }, post.MapToPostDTO());
            }

            return NotFound();
        }

        //Delete Method

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != 0)
            {

                var post = await _dbContext.Posts.FirstOrDefaultAsync(x => x.Id == id);

                if (post!=null)
                {
                    _dbContext.Posts.Remove(post);


                    await _dbContext.SaveChangesAsync();

                    return Ok();
                }
               
            }

            return NotFound();
        }

        //Upload method

        [HttpPost("upload", Name = "upload")]
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
            return (extension == ".mp4" || extension == ".mov"); // Change the extension based on your need
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
                    Directory.CreateDirectory(pathBuilt+ @"/Videos");
                }

                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files/Videos",
                   fileName);

                
              using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                isSaveSuccess = true;

                //---------------< Post_create_Thumbnail() >---------------
                string sPath_FFMpegDir = @"/Users/ozzyozmencelik/development/";

                string thumbnailName = fileName.Substring(0, fileName.Length - 4);

                 fileThumbnail = pathBuilt + @"/Thumbnails/" + thumbnailName + ".jpg";

                var startInfo = new ProcessStartInfo
                {
                   
                    FileName = sPath_FFMpegDir + $"ffmpeg",
                    Arguments = $"-i " + filePath + " -an -vf scale=200x112 " + fileThumbnail + " -ss 00:00:00 -vframes 1",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = sPath_FFMpegDir
                };

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    process.WaitForExit();
                }
                //---------------</ Post_create_Thumbnail() >---------------

            }
            catch (Exception e)
            {
                e.Message.ToString();
            }

            return isSaveSuccess;
        }

    }
}


