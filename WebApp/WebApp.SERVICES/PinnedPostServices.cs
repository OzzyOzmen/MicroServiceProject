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
    public class PinnedPostServices
    {
        PinnedPostRepository pinnedPostRepository;
        static object _lockobject = new object();

        public PinnedPostServices()
        {
            lock (_lockobject)
            {
                if (pinnedPostRepository == null)
                {
                    pinnedPostRepository = new PinnedPostRepository();
                }
            }
        }
        public IEnumerable<PinnedPostDTO> GetAll()
        {
            return pinnedPostRepository.GetAll().Select(x => new PinnedPostDTO
            {
                Id = x.Id,
                UserId = x.UserId,
                PostId = x.PostId

            }).ToList();
        }

        public void Add(PinnedPostDTO entity)
        {
            PinnedPost pinnedPost = new PinnedPost
            {
                UserId = entity.UserId,
                PostId = entity.PostId
            };
            pinnedPostRepository.Add(pinnedPost);
        }

        public void Put(PinnedPostDTO entity)
        {
            var pinnedPost = pinnedPostRepository.GetAll().Where(x => x.Id == entity.Id).FirstOrDefault();

            if (pinnedPost != null)
            {
                pinnedPost.UserId = entity.UserId;
                pinnedPost.PostId = entity.PostId;

                pinnedPostRepository.Update(pinnedPost);

            }


        }

        public void Delete(PinnedPostDTO entity)
        {
            var pinnedPost = pinnedPostRepository.GetAll().Where(x => x.Id == entity.Id).FirstOrDefault();

            if (pinnedPost != null)
            {
                pinnedPost.UserId = entity.UserId;
                pinnedPost.PostId = entity.PostId;

                pinnedPostRepository.Delete(pinnedPost);
            }

        }

        public bool DeleteById(int Id)
        {
            return pinnedPostRepository.DeletebyEntity(x => x.Id == Id);
        }



    }
}