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


namespace PostApi.Controllers
{
    [Route("api/[Controller]")]
    public class FollowController : Controller
    {
        private readonly MicroServiceDBContext _dbContext; // our db context
        // creating lock object
        static object _lockobject = new object();

        //set dbcontext once in Constructor

        public FollowController(MicroServiceDBContext dbContext)
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
            var follow = await _dbContext.Follows.Where(c => c.Id == id).FirstOrDefaultAsync();

            if (follow != null)
            {
                return Ok(follow.MapToFollowDTO());
            }

            return NotFound();
        }

        //  Listing all values method

        [HttpGet("Follow")]
        public async Task<IActionResult> GetAll()
        {
            var follow = await _dbContext.Follows.OrderBy(c => c.Id).ToListAsync();

            return Ok(follow.Select(c => c.MapToFollowDTO()));
        }

        // inserting values method

        [HttpPost("Follow")]
        public async Task<IActionResult> Post(CreateFollowRequest request)
        {
            var follow = new Follow
            {
                FollowerId = request.FollowerId,
                FollowedId=request.FollowedId
            };

            _dbContext.Follows.Add(follow);

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = follow.Id }, follow.MapToFollowDTO());
        }

        //  Updating values method

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, FollowDTO model)
        {
            var follow = await _dbContext.Follows.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (follow != null)
            {
                follow.FollowerId = model.FollowerId;
                follow.FollowedId = model.FollowedId;

                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = follow.Id }, follow.MapToFollowDTO());
            }

            return NotFound();
        }

        // Deleting values by id method


       [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != 0)
            {

                var follow = await _dbContext.Follows.FirstOrDefaultAsync(x => x.Id == id);

                if (follow != null)
                {
                    _dbContext.Follows.Remove(follow);


                    await _dbContext.SaveChangesAsync();

                    return Ok();

                }

            }

            return NotFound();
        }
    }
}

