using FluentValidation;

namespace Application.Features.ProjectManagement.Comments.Commands.UpdateComment
{
    internal class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
    {
        public UpdateCommentCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be a positive number.");

            RuleFor(x => x.Text)
                .NotEmpty()
                .MaximumLength(1000);
        }
    }
}
