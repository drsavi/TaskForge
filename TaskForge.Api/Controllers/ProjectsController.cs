using Microsoft.AspNetCore.Mvc;
using TaskForge.Application.Constants;
using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Services;

namespace TaskForge.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _service;

        public ProjectsController(IProjectService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<ProjectDto>> GetAll(CancellationToken cancellationtoken)
            => await _service.GetAllAsync(cancellationtoken);

        [HttpGet(RouteConstants.Id)]
        public async Task<ActionResult<ProjectDto>> GetById(Guid id, CancellationToken cancellationtoken)
        {
            var dto = await _service.GetByIdAsync(id, cancellationtoken);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto dto, CancellationToken cancellationtoken)
        {
            var created = await _service.CreateAsync(dto, cancellationtoken);
            return CreatedAtAction(nameof(GetById),
                                   new { id = created.Id },
                                   created);
        }

        [HttpPut(RouteConstants.Id)]
        public async Task<IActionResult> Update(Guid id, UpdateProjectDto dto, CancellationToken cancellationtoken)
            => await _service.UpdateAsync(id, dto, cancellationtoken)
                 ? NoContent()
                 : NotFound();

        [HttpDelete(RouteConstants.Id)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationtoken)
            => await _service.DeleteAsync(id, cancellationtoken)
                 ? NoContent()
                 : NotFound();
    }
}