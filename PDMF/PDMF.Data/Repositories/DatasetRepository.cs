using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PDMF.Data.Contexts;
using PDMF.Data.Entities;
using PDMF.Data.Repositories.Abstract;

namespace PDMF.Data.Repositories
{
    public class DatasetRepository : RepositoryBase<Dataset>
    {
        public DatasetRepository(PDMFDatabaseContext context) : base(context)
        {
        }
        
        public DbSet<Dataset> DatasetsContextAccess => Context.Datasets;
        
        public override async Task<Dataset> Get(string id)
        {
            var entity = await Context.Datasets.FirstOrDefaultAsync(dataset => dataset.Id == id);
            return entity;
        }

        public override async Task<Dataset> Create(Dataset dataset)
        {
            await Context.Datasets.AddAsync(dataset);
            return dataset;
        }

        public override async Task<Dataset> Delete(string id)
        {
            var entity = await Context.Datasets.FirstOrDefaultAsync(dataset => dataset.Id == id);
            Context.Datasets.Remove(entity);
            return entity;
        }

        public override async Task<Dataset> Update(Dataset updatedDataset)
        {
            var entity = await Context.Datasets.FirstOrDefaultAsync(dataset => dataset.Id == updatedDataset.Id);
            Context.Datasets.Update(updatedDataset);
            return entity;
        }
    }
}