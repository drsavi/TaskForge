using TaskForge.Domain.Entities;

namespace TaskForge.Tests.DomainTests
{
    public class ProjectTests
    {
        private const string OwnerId = "user-123";

        [Fact]
        public void Ctor_WithValidName_SetsProperties()
        {
            var id = Guid.NewGuid();
            var project = new Project(id, OwnerId, "Aurora Initiative", "Orbital outpost design");

            Assert.Equal(id, project.Id);
            Assert.Equal(OwnerId, project.OwnerId);
            Assert.Equal("Aurora Initiative", project.Name);
            Assert.Equal("Orbital outpost design", project.Description);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Ctor_WithInvalidName_Throws(string invalidName)
        {
            var id = Guid.NewGuid();
            var ex = Assert.Throws<ArgumentException>(() => new Project(id, OwnerId, invalidName));
            Assert.Contains("required", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Ctor_WithInvalidOwnerId_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() => new Project(Guid.NewGuid(), "", "Valid Name"));
            Assert.Contains("Owner", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesProperties()
        {
            var project = new Project(Guid.NewGuid(), OwnerId, "Horizon Mission", "Deep space exploration");
            project.UpdateDetails("Horizon Mission Phase II", "Extended mission scope");

            Assert.Equal("Horizon Mission Phase II", project.Name);
            Assert.Equal("Extended mission scope", project.Description);
        }

        [Fact]
        public void UpdateDetails_WithInvalidName_Throws()
        {
            var project = new Project(Guid.NewGuid(), OwnerId, "Nexus Gateway");
            Assert.Throws<ArgumentException>(
                () => project.UpdateDetails("", "Reconnect modules"));
        }
    }
}
