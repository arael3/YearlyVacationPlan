using RPU.DataAccess.Repository.IRepository;
using RPU.Models;

namespace RPU.DataAccess.Repository;

public class VacationPlanRepository : Repository<VacationPlan>, IVacationPlanRepository
{
    private AppDbContext _db;
    public VacationPlanRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(VacationPlan obj)
    {
        _db.VacationPlans.Update(obj);
    }
}
