using System;
using System.Threading.Tasks;
using QA.Infrastructure.Controllers.LogOutController;
using QA.Infrastructure.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace QA.Web.Areas.Identity.Pages.Account
{
    [IgnoreAntiforgeryToken]
    public class LogoutModel : PageModel
    {
        protected LogOutCommandHandler LogOutCommandHandler { get; set; }

        public LogoutModel(IServiceProvider serviceProvider)
        {
            LogOutCommandHandler = serviceProvider.GetService<LogOutCommandHandler>();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await LogOutCommandHandler.HandleAsync(new LogOutCommand());
                    return Redirect("~/");
                }
                else
                {
                    throw new CustomException("The received data model is invalid.");
                }
            }
            catch (Exception ex)
            {
                var exception = ex.GetBaseException();
                ViewData["Errormessage"] = (exception is CustomException)
                        ? $"Error. {exception.Message}"
                        : $"Error. Could not complete this operation.";
            }
            finally
            {
                LogOutCommandHandler?.Dispose();
            }

            return Page();
        }
    }
}
