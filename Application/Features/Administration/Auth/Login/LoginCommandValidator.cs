using FluentValidation;

namespace Application.Features.Administration.Auth.Login
{
    internal class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(10);

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
