using FluentAssertions;
using Moq;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Projects.Commands.CreateProject;
using TaskForge.Domain.Entities;

namespace TaskForge.Application.Tests.Projects.Commands
{
    public class CreateProjectHandlerTests
    {
        private readonly Mock<IProjectRepository> _mockRepository;
        private readonly CreateProjectHandler _handler;

        public CreateProjectHandlerTests()
        {
            _mockRepository = new Mock<IProjectRepository>();
            _handler = new CreateProjectHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateProjectAndReturnId()
        {
            var command = new CreateProjectCommand("Valid Project", "Test Description");
            Project? capturedProject = null;

            _mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
                .Callback<Project, CancellationToken>((p, ct) => capturedProject = p)
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeEmpty();
            capturedProject.Should().NotBeNull();
            capturedProject!.Name.Should().Be(command.Name);
            capturedProject.Description.Should().Be(command.Description);
            capturedProject.Id.Should().Be(result);

            _mockRepository.Verify(
                r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_InvalidName_ShouldThrowArgumentException(string invalidName)
        {
            var command = new CreateProjectCommand(invalidName, "Invalid Name Test");

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Project name is required. (Parameter 'name')");
        }

        [Fact]
        public async Task Handle_WithoutDescription_ShouldCreateProjectWithNullDescription()
        {
            // Arrange
            var command = new CreateProjectCommand("No Description Project", null);
            Project? capturedProject = null;

            _mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
                .Callback<Project, CancellationToken>((p, ct) => capturedProject = p);

            await _handler.Handle(command, CancellationToken.None);

            capturedProject.Should().NotBeNull();
            capturedProject!.Description.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldPropagateCancellationToken()
        {
            var command = new CreateProjectCommand("Test Project", null);
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            await _handler.Handle(command, token);

            _mockRepository.Verify(
                r => r.AddAsync(It.IsAny<Project>(), token),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_DatabaseError_ShouldPropagateException()
        {
            var command = new CreateProjectCommand("Error Project", "Test");
            var expectedException = new InvalidOperationException("Database failure");

            _mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(expectedException.Message);
        }
    }
}