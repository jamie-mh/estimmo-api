using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Estimmo.Runner.Modules
{
    public class AddUser : IModule
    {
        private readonly UserManager<AdminUser> _userManager;
        private readonly ILogger _log = Log.ForContext<AddUser>();

        public AddUser(UserManager<AdminUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task RunAsync(List<string> args)
        {
            if (args.Count < 2)
            {
                _log.Error("Not enough arguments, expected username and password");
                return;
            }

            var username = args[0];
            var password = args[1];

            _log.Information("Adding user {Username} with password {Password}", username, password);
            await _userManager.CreateAsync(new AdminUser { UserName = username, Email = username }, password);
        }
    }
}
