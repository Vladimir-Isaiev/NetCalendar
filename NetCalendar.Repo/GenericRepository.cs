using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using LinqKit;

namespace NetCalendar.Repo
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected DbContext _entities;
        protected readonly IDbSet<T> _dbset;

        public GenericRepository(DbContext entities)
            {
            _entities = entities;
            _dbset = _entities.Set<T>();
            }



        public void AddOrUpdate(T obj)
        {
            _dbset.AddOrUpdate(obj);
        }

        public void Delete(T obj)
        {
            _dbset.Remove(obj);
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> query = _dbset.AsExpandable().Where(predicate);
            return query;
        }

        public T Get(int id)
        {
            return _dbset.Find(id);
        }


        public IEnumerable<T> GetAll()
        {
            return _dbset.AsEnumerable<T>();
        }
    }
}
