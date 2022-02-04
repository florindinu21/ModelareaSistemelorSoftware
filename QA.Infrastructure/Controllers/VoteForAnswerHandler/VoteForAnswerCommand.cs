using FluentValidation;

namespace QA.Infrastructure.Controllers.VoteForAnswerHandler
{
    public class VoteForAnswerCommand
    {
        public string ProtectedAnswerId { get; set; }
        public bool UpVote { get; set; }
    }

    public class VoteForAnswerCommandValidator
        : AbstractValidator<VoteForAnswerCommand>
    {
        public VoteForAnswerCommandValidator()
        {
            RuleFor(x => x.ProtectedAnswerId)
                .NotEmpty();
        }
    }
}
