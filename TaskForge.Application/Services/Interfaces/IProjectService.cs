using TaskForge.Application.Dtos;

namespace TaskForge.Application.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<ProjectDto> CreateAsync(CreateProjectDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Guid id, UpdateProjectDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}