using BDA.Data;
using BDA.Entities;
using BDA.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BDA.DataAccessRepository
{

    public abstract class GenericRepository<T> : IGenericRepository<T>
       where T : AuditableEntity
    {
        protected DbContext Entities;
        protected readonly DbSet<T> Dbset;

        protected GenericRepository(DbContext context)
        {
            Entities = context;
            Dbset = context.Set<T>();
        }

        //public virtual IEnumerable<T> GetWithRawSql(string query, params object[] parameters)
        //{
        //    return Dbset.SqlQuery(query, parameters).ToList();
        //}

        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = Dbset;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
            (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual IEnumerable<T> GetAll()
        {
            return Dbset.AsEnumerable<T>();
        }

        public virtual T GetByID(object id)
        {
            return Dbset.Find(id);
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {

            IEnumerable<T> query = Dbset.Where(predicate).AsEnumerable();
            return query;
        }

        public virtual T Add(T entity)
        {
            return Dbset.Add(entity).Entity;
        }

        public virtual void Delete(object id)
        {
            T entityToDelete = Dbset.Find(id);
            Delete(entityToDelete);
        }

        public virtual T Delete(T entity)
        {
            return Dbset.Remove(entity).Entity;
        }

        public virtual void Update(T entity)
        {
            Entities.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Save()
        {
            Entities.SaveChanges();
        }

        public IEnumerable<T> GetWithRawSql(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        //public IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
