using TaskForge.Domain.Entities;
using TaskForge.Domain.Enums;

namespace TaskForge.Tests.DomainTests
{
    public class TaskItemTests
    {
        private static readonly Guid ProjectId = Guid.NewGuid();

        [Fact]
        public void Ctor_WithValidTitle_SetsDefaults()
        {
            var id = Guid.NewGuid();
            var task = new TaskItem(id, ProjectId, "Buy supplies");

            Assert.Equal(id, task.Id);
            Assert.Equal(ProjectId, task.ProjectId);
            Assert.Equal("Buy supplies", task.Title);
            Assert.Equal(TaskItemStatus.Pending, task.Status);
            Assert.Equal(TaskItemPriority.Medium, task.Priority);
            Assert.Null(task.CompletedAt);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Ctor_WithInvalidTitle_Throws(string invalidTitle)
        {
            var ex = Assert.Throws<ArgumentException>(() => new TaskItem(Guid.NewGuid(), ProjectId, invalidTitle));
            Assert.Contains("required", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ChangeStatus_ToCompleted_SetsCompletedAt()
        {
            var task = new TaskItem(Guid.NewGuid(), ProjectId, "Finish report");
            task.ChangeStatus(TaskItemStatus.Completed);

            Assert.Equal(TaskItemStatus.Completed, task.Status);
            Assert.NotNull(task.CompletedAt);
        }

        [Fact]
        public void ChangeStatus_FromCompletedToInProgress_ClearsCompletedAt()
        {
            var task = new TaskItem(Guid.NewGuid(), ProjectId, "Reopen task");
            task.ChangeStatus(TaskItemStatus.Completed);
            task.ChangeStatus(TaskItemStatus.InProgress);

            Assert.Equal(TaskItemStatus.InProgress, task.Status);
            Assert.Null(task.CompletedAt);
        }

        [Fact]
        public void UpdateDetails_ChangesStatus_AppliesCompletedAtRule()
        {
            var task = new TaskItem(Guid.NewGuid(), ProjectId, "Update me");
            task.UpdateDetails(
                "Updated title",
                "Notes",
                TaskItemPriority.High,
                TaskItemStatus.Completed,
                null);

            Assert.Equal("Updated title", task.Title);
            Assert.Equal(TaskItemPriority.High, task.Priority);
            Assert.Equal(TaskItemStatus.Completed, task.Status);
            Assert.NotNull(task.CompletedAt);
        }
    }
}
