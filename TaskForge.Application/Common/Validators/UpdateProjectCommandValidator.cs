using FluentValidation;
using TaskForge.Application.Projects.Commands.UpdateProject;

namespace TaskForge.Application.Common.Validators
{
    public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
    {
        public UpdateProjectCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Project name is required.")
                .MaximumLength(100)
                .WithMessage("Project name must not exceed 100 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Project description must not exceed 500 characters.");
        }
    }
}
