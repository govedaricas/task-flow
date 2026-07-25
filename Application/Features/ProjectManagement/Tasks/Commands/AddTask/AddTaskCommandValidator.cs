using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ProjectManagement.Tasks.Commands.AddTask
{
    internal class AddTaskCommandValidator : AbstractValidator<AddTaskCommand>
    {
        public AddTaskCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("ProjectId must be a positive number.");

            RuleFor(x => x.AssignedUserId)
                .GreaterThan(0).WithMessage("AssignedUserId must be a positive number.")
                .When(x => x.AssignedUserId.HasValue);

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("DueDate must be in the future.")
                .When(x => x.DueDate.HasValue);

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .When(x => x.Description != null);
        }
    }
}
