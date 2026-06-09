using FluentValidation;
using TaskForge.Application.Tasks.Commands.CreateTask;

namespace TaskForge.Application.Common.Validators
{
    public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskCommandValidator()
        {
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.Priority).IsInEnum().When(x => x.Priority.HasValue);
        }
    }
}
