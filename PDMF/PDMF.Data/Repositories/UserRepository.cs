using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PDMF.Data.Contexts;
using PDMF.Data.Entities;
using PDMF.Data.Repositories.Abstract;

namespace PDMF.Data.Repositories
{
    public class UserRepository : RepositoryBase<User>
    {
        public UserRepository(PDMFDatabaseContext context) : base(context)
        {
        }
        
        public DbSet<User> UsersContextAccess => Context.AspNetUsers;
        
        public override async Task<User> Get(string id)
        {
            var entity = await Context.AspNetUsers.FirstOrDefaultAsync(User => User.Id == id);
            return entity;
        }

        public override async Task<User> Create(User User)
        {
            await Context.AspNetUsers.AddAsync(User);
            return User;
        }

        public override async Task<User> Delete(string id)
        {
            var entity = await Context.AspNetUsers.FirstOrDefaultAsync(User => User.Id == id);
            Context.AspNetUsers.Remove(entity);
            return entity;
        }

        public override async Task<User> Update(User updatedUser)
        {
            var entity = await Context.AspNetUsers.FirstOrDefaultAsync(User => User.Id == updatedUser.Id);
            Context.AspNetUsers.Update(updatedUser);
            return entity;
        }
    }
}