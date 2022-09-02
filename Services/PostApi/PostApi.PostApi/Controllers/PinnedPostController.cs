using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostApi.DTO;
using PostApi.ORM.Data;
using Microsoft.JSInterop.Implementation;
using PostApi.INFRASTRUCTURE.Extensions;
using PostApi.INFRASTRUCTURE.Requests;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PostApi.PostApi.Controllers
{
    [Route("api/[Controller]")]
    public class PinnedPostController : Controller
    {
        private readonly MicroServiceDBContext _dbContext; // our db context
         // creating lock object
        static object _lockobject = new object();

        //set dbcontext once in Constructor

        public PinnedPostController(MicroServiceDBContext dbContext)
        {
            lock (_lockobject)
            {
                _dbContext = dbContext;
            }

        }

        // Listing values by Id method
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _dbContext.PinnedPosts.Where(c => c.Id == id).FirstOrDefaultAsync();

            if (post != null)
            {
                return Ok(post.MapToPinnedPostDTO());
            }

            return NotFound();
        }

        //  Listing all values method

        [HttpGet("PinnedPost")]
        public async Task<IActionResult> GetAll()
        {
            var post = await _dbContext.PinnedPosts.OrderBy(c => c.Id).ToListAsync();

            return Ok(post.Select(c => c.MapToPinnedPostDTO()));
        }

        // inserting values method

        [HttpPost("PinnedPost")]
        public async Task<IActionResult> Post(CreatePinnedPostRequest request)
        {
            var post = new PinnedPost
            {
                PostId = request.PostId,
                UserId = request.UserId
            };

            _dbContext.PinnedPosts.Add(post);

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post.MapToPinnedPostDTO());
        }

        //  Updating values method

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, PinnedPostDTO model)
        {
            var post = await _dbContext.PinnedPosts.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (post != null)
            {
                post.UserId = model.UserId;
                post.PostId = model.PostId;

                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = post.Id }, post.MapToPinnedPostDTO());
            }

            return NotFound();
        }

       // Deleting values byid method

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != 0)
            {

                var post = await _dbContext.PinnedPosts.FirstOrDefaultAsync(x => x.Id == id);

                if (post != null)
                {
                    _dbContext.PinnedPosts.Remove(post);


                    await _dbContext.SaveChangesAsync();

                    return Ok();

                }

            }

            return NotFound();
        }
    }
}


