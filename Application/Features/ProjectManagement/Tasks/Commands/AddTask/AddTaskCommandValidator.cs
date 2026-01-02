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

            RuleFor(x => x.Description)
                .MaximumLength(100);

            RuleFor(x => x.Name)
                .MaximumLength(100);
        }
    }
}
