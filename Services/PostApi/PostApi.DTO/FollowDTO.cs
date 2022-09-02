using System;
namespace PostApi.DTO
{
	public class FollowDTO
	{
		public int Id { get; set; }
		public int FollowerId { get; set; }
		public int FollowedId { get; set; }
	}
}

