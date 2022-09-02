using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebPage.DTO;
using WebPage.ORM.Data;
using WebPage.REPOSITORY;

namespace WebPAge.Services
{
    public class postServices
    {
        private readonly PostRepository postRepository;
        static object _lockobject = new object();

        public postServices()
        {
            lock (_lockobject)
            {
                if (postRepository == null)
                {
                    postRepository = new PostRepository();
                }
            }
        }
        public IEnumerable<PostDTO> GetAll()
        {
            return postRepository.GetAll().Select(x => new PostDTO
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                UserId = x.UserId,
                Source = x.Source,
                Thumbnail = x.Thumbnail,
                RatingCount = x.RatingCount,
                CreatedDate = x.CreatedDate


            }).ToList();
        }

        public void Add(PostDTO entity)
        {
            Post post = new Post
            {
                CategoryId = entity.CategoryId,
                UserId = entity.UserId,
                Source = entity.Source,
                Thumbnail = entity.Thumbnail,
                RatingCount = entity.RatingCount,
                CreatedDate = entity.CreatedDate
            };
            postRepository.Add(post);
        }

        public void Put(PostDTO entity)
        {
            var post = postRepository.GetAll().Where(x => x.Id == entity.Id).FirstOrDefault();

            if (post != null)
            {
                post.CategoryId = entity.CategoryId;
                post.UserId = entity.UserId;
                post.Source = entity.Source;
                post.Thumbnail = entity.Thumbnail;
                post.RatingCount = entity.RatingCount;
                post.CreatedDate = entity.CreatedDate;

                postRepository.Update(post);
            }



        }

        public void Delete(PostDTO entity)
        {
            var post = postRepository.GetAll().Where(x => x.Id == entity.Id).FirstOrDefault();

            if (post != null)
            {
                post.CategoryId = entity.CategoryId;
                post.UserId = entity.UserId;
                post.Source = entity.Source;
                post.Thumbnail = entity.Thumbnail;
                post.RatingCount = entity.RatingCount;
                post.CreatedDate = entity.CreatedDate;

                postRepository.Delete(post);
            }

        }

        public bool DeleteById(int Id)
        {
            return postRepository.DeletebyEntity(x => x.Id == Id);
        }



    }
}

