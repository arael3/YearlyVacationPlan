using RPU.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace RPU.DataAccess.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _db;

    public DbInitializer(
        UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext db)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }

    public void Initialize()
    {
        // migrations if they are not applied
        try
        {
            if (_db.Database.GetPendingMigrations().Count() > 0)
            {
                _db.Database.Migrate();
            }
        }
        catch (Exception)
        {

            throw;
        }

        // create roles if they are not created
        if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Indi)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp)).GetAwaiter().GetResult();

            // if roles are created, then we will create admin user as well
            _userManager.CreateAsync(new IdentityUser
            {
                UserName = "admin",
                Email = "admin@test468975.com",
            }, "Admin#468975").GetAwaiter().GetResult();

            IdentityUser user = _db.Users.FirstOrDefault(u => u.UserName == "admin");

            _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            
            user.EmailConfirmed = true;
            _userManager.UpdateAsync(user).GetAwaiter().GetResult();
        }

        return;
    }
}
