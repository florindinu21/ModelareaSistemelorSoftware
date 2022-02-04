using FluentValidation;

namespace QA.Infrastructure.Controllers.SelectBestAnswerController
{
    public class SelectBestAnswerCommand
    {
        public string ProtectedAnswerId { get; set; }
    }

    public class SelectBestAnswerCommandValidator
        : AbstractValidator<SelectBestAnswerCommand>
    {
        public SelectBestAnswerCommandValidator()
        {
            RuleFor(x => x.ProtectedAnswerId)
                .NotEmpty();
        }
    }
}
