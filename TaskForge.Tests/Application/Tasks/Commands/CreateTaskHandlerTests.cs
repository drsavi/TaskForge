using FluentAssertions;
using Moq;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Application.Tasks.Commands.CreateTask;
using TaskForge.Domain.Entities;
using TaskForge.Domain.Enums;

namespace TaskForge.Application.Tests.Tasks.Commands
{
    public class CreateTaskHandlerTests
    {
        private const string UserId = "user-abc";
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<ITaskItemRepository> _mockTaskRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly CreateTaskHandler _handler;

        public CreateTaskHandlerTests()
        {
            _mockProjectRepository = new Mock<IProjectRepository>();
            _mockTaskRepository = new Mock<ITaskItemRepository>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _mockCurrentUser.Setup(u => u.UserId).Returns(UserId);
            _handler = new CreateTaskHandler(
                _mockProjectRepository.Object,
                _mockTaskRepository.Object,
                _mockCurrentUser.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesTaskForOwnedProject()
        {
            var projectId = Guid.NewGuid();
            var project = new Project(projectId, UserId, "Project", null);
            TaskItem? capturedTask = null;

            _mockProjectRepository
                .Setup(r => r.GetByIdForOwnerAsync(projectId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            _mockTaskRepository
                .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .Callback<TaskItem, CancellationToken>((t, ct) => capturedTask = t)
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(
                new CreateTaskCommand(projectId, "New task", "Desc", TaskItemPriority.High),
                CancellationToken.None);

            result.Should().NotBeEmpty();
            capturedTask.Should().NotBeNull();
            capturedTask!.ProjectId.Should().Be(projectId);
            capturedTask.Title.Should().Be("New task");
            capturedTask.Priority.Should().Be(TaskItemPriority.High);
            capturedTask.Status.Should().Be(TaskItemStatus.Pending);
        }

        [Fact]
        public async Task Handle_UnownedProject_ThrowsKeyNotFoundException()
        {
            var projectId = Guid.NewGuid();

            _mockProjectRepository
                .Setup(r => r.GetByIdForOwnerAsync(projectId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Project?)null);

            Func<Task> act = async () => await _handler.Handle(
                new CreateTaskCommand(projectId, "Task", null, null),
                CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}
