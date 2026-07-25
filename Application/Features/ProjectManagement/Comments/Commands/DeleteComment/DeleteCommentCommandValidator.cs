using FluentValidation;

namespace Application.Features.ProjectManagement.Comments.Commands.DeleteComment
{
    internal class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
    {
        public DeleteCommentCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be a positive number.");
        }
    }
}
