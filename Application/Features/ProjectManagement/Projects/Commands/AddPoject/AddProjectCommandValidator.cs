using FluentValidation;

namespace Application.Features.ProjectManagement.Projects.Commands.AddPoject
{
    internal class AddProjectCommandValidator : AbstractValidator<AddProjectCommand>
    {
        public AddProjectCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Code)
                .NotEmpty()
                .MaximumLength(10);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500);
        }
    }
}
