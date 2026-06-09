using FluentAssertions;
using Moq;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Application.Projects.Commands.DeleteProject;
using TaskForge.Domain.Entities;

namespace TaskForge.Application.Tests.Projects.Commands
{
    public class DeleteProjectHandlerTests
    {
        private const string UserId = "user-abc";
        private readonly Mock<IProjectRepository> _mockRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly DeleteProjectHandler _handler;

        public DeleteProjectHandlerTests()
        {
            _mockRepository = new Mock<IProjectRepository>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _mockCurrentUser.Setup(u => u.UserId).Returns(UserId);
            _handler = new DeleteProjectHandler(_mockRepository.Object, _mockCurrentUser.Object);
        }

        [Fact]
        public async Task Handle_OwnedProject_ReturnsTrue()
        {
            var projectId = Guid.NewGuid();
            var project = new Project(projectId, UserId, "Delete me", null);

            _mockRepository
                .Setup(r => r.GetByIdForOwnerAsync(projectId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            var result = await _handler.Handle(new DeleteProjectCommand(projectId), CancellationToken.None);

            result.Should().BeTrue();
            _mockRepository.Verify(r => r.DeleteAsync(project, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_OtherUsersProject_ReturnsFalse()
        {
            var projectId = Guid.NewGuid();

            _mockRepository
                .Setup(r => r.GetByIdForOwnerAsync(projectId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Project?)null);

            var result = await _handler.Handle(new DeleteProjectCommand(projectId), CancellationToken.None);

            result.Should().BeFalse();
        }
    }
}
