using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PDMF.Data.Contexts;
using PDMF.Data.Entities;
using PDMF.Data.Repositories.Abstract;

namespace PDMF.Data.Repositories
{
    public class ModelingResultRepository : RepositoryBase<ModelingResult>
    {
        public ModelingResultRepository(PDMFDatabaseContext context) : base(context)
        {
        }

        public DbSet<ModelingResult> ModelingResultsContextAccess => Context.ModelingResults;
        
        public override async Task<ModelingResult> Get(string id)
        {
            var entity = await Context.ModelingResults.FirstOrDefaultAsync(dataset => dataset.Id == id);
            return entity;
        }

        public override async Task<ModelingResult> Create(ModelingResult dataset)
        {
            await Context.ModelingResults.AddAsync(dataset);
            return dataset;
        }

        public override async Task<ModelingResult> Delete(string id)
        {
            var entity = await Context.ModelingResults.FirstOrDefaultAsync(dataset => dataset.Id == id);
            Context.ModelingResults.Remove(entity);
            return entity;
        }

        public override async Task<ModelingResult> Update(ModelingResult updatedDataset)
        {
            var entity = await Context.ModelingResults.FirstOrDefaultAsync(dataset => dataset.Id == updatedDataset.Id);
            Context.ModelingResults.Update(updatedDataset);
            return entity;
        }
    }
}