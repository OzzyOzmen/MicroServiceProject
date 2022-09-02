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


namespace PostApi.PostApi.Controllers
{
    [Route("api/[Controller]")]
    public class RatingController : Controller
    {
        private readonly MicroServiceDBContext _dbContext; // our db context
        static object _lockobject = new object();

        //set dbcontext once in Constructor

        public RatingController(MicroServiceDBContext dbContext)
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
            var rating = await _dbContext.Ratings.Where(c => c.Id == id).FirstOrDefaultAsync();

            if (rating != null)
            {
                return Ok(rating.MapToRatingDTO());
            }

            return NotFound();
        }

        //  Listing all values method

        [HttpGet("Rating")]
        public async Task<IActionResult> GetAll()
        {
            var rating = await _dbContext.Ratings.OrderBy(c => c.RatingCount).ToListAsync();

            return Ok(rating.Select(c => c.MapToRatingDTO()));
        }

        // inserting values method

        [HttpPost("Rating")]
        public async Task<IActionResult> Post(CreateRatingRequest request)
        {
            var rating = new Rating
            {
                RatingCount = request.RatingCount,
                PostId = request.PostId
            };

            _dbContext.Ratings.Add(rating);

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = rating.Id }, rating.MapToRatingDTO());
        }

        //  Updating values method

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, RatingDTO model)
        {
            var rating = await _dbContext.Ratings.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (rating != null)
            {
                rating.RatingCount = model.RatingCount;
                rating.PostId = model.PostId;

                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = rating.Id }, rating.MapToRatingDTO());
            }

            return NotFound();
        }

        // Deleting values by id method

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != 0)
            {

                var rating = await _dbContext.Ratings.FirstOrDefaultAsync(x => x.Id == id);

                if (rating != null)
                {
                    _dbContext.Ratings.Remove(rating);


                    await _dbContext.SaveChangesAsync();

                    return Ok();

                }

            }

            return NotFound();
        }
    }
}

