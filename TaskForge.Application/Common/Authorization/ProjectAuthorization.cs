using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Domain.Entities;

namespace TaskForge.Application.Common.Authorization
{
    internal static class ProjectAuthorization
    {
        public static async Task<Project> GetOwnedProjectAsync(
            IProjectRepository repository,
            Guid projectId,
            string ownerId,
            CancellationToken cancellationToken)
        {
            var project = await repository.GetByIdForOwnerAsync(projectId, ownerId, cancellationToken);

            if (project is null)
                throw new KeyNotFoundException($"Project id '{projectId}' not found.");

            return project;
        }
    }
}
