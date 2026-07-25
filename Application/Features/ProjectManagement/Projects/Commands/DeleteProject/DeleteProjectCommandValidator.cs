using FluentValidation;

namespace Application.Features.ProjectManagement.Projects.Commands.DeleteProject
{
    internal class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
    {
        public DeleteProjectCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be a positive number.");
        }
    }
}
