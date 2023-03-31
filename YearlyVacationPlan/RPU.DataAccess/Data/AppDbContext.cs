using RPU.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RPU.DataAccess;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<VacationPlan> VacationPlans { get; set; }
    public DbSet<YearlyHolidaySchedule> YearlyHolidaySchedules { get; set; }
    public DbSet<SharedVacationPlan> SharedVacationPlans { get; set; }
    public DbSet<IdentityUser> IdentityUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VacationPlan>()
        .HasOne(vp => vp.Deputy1ApplicationUser)
        .WithMany()
        .HasForeignKey(vp => vp.Deputy1ApplicationUserId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VacationPlan>()
            .HasOne(vp => vp.Deputy2ApplicationUser)
            .WithMany()
            .HasForeignKey(vp => vp.Deputy2ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<SharedVacationPlan>()
        .HasOne(svp => svp.SenderApplicationUser)
        .WithMany()
        .HasForeignKey(svp => svp.SenderApplicationUserId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SharedVacationPlan>()
            .HasOne(svp => svp.ReceiverApplicationUser)
            .WithMany()
            .HasForeignKey(svp => svp.ReceiverApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
