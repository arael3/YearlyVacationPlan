using Microsoft.AspNetCore.Identity;

namespace RPU.DataAccess.Repository.IRepository;

public interface IIdentityUserRepository : IRepository<IdentityUser>
{
    void Update(IdentityUser obj);
}
