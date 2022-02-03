using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using QA.Infrastructure.ApplicationContext;
using QA.Infrastructure.BaseCommandHandler;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace QA.Infrastructure.Controllers.GetStatisticsController
{
    public class GetStatisticsCommandHandler : BaseController<GetStatisticsCommand>
    {
        private readonly IDataProtector _dataProtector;

        public GetStatisticsCommandHandler(IServiceProvider provider)
            : base(provider)
        {
            _dataProtector = provider.GetService<IDataProtectionProvider>().CreateProtector(Assembly.GetExecutingAssembly().FullName);
        }

        protected override async Task<dynamic> ExecuteAsync(GetStatisticsCommand command)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            using var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

            var totalQuestions = await applicationDbContext.Questions.CountAsync();
            var totalAnswers = await applicationDbContext.Answers.CountAsync();

            var topUsersList = await applicationDbContext.Users
               .OrderByDescending(u => u.UserAnswers.Count(a => a.BestAnswer))
                    .ThenBy(u => u.UserName)
               .Take(5)
               .Select(u => new
               {
                   ProtectedId = _dataProtector.Protect(u.Id.ToString()),
                   u.UserName,
                   AnswersProvided = u.UserAnswers.Count(a => a.BestAnswer)
               })
               .ToListAsync();

            return new
            {
                TotalQuestions = totalQuestions,
                TotalAnswers = totalAnswers,
                TopUsersList = topUsersList
            };
        }

        public override void Dispose() { }
    }
}
