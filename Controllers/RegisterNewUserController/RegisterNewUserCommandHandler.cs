using System;
using System.Threading.Tasks;
using System.Transactions;
using QA.Infrastructure.BaseCommandHandler;
using QA.Infrastructure.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace QA.Infrastructure.Controllers.RegisterNewUserController
{
    public class RegisterNewUserCommandHandler : BaseController<RegisterNewUserCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RegisterNewUserCommandHandler(IServiceProvider provider)
            : base(provider)
        {
            _userManager = provider.GetService<UserManager<ApplicationUser>>();
            _signInManager = provider.GetService<SignInManager<ApplicationUser>>();
        }

        protected override async Task<dynamic> ExecuteAsync(RegisterNewUserCommand command)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var newUser = new ApplicationUser()
                {
                    UserName = command.UserName.Trim(),
                  
                    Email = command.Email.Trim(),
                   
                    RegistrationDate = DateTime.Now.Date
                };

                var createResult = await _userManager.CreateAsync(newUser, command.Password.Trim());
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException();
                }

                await _signInManager.SignInAsync(newUser, isPersistent: false);
                transactionScope.Complete();
            }

            return default;
        }

        public override void Dispose() => _userManager?.Dispose();
    }
}
