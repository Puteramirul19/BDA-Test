using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using BDA.Entities;

namespace BDA.Repository
{
    public interface IGenericRepository<T> where T : AuditableEntity
    {
        IEnumerable<T> GetWithRawSql(string query, params object[] parameters);
        IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");
        IEnumerable<T> GetAll();
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        T GetByID(object id);
        T Add(T entity);
        void Delete(object id);
        T Delete(T entity);
        void Update(T entity);
        void Save();
    }
}
