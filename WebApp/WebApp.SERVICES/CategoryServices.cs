using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebPage.DTO;
using WebPage.ORM.Data;
using WebPage.REPOSITORY;

namespace WebPage.Services
{
    public class CategoryServices
    {
        CategoryRepository categoryRepository;

        static object _lockobject = new object();

        public CategoryServices()
        {
            lock (_lockobject)
            {
                if (categoryRepository == null)
                {
                    categoryRepository = new CategoryRepository();
                }
            }
        }
        public IEnumerable<CategoryDTO> GetAll()
        {
            return categoryRepository.GetAll().Select(x => new CategoryDTO
            {
                Id = x.Id,
                CategoryName = x.CategoryName

            }).ToList();
        }

        public void Add(CategoryDTO entity)
        {
            Category category = new Category
            {
                CategoryName = entity.CategoryName
            };
            categoryRepository.Add(category);
        }

        public void Put(CategoryDTO entity)
        {
            var category = categoryRepository.GetAll().Where(x => x.Id == entity.Id).FirstOrDefault();

            if (category != null)
            {
                category.CategoryName = entity.CategoryName;

                categoryRepository.Update(category);
            }



        }

        public void Delete(CategoryDTO entity)
        {
            var category = categoryRepository.GetAll().Where(x => x.Id == entity.Id).FirstOrDefault();

            if (category != null)
            {
                category.CategoryName = entity.CategoryName;

                categoryRepository.Delete(category);
            }

        }

        public bool DeleteById(int Id)
        {
            return categoryRepository.DeletebyEntity(x => x.Id == Id);
        }



    }
}