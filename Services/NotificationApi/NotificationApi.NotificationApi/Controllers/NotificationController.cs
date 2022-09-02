using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationApi.DTO;
using NotificationApi.ORM.Data;
using Microsoft.JSInterop.Implementation;
using NotificationApi.INFRASTRUCTURE.Extensions;
using NotificationApi.INFRASTRUCTURE.Requests;

namespace NotificationApi.PostApi.Controllers
{
    [Route("api/[Controller]")]
    public class NotificationController : Controller
    {
        private readonly MicroServiceDBContext _dbContext; // our db context
       // creating lock object
        static object _lockobject = new object();

        //set dbcontext once in Constructor

        public NotificationController(MicroServiceDBContext dbContext)
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
            var notification = await _dbContext.Notifications.Where(c => c.Id == id).FirstOrDefaultAsync();

            if (notification != null)
            {
                return Ok(notification.MapToNotificationDTO());
            }

            return NotFound();
        }

        //  Listing all values method

        [HttpGet("Notification")]
        public async Task<IActionResult> GetAll()
        {
            var notification = await _dbContext.Notifications.OrderBy(c => c.Id).ToListAsync();

            return Ok(notification.Select(c => c.MapToNotificationDTO()));
        }

        // inserting values method

        [HttpPost("Notification")]
        public async Task<IActionResult> Post(CreateNotificationRequest request)
        {
            var notification = new Notification
            {
                FollowedUserId = request.FollowedUserId,
                FollowerId = request.FollowerId,
                Notifications = request.Notifications,
                PinnedVideosId = request.PinnedVideosId,
                PostId = request.PostId,
                RatingId = request.RatingId

            };

            _dbContext.Notifications.Add(notification);

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = notification.Id }, notification.MapToNotificationDTO());
        }

        //  Updating values method

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, NotificationDTO model)
        {
            var notification = await _dbContext.Notifications.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (notification != null)
            {
                notification.FollowedUserId = model.FollowedUserId;
                notification.FollowerId = model.FollowerId;
                notification.Notifications = model.Notifications;
                notification.PinnedVideosId = model.PinnedVideosId;
                notification.PostId = model.PostId;
                notification.RatingId = model.RatingId;

                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = notification.Id }, notification.MapToNotificationDTO());
            }

            return NotFound();
        }

        // Deleting values by id method

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != 0)
            {

                var notification = await _dbContext.Notifications.FirstOrDefaultAsync(x => x.Id == id);

                if (notification != null)
                {
                    _dbContext.Notifications.Remove(notification);


                    await _dbContext.SaveChangesAsync();

                    return Ok();

                }

            }

            return NotFound();
        }
    }
}

