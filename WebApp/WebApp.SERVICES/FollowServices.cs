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
    public class FollowServices
    {
        FollowRepository followRepository;
        static object _lockobject = new object();

        public FollowServices()
        {
            lock (_lockobject)
            {
                if (followRepository == null)
                {
                    followRepository = new FollowRepository();
                }
            }
        }
        public IEnumerable<FollowDTO> GetAll()
        {
            return followRepository.GetAll().Select(x => new FollowDTO
            {
                Id = x.Id,
                FollowedId = x.FollowedId,
                FollowerId = x.FollowerId

            }).ToList();
        }

        public void Add(FollowDTO entity)
        {
            Follow follow = new Follow
            {
                FollowedId = entity.FollowedId,
                FollowerId = entity.FollowerId
            };
            followRepository.Add(follow);
        }

        public void Put(FollowDTO entity)
        {
            var follow = followRepository.GetAll().Where(x => x.Id == entity.Id).FirstOrDefault();

            if (follow != null)
            {
                follow.FollowedId = entity.FollowedId;
                follow.FollowerId = entity.FollowerId;

                followRepository.Update(follow);
            }



        }

        public void Delete(FollowDTO entity)
        {
            var follow = followRepository.GetAll().Where(x => x.Id == entity.Id).FirstOrDefault();

            if (follow != null)
            {
                follow.FollowedId = entity.FollowedId;
                follow.FollowerId = entity.FollowerId;

                followRepository.Delete(follow);
            }

        }

        public bool DeleteById(int Id)
        {
            return followRepository.DeletebyEntity(x => x.Id == Id);
        }



    }
}