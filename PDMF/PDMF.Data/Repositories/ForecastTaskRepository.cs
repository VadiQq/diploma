using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PDMF.Data.Contexts;
using PDMF.Data.Entities;
using PDMF.Data.Repositories.Abstract;

namespace PDMF.Data.Repositories
{
    public class ForecastTaskRepository : RepositoryBase<ForecastTask>
    {
        public ForecastTaskRepository(PDMFDatabaseContext context) : base(context)
        {
        }

        public DbSet<ForecastTask> ForecastTasksContextAccess => Context.ForecastTasks;
        
        public override async Task<ForecastTask> Get(string id)
        {
            var entity = await Context.ForecastTasks.FirstOrDefaultAsync(dataset => dataset.Id == id);
            return entity;
        }

        public override async Task<ForecastTask> Create(ForecastTask dataset)
        {
            await Context.ForecastTasks.AddAsync(dataset);
            return dataset;
        }

        public override async Task<ForecastTask> Delete(string id)
        {
            var entity = await Context.ForecastTasks.FirstOrDefaultAsync(dataset => dataset.Id == id);
            Context.ForecastTasks.Remove(entity);
            return entity;
        }

        public override async Task<ForecastTask> Update(ForecastTask updatedDataset)
        {
            var entity = await Context.ForecastTasks.FirstOrDefaultAsync(dataset => dataset.Id == updatedDataset.Id);
            Context.ForecastTasks.Update(updatedDataset);
            return entity;
        }
    }
}