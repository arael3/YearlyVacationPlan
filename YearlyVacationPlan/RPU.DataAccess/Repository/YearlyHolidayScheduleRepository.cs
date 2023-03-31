using RPU.DataAccess.Repository.IRepository;
using RPU.Models;

namespace RPU.DataAccess.Repository;

public class YearlyHolidayScheduleRepository : Repository<YearlyHolidaySchedule>, IYearlyHolidayScheduleRepository
{
    private AppDbContext _db;
    public YearlyHolidayScheduleRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(YearlyHolidaySchedule obj)
    {
        _db.YearlyHolidaySchedules.Update(obj);
    }
}
