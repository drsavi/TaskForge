using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Services;
using TaskForge.Domain.Entities;

namespace TaskForge.Tests.DomainTests
{
    public class ProjectTests
    {
        [Fact]
        public void Ctor_WithValidName_SetsProperties()
        {
            var id = Guid.NewGuid();
            var project = new Project(id, "Aurora Initiative", "Orbital outpost design");

            Assert.Equal(id, project.Id);
            Assert.Equal("Aurora Initiative", project.Name);
            Assert.Equal("Orbital outpost design", project.Description);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Ctor_WithInvalidName_Throws(string invalidName)
        {
            var id = Guid.NewGuid();
            var ex = Assert.Throws<ArgumentException>(() => new Project(id, invalidName));
            Assert.Contains("required", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void UpdateDetails_WithValidData_UpdatesProperties()
        {
            var project = new Project(Guid.NewGuid(), "Horizon Mission", "Deep space exploration");
            project.UpdateDetails("Horizon Mission Phase II", "Extended mission scope");

            Assert.Equal("Horizon Mission Phase II", project.Name);
            Assert.Equal("Extended mission scope", project.Description);
        }

        [Fact]
        public void UpdateDetails_WithInvalidName_Throws()
        {
            var project = new Project(Guid.NewGuid(), "Nexus Gateway");
            Assert.Throws<ArgumentException>(
                () => project.UpdateDetails("", "Reconnect modules"));
        }
    }
}