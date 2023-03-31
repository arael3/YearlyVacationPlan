using RPU.DataAccess.Repository.IRepository;
using RPU.Models;
using RPU.Models.Enums;

namespace RPU.DataAccess.Repository;

public class SharedVacationPlanRepository : Repository<SharedVacationPlan>, ISharedVacationPlanRepository
{
    private AppDbContext _db;
    public SharedVacationPlanRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(SharedVacationPlan obj)
    {
        _db.SharedVacationPlans.Update(obj);
    }

    public void UpdateStatus(int id, SharedVacationPlanStatuses status)
    {
        var sharedVacationPlanFromDb = _db.SharedVacationPlans.FirstOrDefault(u => u.Id == id);
        if (sharedVacationPlanFromDb is not null)
        {
            sharedVacationPlanFromDb.Status = status;
        }
    }
}
