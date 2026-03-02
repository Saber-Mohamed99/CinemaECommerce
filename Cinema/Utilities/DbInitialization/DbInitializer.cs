using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace CinemaECommerce.Utilities.DbInitialization
{
    public class DbInitializer :IDbInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DbInitializer(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager
            ,ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }
        
        public async Task Initialize()
        {
            if( _context.Database.GetPendingMigrations().Any())
                _context.Database.Migrate();

            if (_roleManager.Roles.IsNullOrEmpty())
            {
                await  _roleManager.CreateAsync(new (SD.Super_Admin_Role));
                await _roleManager.CreateAsync(new (SD.Admin_Role));
                await _roleManager.CreateAsync(new (SD.Employee_Role));
                await _roleManager.CreateAsync(new (SD.Customer_Role));
            }

            await _userManager.CreateAsync(new()
            {
                FName = "Saber",
                LName="Mohamed",
                Email="Sbermo@gmail.com",
                EmailConfirmed=true,
                UserName="Saber123"

            },"Admin123@");
            var user = await _userManager.FindByNameAsync("Saber123");
            if (user != null)
              await  _userManager.AddToRoleAsync(user, SD.Super_Admin_Role);
        }
    }
}
