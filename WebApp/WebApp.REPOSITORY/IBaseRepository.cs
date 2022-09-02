using System;
using System.Linq.Expressions;
namespace WebPage.REPOSITORY
{
	public interface IBaseRepository<T> where T:class,new()
	{
        IEnumerable<T> GetAll();
        IEnumerable<T> GetbyEntity(Func<T, bool> entity);
        void Add(T entity);
        void AddRange(T entity);
        int Update(T entity);
        void Delete(T entity);
        bool DeletebyEntity(Func<T, bool> entity);

    }
}

