using System;
namespace PostApi.INFRASTRUCTURE.Requests
{
	// this is our model requesting page.

	public class CreateCategoryRequest
	{
        public int Id { get; set; }
        public string CategoryName { get; set; }
	}
}

