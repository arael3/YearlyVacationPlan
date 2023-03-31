using RPU.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;

namespace RPU.DataAccess.Repository;

public class IdentityUserRepository : Repository<IdentityUser>, IIdentityUserRepository
{
    private AppDbContext _db;
    public IdentityUserRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(IdentityUser obj)
    {
        _db.IdentityUsers.Update(obj);
    }
}
