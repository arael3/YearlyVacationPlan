using RPU.Models;

namespace RPU.DataAccess.Repository.IRepository;

public interface IVacationPlanRepository : IRepository<VacationPlan>
{
    void Update(VacationPlan obj);
}
