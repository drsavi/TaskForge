using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Services;
using TaskForge.Domain.Entities;

namespace TaskForge.Tests.ApplicationTests
{
    public class ProjectServiceTests
    {
        private class InMemoryRepository : IProjectRepository
        {
            public readonly List<Project> Projects = [];

            public Task AddAsync(Project project, CancellationToken cancellationtoken)
            {
                Projects.Add(project);
                return Task.CompletedTask;
            }

            public Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationtoken)
            {
                return Task.FromResult((IEnumerable<Project>)Projects);
            }

            public Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationtoken)
            {
                return Task.FromResult(Projects.FirstOrDefault(p => p.Id == id));
            }

            public Task UpdateAsync(Project project, CancellationToken cancellationtoken)
            {
                return Task.CompletedTask;
            }
            public Task DeleteAsync(Project project, CancellationToken cancellationtoken)
            {
                Projects.Remove(project);
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task CreateAsync_ShouldAddNewProjectAndReturnDto()
        {
            var repository = new InMemoryRepository();
            var service = new ProjectService(repository);
            var dto = new CreateProjectDto("Lunar Gateway", "Orbital outpost design");

            var result = await service.CreateAsync(dto, CancellationToken.None);

            Assert.Single(repository.Projects);
            Assert.Equal(result.Id, repository.Projects[0].Id);
            Assert.Equal("Lunar Gateway", result.Name);
            Assert.Equal("Orbital outpost design", result.Description);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllExistingProjects()
        {
            var repository = new InMemoryRepository();
            repository.Projects.Add(new Project(Guid.NewGuid(), "Global Connector"));
            repository.Projects.Add(new Project(Guid.NewGuid(), "Horizon Mission"));
            var service = new ProjectService(repository);

            var list = (await service.GetAllAsync(CancellationToken.None)).ToList();

            Assert.Equal(2, list.Count);
            Assert.Contains(list, d => d.Name == "Global Connector");
            Assert.Contains(list, d => d.Name == "Horizon Mission");
        }

        [Fact]
        public async Task UpdateAsync_WhenProjectNotFound_ReturnsFalse()
        {
            var service = new ProjectService(new InMemoryRepository());
            var updated = await service.UpdateAsync(
                Guid.NewGuid(),
                new UpdateProjectDto("Deep Space", "Exploration phase"),
                CancellationToken.None);

            Assert.False(updated);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProjectAndReturnTrue()
        {
            var repository = new InMemoryRepository();
            var project = new Project(Guid.NewGuid(), "Aurora Initiative");
            repository.Projects.Add(project);
            var service = new ProjectService(repository);

            var deleted = await service.DeleteAsync(project.Id, CancellationToken.None);

            Assert.True(deleted);
            Assert.Empty(repository.Projects);
        }
    }
}