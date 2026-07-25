using FluentValidation;

namespace Application.Features.ProjectManagement.Tasks.Commands.DeleteTask
{
    internal class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
    {
        public DeleteTaskCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be a positive number.");
        }
    }
}
