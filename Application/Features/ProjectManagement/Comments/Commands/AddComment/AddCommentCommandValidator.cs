using FluentValidation;

namespace Application.Features.ProjectManagement.Comments.Commands.AddComment
{
    internal class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
    {
        public AddCommentCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.TaskId)
                .GreaterThan(0).WithMessage("TaskId must be a positive number.");

            RuleFor(x => x.Text)
                .NotEmpty()
                .MaximumLength(1000);
        }
    }
}
