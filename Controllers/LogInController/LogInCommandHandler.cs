using System;
using System.Threading.Tasks;
using QA.Infrastructure.BaseCommandHandler;
using QA.Infrastructure.Common;
using QA.Infrastructure.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace QA.Infrastructure.Controllers.LogInController
{
    public class LogInCommandHandler : BaseController<LogInCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogInCommandHandler(IServiceProvider provider)
            : base(provider)
        {
            _userManager = provider.GetService<UserManager<ApplicationUser>>();
            _signInManager = provider.GetService<SignInManager<ApplicationUser>>();
        }

        protected override async Task<dynamic> ExecuteAsync(LogInCommand command)
        {
            var applicationUser = await _userManager.FindByEmailAsync(command.Email.Trim().Normalize().ToLowerInvariant());
            var result = await _signInManager.PasswordSignInAsync(applicationUser != null ? applicationUser.UserName : string.Empty, command.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new CustomException("Invalid user account or password.");
            }

            return default;
        }

        public override void Dispose() => _userManager?.Dispose();
    }
}
