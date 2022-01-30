﻿using System.Threading.Tasks;
using QA.Infrastructure.Controllers.QuestionController;
using QA.Infrastructure.Controllers.InsertNewAnswerController;
using QA.Infrastructure.Controllers.SelectBestAnswerController;
using QA.Infrastructure.Controllers.VoteForAnswerHandler;
using QA.Infrastructure.Common;
using QA.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace QA.Web.Pages
{
    public class QuestionComponent : CustomComponentBase
    {
        [Parameter]
        public string ProtectedId { get; set; }
        public dynamic SubmittedQuestion { get; set; }
        public string ModalImage { get; set; }
        public bool ShowModalImage { get; set; }

        public bool EnableEdit = false;

        public string EditedDescription { get; set; } = string.Empty;

        public InsertNewAnswerCommand InsertNewAnswerCommand { get; set; } = new InsertNewAnswerCommand();
        public AuthenticationState AuthenticationState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            InsertNewAnswerCommand.ProtectedQuestionId = ProtectedId;
            AuthenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            await ReloadQuestionAsync();
        }

        protected async Task ReloadQuestionAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (!string.IsNullOrEmpty(ProtectedId))
                {
                    using var getQuestionCommandHandler = ServiceProvider.GetService<GetQuestionCommandHandler>();
                    object result = await getQuestionCommandHandler.HandleAsync(new GetQuestionCommand()
                    {
                        ProtectedQuestionId = ProtectedId
                    });

                    SubmittedQuestion = result.ToExpando();
                }
            });
        }

        public async Task OnViewImageAsync(string image)
        {
            await Task.FromResult(0).ContinueWith(t =>
            {
                ModalImage = image;
                ShowModalImage = true;
            });
        }

        public async Task OnCloseImageAsync()
        {
            await Task.FromResult(0).ContinueWith(t =>
            {
                ModalImage = null;
                ShowModalImage = false;
            });
        }

        public async Task OnUserClickAsync(string protectedId)
        {
            await ExecuteAsync(async () =>
            {
                await Task.FromResult(0).ContinueWith(t => NavigationManager.NavigateTo($"/users/{protectedId}"));
            });
        }

        public async Task SuggestEdit()
        {
            await ExecuteAsync(async () =>
            {
                EnableEdit = true;
            });
        }

        public async Task SubmitEdit()
        {
            await ExecuteAsync(async () =>
            {
                EnableEdit = false;

            });
        }

        [Authorize]
        public async Task OnSubmitNewAnswerAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (AuthenticationState.User.Identity.IsAuthenticated)
                {
                    using var insertNewAnswerCommandHandler = ServiceProvider.GetService<InsertNewAnswerCommandHandler>();
                    await insertNewAnswerCommandHandler.HandleAsync(InsertNewAnswerCommand);
                    await ShowSuccessMessageAsync("Your answer has been successfully submitted.");
                    await ReloadQuestionAsync();
                    InsertNewAnswerCommand.Description = string.Empty;
                }
            });
        }

        [Authorize]
        public async Task OnVoteForAnswerAsync(string protectedId, bool upVote)
        {
            await ExecuteAsync(async () =>
            {
                if (AuthenticationState.User.Identity.IsAuthenticated)
                {
                    using var voteForAnswerCommandHandler = ServiceProvider.GetService<VoteForAnswerCommandHandler>();
                    await voteForAnswerCommandHandler.HandleAsync(new VoteForAnswerCommand()
                    {
                        ProtectedAnswerId = protectedId,
                        UpVote = upVote
                    });
                    await ShowSuccessMessageAsync("Your vote has been successfully submitted.");
                    await ReloadQuestionAsync();
                }
            });
        }

        [Authorize]
        public async Task OnSelectBestAnswerAsync(string protectedId)
        {
            await ExecuteAsync(async () =>
            {
                if (AuthenticationState.User.Identity.IsAuthenticated)
                {
                    using var selectBestAnswerCommandHandler = ServiceProvider.GetService<SelectBestAnswerCommandHandler>();
                    await selectBestAnswerCommandHandler.HandleAsync(new SelectBestAnswerCommand()
                    {
                        ProtectedAnswerId = protectedId
                    });
                    await ShowSuccessMessageAsync("Your selection has been successfully submitted.");
                    await ReloadQuestionAsync();
                }
            });
        }
    }
}
