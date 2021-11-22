using System.Collections.Generic;
using System.Linq;

namespace DMSTask.Repositories
{
    public interface IRepository<TEntity, TKey>
    {
        IEnumerable<TEntity> GetAll();
        TEntity GetById(TKey id);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
