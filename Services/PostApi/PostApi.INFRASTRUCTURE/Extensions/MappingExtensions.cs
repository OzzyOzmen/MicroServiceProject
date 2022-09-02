using System;
using PostApi.DTO;
using PostApi.ORM.Data;

namespace PostApi.INFRASTRUCTURE.Extensions
{
	public static class MappingExtensions
	{
        // Here is our mapping page to DTO's

        public static CategoryDTO MapToCategoryDTO(this Category category)
        {
            if (category != null)
            {
                return new CategoryDTO
                {
                    Id = category.Id,
                    CategoryName = category.CategoryName
                };
            }

            return null;
        }

        public static PostDTO MapToPostDTO(this Post post)
        {
            if (post != null)
            {
                return new PostDTO
                {
                    Id = post.Id,
                    Source = post.Source,
                    Thumbnail = post.Thumbnail,
                    UserId = post.UserId,
                    CategoryId = post.CategoryId,
                    CreatedDate = post.CreatedDate,
                    RatingCount= post.RatingCount
                };
            }

            return null;
        }

        public static FollowDTO MapToFollowDTO(this Follow follow)
        {
            if (follow != null)
            {
                return new FollowDTO
                {
                    Id = follow.Id,
                    FollowerId = follow.FollowerId,
                    FollowedId = follow.FollowedId
                };
            }

            return null;
        }

        public static PinnedPostDTO MapToPinnedPostDTO(this PinnedPost post)
        {
            if (post != null)
            {
                return new PinnedPostDTO
                {
                    Id = post.Id,
                    UserId = post.UserId,
                    PostId = post.PostId
                };
            }

            return null;
        }

        public static RatingDTO MapToRatingDTO(this Rating rating)
        {
            if (rating != null)
            {
                return new RatingDTO
                {
                    Id = rating.Id,
                    RatingCount = rating.RatingCount,
                    PostId = rating.PostId
                };
            }

            return null;
        }

        
    }
}

