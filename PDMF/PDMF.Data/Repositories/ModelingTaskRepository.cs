using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PDMF.Data.Contexts;
using PDMF.Data.Entities;
using PDMF.Data.Repositories.Abstract;

namespace PDMF.Data.Repositories
{
    public class ModelingTaskRepository : RepositoryBase<ModelingTask>
    {
        public ModelingTaskRepository(PDMFDatabaseContext context) : base(context)
        {
        }

        public DbSet<ModelingTask> ModelingTasksContextAccess => Context.ModelingTasks;
        
        public override async Task<ModelingTask> Get(string id)
        {
            var entity = await Context.ModelingTasks.FirstOrDefaultAsync(dataset => dataset.Id == id);
            return entity;
        }

        public override async Task<ModelingTask> Create(ModelingTask dataset)
        {
            await Context.ModelingTasks.AddAsync(dataset);
            return dataset;
        }

        public override async Task<ModelingTask> Delete(string id)
        {
            var entity = await Context.ModelingTasks.FirstOrDefaultAsync(dataset => dataset.Id == id);
            Context.ModelingTasks.Remove(entity);
            return entity;
        }

        public override async Task<ModelingTask> Update(ModelingTask updatedDataset)
        {
            var entity = await Context.ModelingTasks.FirstOrDefaultAsync(dataset => dataset.Id == updatedDataset.Id);
            Context.ModelingTasks.Update(updatedDataset);
            return entity;
        }
    }
}