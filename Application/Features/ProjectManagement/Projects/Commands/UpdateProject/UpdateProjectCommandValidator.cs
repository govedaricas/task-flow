using FluentValidation;

namespace Application.Features.ProjectManagement.Projects.Commands.UpdateProject
{
    internal class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
    {
        public UpdateProjectCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be a positive number.");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => x.Description != null);
        }
    }
}
