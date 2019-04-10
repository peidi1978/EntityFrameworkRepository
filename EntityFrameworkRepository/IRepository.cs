using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace EntityFrameworkRepository
{
    public enum ObjectState
    {
        Added = 1,
        Modified,
        Deleted,
        Unchanged
    }
    /// <summary>
    /// A generic repository interface
    /// </summary>
    public interface IRepository : IDisposable
    {
        void Save();
        Task SaveAsync();
        IQueryable<T> Query<T>(Expression<Func<T, bool>> filter = null) where T : class;
        T FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<List<T>> ListAsync<T>(Expression<Func<T, bool>> filter = null) where T : class;
        int Count<T>(Expression<Func<T, bool>> filter = null) where T : class;
        T Find<T>(params object[] keys) where T : class;
        Task<T> FindAsync<T>(params object[] keys) where T : class;
        void Add<T>(T entity) where T : class;
        void AddRange<T>(IEnumerable<T> entities) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void SetEntityState<T>(T entity, ObjectState state) where T : class;
        ObjectState GetEntityState<T>(T entity) where T : class;
        IQueryable<T> RunQuery<T>(string query, params object[] parameters);
        int ExecuteSql(string query, params object[] parameters);
    }
}