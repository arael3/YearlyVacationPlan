using RPU.DataAccess.Repository.IRepository;

namespace RPU.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    public IVacationPlanRepository VacationPlan { get; private set; }
    public IYearlyHolidayScheduleRepository YearlyHolidaySchedule { get; private set; }
    public ISharedVacationPlanRepository SharedVacationPlan { get; private set; }
    public IIdentityUserRepository IdentityUser { get; private set; }

    private AppDbContext _db;

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        VacationPlan = new VacationPlanRepository(_db);
        YearlyHolidaySchedule = new YearlyHolidayScheduleRepository(_db);
        SharedVacationPlan = new SharedVacationPlanRepository(_db);
        IdentityUser = new IdentityUserRepository(_db);
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
