namespace TaskForge.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; private set; }
        public string OwnerId { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }
        public ICollection<TaskItem> Tasks { get; private set; } = new List<TaskItem>();

        protected Project() { }

        public Project(Guid id, string ownerId, string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                throw new ArgumentException("Owner id is required.", nameof(ownerId));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name is required.", nameof(name));

            Id = id;
            OwnerId = ownerId;
            Name = name;
            Description = description;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public void UpdateDetails(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name is required.", nameof(name));

            Name = name;
            Description = description;
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
