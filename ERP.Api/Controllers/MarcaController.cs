using ERP.Domain;
using ERP.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MarcaController : ControllerBase
    {
        private readonly MarcasService service;
        public MarcaController(MarcasService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await service.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var marca = await service.GetById(id);
            if (marca == null) return NotFound();
            return Ok(marca);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Marca marca)
            => CreatedAtAction(nameof(GetById), new { id = marca.Id }, await service.Create(marca));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Marca input)
        {
            var marca = await service.Update(id, input);
            if (marca == null) return NotFound();
            return Ok(marca);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await service.Delete(id);
            if (!result) return NotFound();
            return Ok();
        }
    }
}