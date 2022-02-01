using System;
using System.Threading.Tasks;
using QA.Infrastructure.BaseCommandHandler;
using QA.Infrastructure.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace QA.Infrastructure.Controllers.LogOutController
{
    public class LogOutCommandHandler : BaseController<LogOutCommand>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogOutCommandHandler(IServiceProvider provider)
            : base(provider)
        {
            _signInManager = provider.GetService<SignInManager<ApplicationUser>>();
        }

        protected override async Task<dynamic> ExecuteAsync(LogOutCommand command)
        {
            await _signInManager.SignOutAsync();
            return default;
        }

        public override void Dispose() { }
    }
}
