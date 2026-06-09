using TaskForge.Application.Dtos;
using TaskForge.Domain.Entities;

namespace TaskForge.Application.Mappers
{
    public static class TaskItemMappingExtensions
    {
        public static TaskItemDto ToDto(this TaskItem task) =>
            new(
                task.Id,
                task.ProjectId,
                task.Title,
                task.Description,
                task.Status,
                task.Priority,
                task.DueDate,
                task.CreatedAt,
                task.UpdatedAt,
                task.CompletedAt);
    }
}
