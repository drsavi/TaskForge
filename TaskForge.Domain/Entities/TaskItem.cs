using TaskForge.Domain.Enums;

namespace TaskForge.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; private set; }
        public Guid ProjectId { get; private set; }
        public string Title { get; private set; } = default!;
        public string? Description { get; private set; }
        public TaskItemStatus Status { get; private set; }
        public TaskItemPriority Priority { get; private set; }
        public DateTimeOffset? DueDate { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }
        public DateTimeOffset? CompletedAt { get; private set; }

        protected TaskItem() { }

        public TaskItem(
            Guid id,
            Guid projectId,
            string title,
            string? description = null,
            TaskItemPriority priority = TaskItemPriority.Medium,
            DateTimeOffset? dueDate = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Task title is required.", nameof(title));

            Id = id;
            ProjectId = projectId;
            Title = title;
            Description = description;
            Status = TaskItemStatus.Pending;
            Priority = priority;
            DueDate = dueDate;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public void UpdateDetails(
            string title,
            string? description,
            TaskItemPriority priority,
            TaskItemStatus status,
            DateTimeOffset? dueDate)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Task title is required.", nameof(title));

            Title = title;
            Description = description;
            Priority = priority;
            DueDate = dueDate;
            ChangeStatus(status);
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void ChangeStatus(TaskItemStatus newStatus)
        {
            if (Status == newStatus)
                return;

            if (newStatus == TaskItemStatus.Completed)
                CompletedAt = DateTimeOffset.UtcNow;
            else if (Status == TaskItemStatus.Completed)
                CompletedAt = null;

            Status = newStatus;
        }
    }
}
