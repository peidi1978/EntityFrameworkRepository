using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Text;

namespace EntityFrameworkRepository
{
    /// <summary>
    /// An IRepository implementation that uses the entity framework.
    /// </summary>
    public class EntityFrameworkRepository : IRepository
    {
        protected readonly DbContext dataContext;
        private bool disposed = false;

        public EntityFrameworkRepository()//for unit test purposes
        {
           
        }

        public EntityFrameworkRepository(DbContext ctx)
        {
            dataContext = ctx;
        }
        public virtual void Save()
        {
            dataContext.SaveChanges();
        }

        public virtual async Task SaveAsync()
        {
            await dataContext.SaveChangesAsync();
        }

        public IQueryable<T> Query<T>(System.Linq.Expressions.Expression<Func<T, bool>> filter = null) where T : class
        {
            IQueryable<T> query = null;

            if (filter != null)
                query = dataContext.Set<T>().Where(filter);
            else
                query = dataContext.Set<T>();

            return query;
        }

        public async Task<List<T>> ListAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> filter = null) where T : class
        {
            List<T> query = null;

            if (filter != null)
                query = await dataContext.Set<T>().Where(filter).ToListAsync();
            else
                query = await dataContext.Set<T>().ToListAsync();

            return query;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dataContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public T FirstOrDefault<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            return dataContext.Set<T>().FirstOrDefault(predicate);
        }

        public async Task<T> FirstOrDefaultAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            return await dataContext.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public T Find<T>(params object[] keys) where T : class
        {
            return dataContext.Set<T>().Find(keys);
        }

        public async Task<T> FindAsync<T>(params object[] keys) where T : class
        {
            return await dataContext.Set<T>().FindAsync(keys);
        }

        public void Add<T>(T entity) where T : class
        {
            dataContext.Set<T>().Add(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            DbEntityEntry entityEntry = dataContext.Entry(entity);
            if (entityEntry.State == EntityState.Detached)
            {
                dataContext.Set<T>().Attach(entity);
                entityEntry.State = EntityState.Modified;
            }
        }

        public void Delete<T>(T entity) where T : class
        {
            dataContext.Set<T>().Remove(entity);
        }

        public void SetEntityState<T>(T entity, ObjectState state) where T : class
        {
            EntityState es = EntityState.Unchanged;
            switch (state)
            {
                case ObjectState.Added:
                    es = EntityState.Added;
                    break;
                case ObjectState.Modified:
                    es = EntityState.Modified;
                    break;
                case ObjectState.Deleted:
                    es = EntityState.Deleted;
                    break;
            }
            dataContext.Entry<T>(entity).State = es;
        }

        public IQueryable<T> RunQuery<T>(string query, params object[] parameters)
        {
            var ps = GetSqlParameters(query, parameters);

            return dataContext.Database.SqlQuery<T>(query, ps.ToArray()).AsQueryable<T>();
        }

        public int ExecuteSql(string query, params object[] parameters)
        {
            var ps = GetSqlParameters(query, parameters);

            return dataContext.Database.ExecuteSqlCommand(query, ps);
        }

        private SqlParameter[] GetSqlParameters(string query, params object[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            List<SqlParameter> ps = new List<SqlParameter>();

            var args = query.Split(' ');
            int pi = 0;
            for (int i = 0; i < args.Length - 1; i++)
            {
                string name = args[i + 1].Trim(',');
                if (name.StartsWith("@") && pi < parameters.Length)
                {
                    ps.Add(new SqlParameter(name, parameters[pi] == null ? DBNull.Value : parameters[pi]));
                    pi++;
                }
            }

            return ps.ToArray();
        }

        public ObjectState GetEntityState<T>(T entity) where T : class
        {
            ObjectState os = ObjectState.Unchanged;
            switch (dataContext.Entry(entity).State)
            {
                case EntityState.Added:
                    os = ObjectState.Added;
                    break;
                case EntityState.Modified:
                    os = ObjectState.Modified;
                    break;
                case EntityState.Deleted:
                    os = ObjectState.Deleted;
                    break;
            }
            return os;
        }

        public int Count<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            return dataContext.Set<T>().Count(predicate);
        }


        public void AddRange<T>(IEnumerable<T> entities) where T : class
        {
            dataContext.Set<T>().AddRange(entities);
        }
    }
}