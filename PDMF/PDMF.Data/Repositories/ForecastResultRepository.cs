using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PDMF.Data.Contexts;
using PDMF.Data.Entities;
using PDMF.Data.Repositories.Abstract;

namespace PDMF.Data.Repositories
{
    public class ForecastResultRepository : RepositoryBase<ForecastResult>
    {
        public ForecastResultRepository(PDMFDatabaseContext context) : base(context)
        {
        }

        public DbSet<ForecastResult> ForecastResultsContextAccess => Context.ForecastResults;
        
        public override async Task<ForecastResult> Get(string id)
        {
            var entity = await Context.ForecastResults.FirstOrDefaultAsync(dataset => dataset.Id == id);
            return entity;
        }

        public override async Task<ForecastResult> Create(ForecastResult dataset)
        {
            await Context.ForecastResults.AddAsync(dataset);
            return dataset;
        }

        public override async Task<ForecastResult> Delete(string id)
        {
            var entity = await Context.ForecastResults.FirstOrDefaultAsync(dataset => dataset.Id == id);
            Context.ForecastResults.Remove(entity);
            return entity;
        }

        public override async Task<ForecastResult> Update(ForecastResult updatedDataset)
        {
            var entity = await Context.ForecastResults.FirstOrDefaultAsync(dataset => dataset.Id == updatedDataset.Id);
            Context.ForecastResults.Update(updatedDataset);
            return entity;
        }
    }
}