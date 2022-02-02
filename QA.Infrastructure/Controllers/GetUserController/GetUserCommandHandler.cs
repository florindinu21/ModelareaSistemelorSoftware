using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using QA.Infrastructure.ApplicationContext;
using QA.Infrastructure.BaseCommandHandler;
using QA.Infrastructure.Common;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace QA.Infrastructure.Controllers.GetUserController
{
    public class GetUserCommandHandler : BaseController<GetUserCommand>
    {
        private readonly IDataProtector _dataProtector;

        public GetUserCommandHandler(IServiceProvider provider)
            : base(provider)
        {
            _dataProtector = provider.GetService<IDataProtectionProvider>().CreateProtector(Assembly.GetExecutingAssembly().FullName);
        }

        protected override async Task<dynamic> ExecuteAsync(GetUserCommand command)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            using var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            var userId = int.Parse(_dataProtector.Unprotect(command.ProtectedUserId));

            var registeredUser = await applicationDbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    ProtectedId = _dataProtector.Protect(u.Id.ToString()),
                    u.UserName,
                    u.Email,
                    
                    u.RegistrationDate,
                    
                    TotalAnswers = u.UserAnswers.Count,
                    TotalQuestions = u.UserQuestions.Count
                })
                .FirstOrDefaultAsync();

            if (registeredUser == null)
            {
                throw new CustomException("Invalid user specified.");
            }

            return registeredUser;
        }

        public override void Dispose() { }
    }
}
