using FluentValidation;

namespace Application.Features.ProjectManagement.Tasks.Commands.ChangeTaskStatus
{
    internal class ChangeTaskStatusCommandValidator : AbstractValidator<ChangeTaskStatusCommand>
    {
        public ChangeTaskStatusCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be a positive number.");

            RuleFor(x => x.TaskStatusId)
                .GreaterThan((byte)0).WithMessage("TaskStatusId must be a positive number.");
        }
    }
}
