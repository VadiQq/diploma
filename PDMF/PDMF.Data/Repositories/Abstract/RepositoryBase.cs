using System.Threading.Tasks;
using PDMF.Data.Contexts;
using PDMF.Data.Entities;

namespace PDMF.Data.Repositories.Abstract
{
    public abstract class RepositoryBase<T>
    {
        protected readonly PDMFDatabaseContext Context;

        protected RepositoryBase(PDMFDatabaseContext context)
        {
            Context = context;
        }

        public abstract Task<T> Get(string id);
        
        public abstract Task<T> Create(T dataset);
        
        public abstract Task<T> Delete(string id);

        public abstract Task<T> Update(T dataset);

        public async Task Commit()
        {
            await Context.SaveChangesAsync();
        }
    }
}