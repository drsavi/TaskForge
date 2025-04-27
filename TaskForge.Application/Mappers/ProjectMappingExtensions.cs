using TaskForge.Application.Dtos;
using TaskForge.Domain.Entities;

namespace TaskForge.Application.Mappers
{
    public static class ProjectMappingExtensions
    {
        public static ProjectDto ToDto(this Project project)
            => new ProjectDto(project.Id, project.Name, project.Description);

        public static Project ToDomain(this CreateProjectDto dto)
            => new Project(Guid.NewGuid(), dto.Name, dto.Description);

        public static void ApplyUpdate(this Project project, UpdateProjectDto dto)
            => project.UpdateDetails(dto.Name, dto.Description);
    }
}