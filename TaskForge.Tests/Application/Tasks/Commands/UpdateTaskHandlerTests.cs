using FluentAssertions;
using Moq;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Application.Tasks.Commands.UpdateTask;
using TaskForge.Domain.Entities;
using TaskForge.Domain.Enums;

namespace TaskForge.Application.Tests.Tasks.Commands
{
    public class UpdateTaskHandlerTests
    {
        private const string UserId = "user-abc";
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<ITaskItemRepository> _mockTaskRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly UpdateTaskHandler _handler;

        public UpdateTaskHandlerTests()
        {
            _mockProjectRepository = new Mock<IProjectRepository>();
            _mockTaskRepository = new Mock<ITaskItemRepository>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _mockCurrentUser.Setup(u => u.UserId).Returns(UserId);
            _handler = new UpdateTaskHandler(
                _mockProjectRepository.Object,
                _mockTaskRepository.Object,
                _mockCurrentUser.Object);
        }

        [Fact]
        public async Task Handle_CompleteTask_SetsCompletedAt()
        {
            var projectId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var project = new Project(projectId, UserId, "Project", null);
            var task = new TaskItem(taskId, projectId, "Task");

            _mockProjectRepository
                .Setup(r => r.GetByIdForOwnerAsync(projectId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            _mockTaskRepository
                .Setup(r => r.GetByIdAsync(projectId, taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            var result = await _handler.Handle(
                new UpdateTaskCommand(
                    projectId,
                    taskId,
                    "Task",
                    null,
                    TaskItemPriority.High,
                    TaskItemStatus.Completed,
                    null),
                CancellationToken.None);

            result.Should().BeTrue();
            task.Status.Should().Be(TaskItemStatus.Completed);
            task.Priority.Should().Be(TaskItemPriority.High);
            task.CompletedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ReopenTask_ClearsCompletedAt()
        {
            var projectId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var project = new Project(projectId, UserId, "Project", null);
            var task = new TaskItem(taskId, projectId, "Task");
            task.ChangeStatus(TaskItemStatus.Completed);

            _mockProjectRepository
                .Setup(r => r.GetByIdForOwnerAsync(projectId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            _mockTaskRepository
                .Setup(r => r.GetByIdAsync(projectId, taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            var result = await _handler.Handle(
                new UpdateTaskCommand(
                    projectId,
                    taskId,
                    "Task",
                    null,
                    TaskItemPriority.Medium,
                    TaskItemStatus.InProgress,
                    null),
                CancellationToken.None);

            result.Should().BeTrue();
            task.Status.Should().Be(TaskItemStatus.InProgress);
            task.CompletedAt.Should().BeNull();
        }
    }
}
