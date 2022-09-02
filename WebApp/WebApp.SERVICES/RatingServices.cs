using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebPage.DTO;
using WebPage.ORM.Data;
using WebPage.REPOSITORY;

namespace WebPage.SERVICES
{
    public class RatingServices
    {
        RatingsRepository ratingsRepository;
        static object _lockobject = new object();

        public RatingServices()
        {
            lock (_lockobject)
            {

                if (ratingsRepository == null)
                {
                    ratingsRepository = new RatingsRepository();
                }
            }
        }
        public IEnumerable<RatingDTO> GetAll()
        {
            return ratingsRepository.GetAll().Select(x => new RatingDTO
            {
                Id = x.Id,
                RatingCount = x.RatingCount,
                PostId = x.PostId

            }).ToList();
        }

        public void Add(RatingDTO entity)
        {
            Rating rating = new Rating
            {
                RatingCount = entity.RatingCount,
                PostId = entity.PostId
            };
            ratingsRepository.Add(rating);
        }

        public void Put(RatingDTO entity)
        {
            var rating = ratingsRepository.GetAll().Where(x => x.Id == entity.Id).FirstOrDefault();

            if (rating != null)
            {
                rating.RatingCount = entity.RatingCount;
                rating.PostId = entity.PostId;

                ratingsRepository.Update(rating);

            }


        }

        public void Delete(RatingDTO entity)
        {
            var rating = ratingsRepository.GetAll().Where(x => x.Id == entity.Id).FirstOrDefault();

            if (rating != null)
            {
                rating.RatingCount = entity.RatingCount;
                rating.PostId = entity.PostId;

                ratingsRepository.Delete(rating);
            }

        }

        public bool DeleteById(int Id)
        {
            return ratingsRepository.DeletebyEntity(x => x.Id == Id);
        }



    }
}