namespace RPU.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    IVacationPlanRepository VacationPlan { get; }
    IYearlyHolidayScheduleRepository YearlyHolidaySchedule { get; }
    ISharedVacationPlanRepository SharedVacationPlan { get; }
    IIdentityUserRepository IdentityUser { get; }

    void Save();
}
