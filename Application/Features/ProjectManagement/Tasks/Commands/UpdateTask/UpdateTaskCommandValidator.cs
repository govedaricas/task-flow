using FluentValidation;

namespace Application.Features.ProjectManagement.Tasks.Commands.UpdateTask
{
    internal class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
    {
        public UpdateTaskCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be a positive number.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .When(x => x.Description != null);

            RuleFor(x => x.AssignedToUserId)
                .GreaterThan(0).WithMessage("AssignedToUserId must be a positive number.")
                .When(x => x.AssignedToUserId.HasValue);

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("DueDate must be in the future.")
                .When(x => x.DueDate.HasValue);
        }
    }
}
