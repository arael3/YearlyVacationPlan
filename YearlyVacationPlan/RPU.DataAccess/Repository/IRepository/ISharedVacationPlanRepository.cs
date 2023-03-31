using RPU.Models;
using RPU.Models.Enums;

namespace RPU.DataAccess.Repository.IRepository;

public interface ISharedVacationPlanRepository : IRepository<SharedVacationPlan>
{
    void Update(SharedVacationPlan obj);
    void UpdateStatus(int id, SharedVacationPlanStatuses status);
}
