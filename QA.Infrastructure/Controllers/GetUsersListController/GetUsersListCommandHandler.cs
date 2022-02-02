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

namespace QA.Infrastructure.Controllers.GetUsersListController
{
    public class GetUsersListCommandHandler : BaseController<GetUsersListCommand>
    {
        private readonly IDataProtector _dataProtector;

        public GetUsersListCommandHandler(IServiceProvider provider)
            : base(provider)
        {
            _dataProtector = provider.GetService<IDataProtectionProvider>().CreateProtector(Assembly.GetExecutingAssembly().FullName);
        }

        protected override async Task<dynamic> ExecuteAsync(GetUsersListCommand command)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            using var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

            var query = applicationDbContext.Users
                .OrderBy(u => u.UserName)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(command.SearchQuery))
            {
                query = query.Where(u => (u.Email + u.UserName).ToLower().Contains(command.SearchQuery.ToLower().Trim()));
            }

            var queryResult = await query
                .ApplyPaging(command.Page, command.PageSize)
                .Select(u => new
                {
                    ProtectedId = _dataProtector.Protect(u.Id.ToString()),
                    u.UserName,
                   
                  
                    TotalAnswers = u.UserAnswers.Count,
                    TotalQuestions = u.UserQuestions.Count
                })
                .ToListAsync();

            return new ListResult
            {
                TotalItemCount = await query.CountAsync(),
                ItemsList = queryResult
            };
        }

        public override void Dispose() { }
    }
}
