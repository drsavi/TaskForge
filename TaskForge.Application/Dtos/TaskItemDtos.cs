using TaskForge.Domain.Enums;

namespace TaskForge.Application.Dtos
{
    public record CreateTaskRequest(
        string Title,
        string? Description,
        TaskItemPriority? Priority);

    public record UpdateTaskRequest(
        string Title,
        string? Description,
        TaskItemPriority Priority,
        TaskItemStatus Status,
        DateTimeOffset? DueDate);

    public record TaskItemDto(
        Guid Id,
        Guid ProjectId,
        string Title,
        string? Description,
        TaskItemStatus Status,
        TaskItemPriority Priority,
        DateTimeOffset? DueDate,
        DateTimeOffset CreatedAt,
        DateTimeOffset? UpdatedAt,
        DateTimeOffset? CompletedAt);
}
