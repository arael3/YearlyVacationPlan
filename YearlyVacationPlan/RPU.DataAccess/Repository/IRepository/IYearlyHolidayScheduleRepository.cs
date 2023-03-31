using RPU.Models;

namespace RPU.DataAccess.Repository.IRepository;

public interface IYearlyHolidayScheduleRepository : IRepository<YearlyHolidaySchedule>
{
    void Update(YearlyHolidaySchedule obj);
}
