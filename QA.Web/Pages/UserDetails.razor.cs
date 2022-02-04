using System.Threading.Tasks;
using QA.Infrastructure.Controllers.GetUserController;
using QA.Infrastructure.Common;
using QA.Web.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace QA.Web.Pages
{
    public class UserDetailsComponent : CustomComponentBase
    {
        [Parameter]
        public string ProtectedId { get; set; }
        public dynamic RegisteredUser { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (!string.IsNullOrEmpty(ProtectedId))
                {
                    using var getUserCommandHandler = ServiceProvider.GetService<GetUserCommandHandler>();
                    object result = await getUserCommandHandler.HandleAsync(new GetUserCommand()
                    {
                        ProtectedUserId = ProtectedId
                    });

                    RegisteredUser = result.ToExpando();
                }
            });
        }
    }
}
