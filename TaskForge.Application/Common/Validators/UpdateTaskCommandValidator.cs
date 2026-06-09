using FluentValidation;
using TaskForge.Application.Tasks.Commands.UpdateTask;

namespace TaskForge.Application.Common.Validators
{
    public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
    {
        public UpdateTaskCommandValidator()
        {
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.TaskId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.Priority).IsInEnum();
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}
