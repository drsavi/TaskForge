using FluentAssertions;
using Moq;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Application.Projects.Queries.GetProjectById;
using TaskForge.Domain.Entities;

namespace TaskForge.Application.Tests.Projects.Queries
{
    public class GetProjectByIdHandlerTests
    {
        private const string UserId = "user-abc";
        private readonly Mock<IProjectRepository> _mockRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly GetProjectByIdHandler _handler;

        public GetProjectByIdHandlerTests()
        {
            _mockRepository = new Mock<IProjectRepository>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _mockCurrentUser.Setup(u => u.UserId).Returns(UserId);
            _handler = new GetProjectByIdHandler(_mockRepository.Object, _mockCurrentUser.Object);
        }

        [Fact]
        public async Task Handle_OwnedProject_ReturnsDto()
        {
            var projectId = Guid.NewGuid();
            var project = new Project(projectId, UserId, "Owned", "Desc");

            _mockRepository
                .Setup(r => r.GetByIdForOwnerAsync(projectId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            var result = await _handler.Handle(new GetProjectByIdQuery(projectId), CancellationToken.None);

            result.Should().NotBeNull();
            result!.Id.Should().Be(projectId);
            result.Name.Should().Be("Owned");
        }

        [Fact]
        public async Task Handle_OtherUsersProject_ReturnsNull()
        {
            var projectId = Guid.NewGuid();

            _mockRepository
                .Setup(r => r.GetByIdForOwnerAsync(projectId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Project?)null);

            var result = await _handler.Handle(new GetProjectByIdQuery(projectId), CancellationToken.None);

            result.Should().BeNull();
        }
    }
}
