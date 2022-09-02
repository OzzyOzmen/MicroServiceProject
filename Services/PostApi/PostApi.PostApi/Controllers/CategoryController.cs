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
    public class CategoryController : Controller
    {
        private readonly MicroServiceDBContext _dbContext; // our db context
        // creating lock object
        static object _lockobject = new object();

        //set dbcontext once in Constructor

        public CategoryController(MicroServiceDBContext dbContext)
        {
            lock (_lockobject)
            {
                _dbContext = dbContext;
            }

        }

       // Listing values by Id method
       [HttpGet]
       [Route("Category/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _dbContext.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();

            if (category != null)
            {
                return Ok(category.MapToCategoryDTO());
            }

            return NotFound();
        }

      //  Listing all values method

       [HttpGet("Category")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _dbContext.Categories.OrderBy(c => c.CategoryName).ToListAsync();

           
                return Ok(categories.Select(c => c.MapToCategoryDTO()));
          
        }

       // inserting values method

       [HttpPost("Category")]
        public async Task<IActionResult> Post(CreateCategoryRequest request)
        {
            var category = new Category
            {
                CategoryName = request.CategoryName
            };

            _dbContext.Categories.Add(category);

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category.MapToCategoryDTO());
        }

      //  Updating values method

       [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, CategoryDTO model)
        {
            var category = await _dbContext.Categories.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (category != null)
            {
                category.CategoryName = model.CategoryName;

                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = category.Id }, category.MapToCategoryDTO());
            }

            return NotFound();
        }

       // Deleting values by id method

       [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != 0)
            {

                var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (category!=null)
                {
                    _dbContext.Categories.Remove(category);


                    await _dbContext.SaveChangesAsync();

                    return Ok();

                }
               
            }

            return NotFound();
        }
    }
}

