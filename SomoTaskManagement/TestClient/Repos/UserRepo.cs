

using Microsoft.EntityFrameworkCore;
using SomoTaskManagement.Data;
using SomoTaskManagement.Domain.Entities;

namespace TestClient.Repos
{
    public class UserRepo
    {
        private readonly SomoTaskManagemnetContext dbContext;   

        public UserRepo(SomoTaskManagemnetContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Member> GetUserDetails(string username, string password)
        {
            return await dbContext.Member.FirstOrDefaultAsync(user => user.UserName == username && user.Password == password);
        }
    }
}
