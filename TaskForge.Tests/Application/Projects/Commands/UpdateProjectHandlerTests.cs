using FluentAssertions;
using Moq;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Application.Projects.Commands.UpdateProject;
using TaskForge.Domain.Entities;

namespace TaskForge.Application.Tests.Projects.Commands
{
    public class UpdateProjectHandlerTests
    {
        private const string UserId = "user-abc";
        private readonly Mock<IProjectRepository> _mockRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly UpdateProjectHandler _handler;

        public UpdateProjectHandlerTests()
        {
            _mockRepository = new Mock<IProjectRepository>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _mockCurrentUser.Setup(u => u.UserId).Returns(UserId);
            _handler = new UpdateProjectHandler(_mockRepository.Object, _mockCurrentUser.Object);
        }

        [Fact]
        public async Task Handle_OwnedProject_ReturnsTrue()
        {
            var projectId = Guid.NewGuid();
            var project = new Project(projectId, UserId, "Old", null);

            _mockRepository
                .Setup(r => r.GetByIdForOwnerAsync(projectId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            var result = await _handler.Handle(
                new UpdateProjectCommand(projectId, "New", "Updated"),
                CancellationToken.None);

            result.Should().BeTrue();
            project.Name.Should().Be("New");
            _mockRepository.Verify(r => r.UpdateAsync(project, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_OtherUsersProject_ReturnsFalse()
        {
            var projectId = Guid.NewGuid();

            _mockRepository
                .Setup(r => r.GetByIdForOwnerAsync(projectId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Project?)null);

            var result = await _handler.Handle(
                new UpdateProjectCommand(projectId, "New", null),
                CancellationToken.None);

            result.Should().BeFalse();
        }
    }
}
