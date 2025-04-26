namespace TaskForge.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }

        protected Project() { }

        public Project(Guid id, string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name é obrigatório.", nameof(name));

            Id = id;
            Name = name;
            Description = description;
        }

        public void UpdateDetails(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name é obrigatório.", nameof(name));

            Name = name;
            Description = description;
        }
    }
}
