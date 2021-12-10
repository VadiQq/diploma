using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PDMF.Data.Contexts;
using PDMF.Data.Entities;
using PDMF.Data.Repositories.Abstract;

namespace PDMF.Data.Repositories
{
    public class ParseTaskRepository : RepositoryBase<ParseTask>
    {
        public ParseTaskRepository(PDMFDatabaseContext context) : base(context)
        {
        }

        public DbSet<ParseTask> ParseTasksContextAccess => Context.ParseTasks;
        
        public override async Task<ParseTask> Get(string id)
        {
            var entity = await Context.ParseTasks.FirstOrDefaultAsync(dataset => dataset.Id == id);
            return entity;
        }

        public override async Task<ParseTask> Create(ParseTask dataset)
        {
            await Context.ParseTasks.AddAsync(dataset);
            return dataset;
        }

        public override async Task<ParseTask> Delete(string id)
        {
            var entity = await Context.ParseTasks.FirstOrDefaultAsync(dataset => dataset.Id == id);
            Context.ParseTasks.Remove(entity);
            return entity;
        }

        public override async Task<ParseTask> Update(ParseTask updatedDataset)
        {
            var entity = await Context.ParseTasks.FirstOrDefaultAsync(dataset => dataset.Id == updatedDataset.Id);
            Context.ParseTasks.Update(updatedDataset);
            return entity;
        }
    }
}